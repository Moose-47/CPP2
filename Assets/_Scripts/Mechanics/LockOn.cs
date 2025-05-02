using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class LockOn : MonoBehaviour
{
    [Header("Lock On Settings")]
    public float lockOnRadius = 20f;
    public float maxLockAngle = 60f;
    public string targetTag = "Enemy";
    public LayerMask obstructionMask;

    private PlayerState playerState;
    private CameraController cameraController;
    private Transform cameraTransform;
    private Transform player;

    private void Awake()
    {
        player = transform;
        playerState = GetComponentInParent<PlayerState>();
        cameraController = FindAnyObjectByType<CameraController>();
        cameraTransform = cameraController.cameraTransform;
    }
    public void HandleLockOnInput()
    {
        if (!playerState.IsLockedOn)
        {
            Transform newTarget = FindLockOnTarget();

            if (newTarget != null)
            {
                playerState.SetLockOn(newTarget);
                cameraController.ToggleLockOn(newTarget);
            }
            else
            {
                cameraController.PanToPlayerDirection();
            }
        }
        else
        {
            playerState.ClearLockOn();
            cameraController.ToggleLockOn(null);
        }
    }

    private Transform FindLockOnTarget()
    {
        Collider[] hits = Physics.OverlapSphere(player.position, lockOnRadius);
        List<Transform> visibleTargets = new List<Transform>();

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag(targetTag))
            {
                Vector3 dirToTarget = (hit.transform.position - cameraTransform.position).normalized;
                float angleToTarget = Vector3.Angle(cameraTransform.forward, dirToTarget);

                if (angleToTarget < maxLockAngle)
                {
                    float distance = Vector3.Distance(cameraTransform.position, hit.transform.position);
                    if (!Physics.Raycast(cameraTransform.position, dirToTarget, distance, obstructionMask))
                    {
                        visibleTargets.Add(hit.transform);
                    }
                }
            }
        }

        Transform closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (Transform target in visibleTargets)
        {
            float dist = Vector3.Distance(player.position, target.position);
            if (dist < closestDistance)
            {
                closestDistance = dist;
                closest = target;
            }
        }

        return closest;
    }
}
        