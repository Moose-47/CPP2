using UnityEngine;
using StateMachine;
using UnityEngine.AI;
using StateMachine.Editor;

public class EnemyStateMachine : StateMachine<EnemyContext>
{
    public IState<EnemyContext> idleState;
    public IState<EnemyContext> patrolState;
    public IState<EnemyContext> chaseState;


    void Start()
    {
        context = new EnemyContext()
        {
            baseHealth = 50,
            CurrentHealth = 50,
            maxHealth = 100,
            agent = GetComponent<NavMeshAgent>()
        };

        StateBuilder<EnemyContext> stateBuilder = new StateBuilder<EnemyContext>();

        idleState = stateBuilder
            .SetName("idle")
            .OnEnter(() => Debug.Log("Entering Idle State"))
            .Build();
        patrolState = stateBuilder
            .SetName("patrol")
            .OnEnter(() => Debug.Log("Entering Patrol State"))
            .Build();
        chaseState = stateBuilder
            .SetName("chase")
            .OnEnter(() => Debug.Log("Entering Chase State"))
            .Build();

        RegisterState("idle", idleState);
        RegisterState("patrol", patrolState);
        RegisterState("chase", chaseState);

        idleState.AddTransition(this)
            .To(patrolState)
            .When(idleToPatrolFunction)
            .WithAction(() => Debug.Log("Action for idle to patrol"))
            .WithPriority(10);


        StateMachineRegistry.RegisterStateMachine(this, "EnemyStateMachine");
    }

    bool idleToPatrolFunction()
    {
        return true;
    }
    
    //void Update()
    //{
        
    //}
}
