using StateMachine;
using UnityEngine.AI;

public class EnemyContext : ReactiveContext<EnemyContext>
{
    public int baseHealth;
    public int maxHealth;

    private int currentHealth;

    public int CurrentHealth
    {
        get => currentHealth;
        set => SetProperty(ref currentHealth, value, nameof(CurrentHealth));
    }

    public NavMeshAgent agent;
}
