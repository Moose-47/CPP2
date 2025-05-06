using UnityEngine;
using StateMachine;
using UnityEngine.AI;
using StateMachine.Editor;
using System;

public class EnemyStateMachine : StateMachine<EnemyContext>
{
    public IState<EnemyContext> idleState;
    public IState<EnemyContext> patrolState;
    public IState<EnemyContext> chaseState;
    public IState<EnemyContext> atkState;


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
            .When(idleToPatrol)
            .WithAction(() => Debug.Log("Action for idle to patrol"))
            .WithPriority(10)
            .To(atkState)
            .When(anyStateToAtk)
            .WithPriority(10);

        patrolState.AddTransition(this)
            .To(idleState)
            .When(patrolToIdle)
            .WithAction(() => Debug.Log("Action for patrol to idle"))
            .WithPriority(10)
            .To(chaseState)
            .When(patrolToChase)
            .WithAction(() => Debug.Log("Action for patrol to chase"))
            .WithPriority(10)
            .To(atkState)
            .When(anyStateToAtk)
            .WithPriority(10);

        chaseState.AddTransition(this)
            .To(idleState)
            .When(chaseToIdle)
            .WithAction(() => Debug.Log("Action for chase to idle"))
            .WithPriority(10)
            .To(atkState)
            .When(anyStateToAtk)
            .WithPriority(10);

        StateMachineRegistry.RegisterStateMachine(this, "EnemyStateMachine");
    }

    private bool anyStateToAtk()
    {
        throw new NotImplementedException();
    }

    private bool idleToPatrol()
    {
        throw new NotImplementedException();
    }

    private bool chaseToIdle()
    {
        throw new NotImplementedException();
    }

    private bool patrolToChase()
    {
        throw new NotImplementedException();
    }

    private bool patrolToIdle()
    {
        throw new NotImplementedException();
    }

    
    //void Update()
    //{
        
    //}
}
