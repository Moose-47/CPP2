using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private Animator anim;
    private PlayerState playerState;

    public bool isAttacking = false;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        playerState = GetComponentInChildren<PlayerState>();
    }

    public void Attack()
    {
        if (!playerState.IsLockedOn || playerState.IsDead) return;

        isAttacking = true;
    }
}
