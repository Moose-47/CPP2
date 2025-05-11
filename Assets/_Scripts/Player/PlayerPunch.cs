using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class PlayerPunch : MonoBehaviour
{
    Rigidbody rb;
    SphereCollider sc;
    public Animator animator;
    [HideInInspector] public bool isAttacking = false;
    public int punchDmg;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        animator = GetComponentInParent<Animator>();
    }

    public void StartUnarmedAtk()
    {
        if (isAttacking) return;
        isAttacking = true;
        StartCoroutine(AtkHitBox());
        StartCoroutine(AttackDuration());

    }
    IEnumerator AtkHitBox()
    {
        yield return new WaitForSeconds(0.2f);
        sc.enabled = true;
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(0.6f);
        EndAttack();
    }
    private void EndAttack()
    {
        isAttacking = false;
        sc.enabled = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            var enemyContext = other.GetComponent<EnemyStateMachine>();
            if (enemyContext != null)
            {
                var enemy = enemyContext.Context;
                if (enemy != null)
                {
                    sc.enabled = false;
                    enemy.TakeDamage(punchDmg);
                    Debug.LogWarning(enemy.CurrentHealth);
                }
            }
        }
    }
}
