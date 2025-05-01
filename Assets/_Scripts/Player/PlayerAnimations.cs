using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    private Animator anim;
    private Locomotion locomotion;
    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        locomotion = GetComponentInChildren<Locomotion>();
    }

    public void UpdateAnimationVelocity(Vector3 velocity, bool isGrounded)
    {
        anim.SetFloat("vel", locomotion.smoothedVel);
        anim.SetFloat("jumpVel", locomotion.jumpVel);
    }
}
        