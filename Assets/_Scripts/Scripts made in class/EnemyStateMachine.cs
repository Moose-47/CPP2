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
    public IState<EnemyContext> deathState;

    public void InitializeEnemy(EnemyContext ctx)
    {
        context = ctx;

        AssignAttackHitBox();

        StateBuilder<EnemyContext> stateBuilder = new StateBuilder<EnemyContext>();

        idleState = stateBuilder
            .SetName("idle")
            .OnEnter(() =>
            {

                Debug.Log("Entering Idle");
                context.anim.SetBool("idle", true);
                context.anim.SetBool("walk", false);
                context.agent.velocity = Vector3.zero;
                context.agent.isStopped = true;
            })

            .Build();

        patrolState = stateBuilder
            .SetName("patrol")
            .OnEnter(() => 
            {
                Debug.Log("Entering Patrol");
                context.anim.SetBool("idle", false);
                context.anim.SetBool("walk", true);
                context.agent.isStopped = false;
            })
            .OnUpdate(OnPatrolUpdate)
            .Build();

        chaseState = stateBuilder
            .SetName("chase")
            .OnEnter(() => 
            {
                Debug.Log("Entering Chase");               
                context.anim.SetBool("idle", false);
                context.anim.SetBool("walk", true);
                context.agent.isStopped = false;
            })
            .OnUpdate(OnChaseUpdate)
            .Build();

        atkState = stateBuilder
            .SetName("attack")
            .OnEnter(() =>
            {
                Debug.Log("Entering Attack");
                context.anim.SetBool("idle", false);
                context.anim.SetBool("walk", false);
                context.anim.SetTrigger("atk");

                if (context.attackHitBox != null)
                {
                    context.attackHitBox.enabled = true;
                }
            })
            .OnUpdate(OnAttackUpdate)
            .OnExit(() =>
            {
                if (context.attackHitBox != null)
                {
                    context.attackHitBox.enabled = false;
                }
            })
            .Build();

        deathState = stateBuilder
            .SetName("death")
            .OnEnter(() =>
            {
                Debug.Log("Enemy Died");
                context.anim.SetBool("idle", false);
                context.anim.SetBool("walk", false);
                context.anim.SetTrigger("die");

                context.agent.isStopped = true;
                context.agent.velocity = Vector3.zero;
                if (context.attackHitBox != null)
                {
                    context.attackHitBox.enabled = false;
                }
                Destroy(gameObject, 5f);
            })
            .Build();

        RegisterState("idle", idleState);
        RegisterState("patrol", patrolState);
        RegisterState("chase", chaseState);
        RegisterState("attack", atkState);
        RegisterState("death", deathState);

        // Transitions
        idleState.AddTransition(this)
            .To(patrolState)
            .When(() => IsPlayerTooFar() && context.PathIndex > 0)
            .Build();

        idleState.AddTransition(this)
            .To(chaseState)
            .When(CanSeePlayer)
            .Build();

        patrolState.AddTransition(this)
            .To(chaseState)
            .When(CanSeePlayer)
            .Build();

        chaseState.AddTransition(this)
            .To(idleState)
            .When(() => IsPlayerTooFar() && context.PathIndex == 0)
            .Build();

        chaseState.AddTransition(this)
            .To(patrolState)
            .When(() => IsPlayerTooFar() && context.PathIndex > 0)
            .Build();

        chaseState.AddTransition(this)
            .To(atkState)
            .When(() => Vector3.Distance(context.Player.position, context.agent.transform.position) < context.attackRange)
            .Build();

        chaseState.AddTransition(this)
            .To(deathState)
            .When(() => context.IsDead)
            .Build();

        atkState.AddTransition(this)
            .To(deathState)
            .When(() => context.IsDead)
            .Build();

        deathState.AddTransition(this)
            .To(deathState)
            .When(() => context.IsDead)
            .Build();

        ChangeState(idleState);

    }
    public bool CanSeePlayer()
    {
        if (context.Player == null)
        {
            Debug.Log("player not found for canseeplayer");
            return false;
        }
        float dist = Vector3.Distance(context.Player.position, context.agent.nextPosition);
        if (dist < context.followRange) 
            return true;

        else return false;
    }
    private bool IsPlayerTooFar()
    {
        float dist = Vector3.Distance(context.Player.position, context.agent.nextPosition);
        if (dist > context.followRange)
            return true;
        else return false;
    }
    private bool IsPlayerInAttackRange()
    {
        return Vector3.Distance(context.Player.position, context.agent.nextPosition) < context.attackRange;
    }
    private void OnPatrolUpdate()
    {
        if (context.Path == null || context.Path.Length == 0) return;

        var target = context.Path[context.PathIndex];
        context.agent.SetDestination(target.position);

        if (context.agent.remainingDistance < 0.2f)
        {
            context.PathIndex = (context.PathIndex + 1) % context.Path.Length;
        }
    }

    private void OnChaseUpdate()
    {
        context.agent.SetDestination(context.Player.position);
    }

    private void OnAttackUpdate()
    {
        context.agent.SetDestination(context.agent.transform.position);

        Debug.Log("Attacking player...");
    }
    private void AssignAttackHitBox()
    {
        Collider[] colliders = context.agent.GetComponentsInChildren<Collider>();
        foreach (var col in colliders)
        {
            if (col.CompareTag("enemy atk"))
            {
                context.attackHitBox = col;
                return;
            }
        }

        Debug.LogWarning("No attack hitbox found with tag 'enemy atk'.");
    }
    public void StateMachineUpdate()
    {
        base.Update();
    }
}
