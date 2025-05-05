using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator anim;
    private Locomotion locomotion;
    private PlayerState playerState;
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        locomotion = GetComponentInChildren<Locomotion>();
        playerState = GetComponentInChildren<PlayerState>();
    }

    public void UpdateAnimationVelocity(Vector3 velocity, bool isGrounded)
    {
        #region Locomotion Animations
        anim.SetBool("isGrounded", locomotion.IsGrounded());
        if (playerState.Equipped) anim.SetBool("Equipped", true);
            else anim.SetBool("Equipped", false);

        if (playerState.IsLockedOn)
        {
            anim.SetBool("LockedOn", true);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S))
            {
                if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    anim.SetFloat("vel", locomotion.smoothedVel + 1f);
                else
                    anim.SetFloat("vel", locomotion.smoothedVel);
            }
            else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
            {
                anim.SetFloat("vel", locomotion.smoothedVel);
            }
            else anim.SetFloat("vel", locomotion.smoothedVel);
        }
        else anim.SetBool("LockedOn", false);
        if (!playerState.IsLockedOn)
        {
            anim.SetFloat("vel", locomotion.smoothedVel);
        }
        anim.SetFloat("jumpVel", locomotion.jumpVel);
        #endregion
    }
    public void Punch()
    {
        anim.SetTrigger("Attack");
    }
    public void EquippedAttack()
    {
        anim.SetTrigger("Attack");
    }
    public void deathAnimation()
    {
        anim.SetTrigger("death");
    }
}
        