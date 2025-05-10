using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class _Enemy : MonoBehaviour
{
    public enum EnemyState
    {
        Chase, Patrol, Attack
    }
    [Header("References")]
    public Transform player;
    public EnemyState currentState;

    Transform target;
    NavMeshAgent agent;

    [Header("Pathing")]
    public Transform[] path;
    public int pathIndex = 0;
    public float distThreshold = 0.2f; //Floating point math is inexact, this allows us to get close enough to a waypoint and move to the next one.

    #region player spawning
    private void OnEnable()
    {
        GameManager.Instance.OnPlayerSpawned += OnPlayerSpawnedCallback;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnPlayerSpawned -= OnPlayerSpawnedCallback;
    }
    private void OnPlayerSpawnedCallback(Player playerObj)
    {
        player = playerObj.transform;
    }
    #endregion
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void FixedUpdate()
    {
        if (!player) return;

        if (currentState == EnemyState.Chase) target = player;

        if (currentState == EnemyState.Patrol)
        {
            if (target == player) target = path[pathIndex];

            if (agent.remainingDistance < distThreshold)
            {
                pathIndex++;

                pathIndex %= path.Length; //0 mod 4 returns 0, 4 mod 4 return 0

                target = path[pathIndex];

                //if (pathIndex == path.Length) pathIndex = 0; //If we reach the end of the path - go back to zero               
            }
        }

        agent.SetDestination(target.position);
    }
}
