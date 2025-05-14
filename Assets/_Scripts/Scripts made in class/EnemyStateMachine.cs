using UnityEngine;
using StateMachine;
using UnityEngine.AI;
using StateMachine.Editor;
using System;
using System.Collections;

public class EnemyStateMachine : StateMachine<EnemyContext>
{
    public IState<EnemyContext> idleState;
    public IState<EnemyContext> patrolState;
    public IState<EnemyContext> chaseState;
    public IState<EnemyContext> atkState;
    public IState<EnemyContext> deathState;

    [SerializeField] private String defaultState;
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
                context.anim.SetBool("atk", false);
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
                context.anim.SetBool("atk", false);
                context.agent.isStopped = false;
            })
            .OnUpdate(OnChaseUpdate)
            .Build();

        atkState = stateBuilder
            .SetName("attack")
            .OnEnter(() =>
            {
                context.agent.isStopped = true;
                context.agent.velocity = Vector3.zero;
                Debug.Log("Entering Attack");
                context.anim.SetBool("idle", false);
                context.anim.SetBool("walk", false);
                context.anim.SetBool("atk", true);
            })
            .OnUpdate(OnAttackUpdate)
            .OnExit(() =>
            {
                context.agent.isStopped = false;

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
                context.anim.SetBool("idle", false);
                context.anim.SetBool("walk", false);
                context.anim.SetBool("atk", false);
                context.anim.SetTrigger("die");

                context.agent.isStopped = true;
                context.agent.velocity = Vector3.zero;

                context.cc.enabled = false;

                if (context.attackHitBox != null)
                {
                    context.attackHitBox.enabled = false;
                }
                StartCoroutine(DeathItemSpawn(4.9f));
                Destroy(gameObject, 5f);
            })
            .Build();

        RegisterState("idle", idleState);
        RegisterState("patrol", patrolState);
        RegisterState("chase", chaseState);
        RegisterState("attack", atkState);
        RegisterState("death", deathState);

        // Transitions
        #region Idle to-
        idleState.AddTransition(this)
            .To(patrolState)
            .When(() => IsPlayerTooFar() && context.Path.Length > 0)
            .Build();

        idleState.AddTransition(this)
            .To(chaseState)
            .When(() => CanSeePlayer() && !IsPlayerInAttackRange())
            .Build();

        idleState.AddTransition(this)
            .To(atkState)
            .When(() => 
                IsPlayerInAttackRange() 
                && !context.attackHitBox.enabled 
                && !context.anim.GetCurrentAnimatorStateInfo(0).IsName("E atk"))
            .Build();

        idleState.AddTransition(this)
            .To(deathState)
            .When(() => context.IsDead)
            .Build();
        #endregion

        #region Patrol to-
        patrolState.AddTransition(this)
            .To(chaseState)
            .When(CanSeePlayer)
            .Build();

        patrolState.AddTransition(this)
            .To(idleState)
            .When(() => context.Path.Length == 0)
            .Build();

        patrolState.AddTransition(this)
            .To(deathState)
            .When(() => context.IsDead)
            .Build();
        #endregion

        #region Chase to-
        chaseState.AddTransition(this)
            .To(idleState)
            .When(() => IsPlayerTooFar() && context.Path.Length == 0)
            .Build();

        chaseState.AddTransition(this)
            .To(patrolState)
            .When(() => IsPlayerTooFar() && context.Path.Length > 0)
            .Build();

        chaseState.AddTransition(this)
            .To(atkState)
            .When(() => IsPlayerInAttackRange() && !context.anim.GetCurrentAnimatorStateInfo(0).IsName("E atk"))
            .Build();

        chaseState.AddTransition(this)
            .To(deathState)
            .When(() => context.IsDead)
            .Build();
        #endregion

        #region Attack to-
        atkState.AddTransition(this)
            .To(deathState)
            .When(() => context.IsDead)
            .Build();

        atkState.AddTransition(this)
            .To(idleState)
            .When(() => 
                IsPlayerInAttackRange() 
                && context.anim.GetCurrentAnimatorStateInfo(0).IsName("E atk")
                && context.anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
            .Build();

        atkState.AddTransition(this)
            .To(chaseState)
            .When(() => 
                CanSeePlayer() && !IsPlayerInAttackRange() 
                && !context.anim.GetCurrentAnimatorStateInfo(0).IsName("E atk"))
            .Build();

        atkState.AddTransition(this) //This transition is to prevent state bricking
            .To(idleState)
            .When(() => 
                !IsPlayerInAttackRange() 
                && context.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            .Build();
        #endregion

        #region State to when player dead
        chaseState.AddTransition(this)
            .To(idleState)
            .When(() => context._player != null && context._player.isDead)
            .Build();

        atkState.AddTransition(this)
            .To(idleState)
            .When(() => context._player != null && context._player.isDead)
            .Build();
        #endregion

        ChangeState(defaultState); //Default state on start
    }
    public bool CanSeePlayer()
    {
        if (context.Player == null || context._player == null || context._player.isDead)
        {
            return false;
        }
        float dist = Vector3.Distance(context.Player.position, context.agent.nextPosition);
        if (dist < context.followRange) 
            return true;

        else return false;
    }
    private bool IsPlayerTooFar()
    {
        if (context.Player == null || context._player == null || context._player.isDead)
        {
            return true;
        }
            float dist = Vector3.Distance(context.Player.position, context.agent.nextPosition);
        if (dist > context.followRange)
            return true;
        else return false;
    }
    private bool IsPlayerInAttackRange()
    {
        if (context.Player == null || context._player == null || context._player.isDead)
        {
            return false;
        }

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

        float time = context.anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
        float target = context.atkHitBoxTimer;
        if (context.anim.GetCurrentAnimatorStateInfo(0).IsName("E atk") && time >= target && time < target + 0.02f)
            context.attackHitBox.enabled = true;
        if (context.anim.GetCurrentAnimatorStateInfo(0).IsName("E atk") && time > target * 2f)
            context.attackHitBox.enabled = false;
    }
    private void AssignAttackHitBox()
    {
        SphereCollider[] colliders = context.agent.GetComponentsInChildren<SphereCollider>();
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
    private IEnumerator DeathItemSpawn(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (context.deathEffectPrefab != null)
            Instantiate(context.deathEffectPrefab, transform.position, Quaternion.identity);
    }
    public void StateMachineUpdate()
    {
        base.Update();
    }
}
