using UnityEngine;

public class EnemyMeleeHitBox : MonoBehaviour
{
    public int dmg;
    public SphereCollider sphereCollider;

    private void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Apply damage to the player
            GameManager.Instance.PlayerHealth -= dmg;

            //Disable the hitbox immediately after hitting
            sphereCollider.enabled = false;
        }
    }    
}
