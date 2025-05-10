using UnityEngine;

public class EnemyMeleeHitBox : MonoBehaviour
{

    public class AttackHitbox : MonoBehaviour
    {
        public EnemyContext context;
        public int dmg;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                //Apply damage to the player
                Debug.Log("Hit player with attack!");
                GameManager.Instance.PlayerHealth -= dmg;

                //Disable the hitbox immediately after hitting
                context.attackHitBox.enabled = false;
            }
        }
    }
}
