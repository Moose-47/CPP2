using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;

public class RandomFishSpawn : MonoBehaviour
{
    public GameObject[] fishPrefabs;
    public int numberOfSpawnPoints = 5;
    public int fishToSpawn = 3;

    private List<Vector3> spawnPoints = new List<Vector3>();
    private int exceptionCount = 0;
    private const int maxExceptionsToPrint = 2;

    List<int> usedIndices = new List<int>();

    bool sharkSpawned = false;

    void Start()
    {
        GenerateRandomSpawnPoints();

        for (int i = 0; i < fishToSpawn; i++)
        {
            try
            {
                SpawnFishAtRandomPoint();
            }
            catch (Exception ex)
            {
                if (exceptionCount < maxExceptionsToPrint)
                {
                    Debug.LogWarning($"Exception [{exceptionCount + 1}]: {ex.Message}");
                }
                exceptionCount++;
            }
        }
    }

    void GenerateRandomSpawnPoints()
    {
        float minDistance = 3f;
        int attempts = 0;
        int maxAttempts = 100;
        int groundLayerMask = LayerMask.GetMask("ground");
        while (spawnPoints.Count < numberOfSpawnPoints && attempts < maxAttempts)
        {

            float x = UnityEngine.Random.Range(50f, 80f); //X-Axis
            float z = UnityEngine.Random.Range(-80f, -40f); //Z-Axis
            Vector3 rayOrigin = new Vector3(x, 200f, z);

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, 500f, groundLayerMask))
            {
                Vector3 randomSpawnPoint = hit.point;

                float verticalOffset = UnityEngine.Random.Range(0f, 10f);
                randomSpawnPoint.y += verticalOffset;
                randomSpawnPoint.y = Mathf.Clamp(randomSpawnPoint.y, hit.point.y, 10.5f);

                bool isFarEnough = true;
                foreach (Vector3 point in spawnPoints)
                {
                    if (Vector3.Distance(randomSpawnPoint, point) < minDistance)
                    {
                        isFarEnough = false;
                        break;
                    }
                }
                if (isFarEnough)
                {
                    spawnPoints.Add(randomSpawnPoint);
                }
                attempts++;
            }
        }
            if (spawnPoints.Count < numberOfSpawnPoints)
            {
                Debug.LogWarning($"Only generated {spawnPoints.Count} valid spawn points after {maxAttempts} attempts");
            }
        }

    
    void SpawnFishAtRandomPoint()
    {
        if (fishPrefabs.Length == 0)
            throw new Exception("Fish prefab array is empty");

        if (spawnPoints.Count == 0)
            throw new Exception("Spawn points not generated");

        int pointIndex;
        int maxTries = 10;
        int tries = 0;

        do
        {
            pointIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            tries++;
        }
        while (usedIndices.Contains(pointIndex) && tries < maxTries);
        
            if (usedIndices.Contains(pointIndex))
                throw new Exception("Could not find a unique spawn points");

            usedIndices.Add(pointIndex);

        int prefabIndex;

        do
        {
            prefabIndex = UnityEngine.Random.Range(0, fishPrefabs.Length);
        }
        while (prefabIndex == 4 && sharkSpawned);

        if (prefabIndex == 4)
        {
            sharkSpawned = true;
        }

        Quaternion randomRotation = Quaternion.Euler(0, UnityEngine.Random.Range(0f, 360f), 0);
        GameObject fish = Instantiate(fishPrefabs[prefabIndex], spawnPoints[pointIndex], randomRotation);
    }
}
