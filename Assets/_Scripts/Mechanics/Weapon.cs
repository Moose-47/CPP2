using System.Collections;
using UnityEngine;


[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Weapon : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;
    public int wepDmg;
    private bool isAttacking = false;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    public void Equip(Collider playerCollider, Transform weaponAttachPoint)
    {
        rb.isKinematic = true;
        bc.isTrigger = true;
        bc.enabled = false;
        transform.SetParent(weaponAttachPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Physics.IgnoreCollision(playerCollider, bc);

        PlayerCombat playerCombat = weaponAttachPoint.root.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.SetWeapon(this);
        }
    }

    public void Drop(Collider playerCollider, Vector3 playerForward)
    {
        transform.parent = null;
        bc.enabled = true;
        rb.isKinematic = false;
        bc.isTrigger = false;
        rb.AddForce(playerForward * 3, ForceMode.Impulse);
        StartCoroutine(DropCooldown(playerCollider));
    }

    IEnumerator DropCooldown(Collider playerCollider)
    {
        yield return new WaitForSeconds(2);

        Physics.IgnoreCollision(playerCollider, bc, false);
    }

    public void StartAttack()
    {
        if (isAttacking) return; //Prevent starting another attack while one is already active

        isAttacking = true;
        //bc.enabled = true;  //Enable the hitbox during the attack
        StartCoroutine(AttackDuration()); //Start the attack duration coroutine
    }
    IEnumerator AttackDuration()
    {
        yield return new WaitForSeconds(0.2f);
        bc.enabled = true;
        yield return new WaitForSeconds(0.75f); // Adjust this duration to match your attack animation time
        EndAttack();
    }
    private void EndAttack()
    {
        isAttacking = false;
        bc.enabled = false; // Disable the hitbox after the attack
    }
    private void OnTriggerEnter(Collider other)
    {
        if (isAttacking && other.CompareTag("Enemy"))
        {
            EnemyBaseClass oldEnemy = other.GetComponentInChildren<EnemyBaseClass>();
            if (oldEnemy != null)
            {
                oldEnemy.TakeDamage(wepDmg);
            }

            var enemyContext = other.GetComponent<EnemyStateMachine>();
            if (enemyContext != null)
            {
                var enemy = enemyContext.Context;
                if (enemy != null)
                {
                    bc.enabled = false;
                    enemy.TakeDamage(wepDmg);
                    Debug.LogWarning(enemy.CurrentHealth);
                }
            }
        }
    }
}