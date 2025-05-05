using System.Collections;
using UnityEngine;
using static EnemyBaseClass;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Weapon : MonoBehaviour
{
    Rigidbody rb;
    BoxCollider bc;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();
    }

    public void Equip(Collider playerCollider, Transform weaponAttachPoint)
    {
        //Setting our rigidbody to kinematic because we don't want to move via physics anymore
        rb.isKinematic = true;
        //Setting our box collider to be a trigger so we are not blocked by the sword collision
        bc.isTrigger = true;
        transform.SetParent(weaponAttachPoint);
        transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Physics.IgnoreCollision(playerCollider, bc);
    }

    public void Drop(Collider playerCollider, Vector3 playerForward)
    {
        transform.parent = null;
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
    private void OnTriggerEnter(Collider other)
    {

        if (other.CompareTag("Enemy"))
        {
            EnemyBaseClass enemy = other.GetComponentInChildren<EnemyBaseClass>();
            if (enemy != null)
            {
                enemy.TakeDamage(5);
            }
        }
    }
}