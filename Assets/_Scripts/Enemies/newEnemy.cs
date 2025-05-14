using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(Animator))]
public class newEnemy : MonoBehaviour
{
    public Transform[] path;
    public float distThreshold = 0.2f;
    
    private NavMeshAgent agent;
    private Transform player;

    [SerializeField] private EnemyContext context;
    private EnemyStateMachine state;

    private int pathIndex = 0;
    [Header("Enemy Stats")]
    public int hp = 15;
    public float followRange = 15f;
    public float attackRange = 2f;
    public float WhenToActivateHitBoxOnMeleeAtk = 0.6f;

    private CapsuleCollider cc;

    [SerializeField] private GameObject deathEffectPrefab;

    [SerializeField] private GameObject healthBarPrefab;
    private EnemyHealthBar healthBar;
    #region player spawning
    private void OnEnable()
    {
        GameManager.Instance.OnPlayerSpawned += OnPlayerSpawnedCallback;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnPlayerSpawned -= OnPlayerSpawnedCallback;
    }
    #endregion

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cc = GetComponent<CapsuleCollider>();
        GameObject bar = Instantiate(healthBarPrefab);
        healthBar = bar.GetComponent<EnemyHealthBar>();


        context = new EnemyContext()
        {
            baseHealth = hp,
            CurrentHealth = hp,
            maxHealth = hp,
            agent = agent,
            Path = path,
            PathIndex = pathIndex,
            followRange = followRange,
            attackRange = attackRange,
            anim = GetComponent<Animator>(),
            atkHitBoxTimer = WhenToActivateHitBoxOnMeleeAtk,
            deathEffectPrefab = deathEffectPrefab,
            cc = cc,
            healthBar = healthBar
        };
        healthBar.Initialize(transform, context);
        healthBar.SetHealth(context.CurrentHealth, context.maxHealth);
        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }

    }
    private void OnPlayerSpawnedCallback(Player playerObj)
    {
        player = playerObj.transform;
        context._player = playerObj;
        context.Player = player;

        if (state == null)
        {
            state = GetComponent<EnemyStateMachine>();
            //state.Initialize(context);
            state.InitializeEnemy(context);
        }
    }
    private void Start()
    {

    }

    private void Update()
    {
        if (player == null || context.IsDead) return;
        state.StateMachineUpdate();
    }
}
