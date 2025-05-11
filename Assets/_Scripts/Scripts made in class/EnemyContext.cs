using StateMachine;
using UnityEngine.AI;
using UnityEngine;

public class EnemyContext : ReactiveContext<EnemyContext>
{
    [HideInInspector] public bool IsDead = false;
    public int baseHealth;
    public int maxHealth;
    public CapsuleCollider cc;
    private int currentHealth;
    public int CurrentHealth
    {
        get => currentHealth;
        set => SetProperty(ref currentHealth, value, nameof(CurrentHealth));
    }

    [HideInInspector] public NavMeshAgent agent;
    public Collider attackHitBox;
    public Transform[] Path;
    public int PathIndex;
    [HideInInspector] public Transform Player;

    public float followRange = 15f;
    public float attackRange = 2f;
    public float atkHitBoxTimer = 0.6f;
    public GameObject deathEffectPrefab;
    public Animator anim;

    public void TakeDamage(int damage)
    {
        // Reduce health
        CurrentHealth -= damage;

        // Check if health is 0 or below
        if (CurrentHealth <= 0 && !IsDead)
        {
            IsDead = true;
        }
    }
}
