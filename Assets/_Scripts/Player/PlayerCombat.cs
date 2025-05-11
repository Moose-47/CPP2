using System.Collections;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    private PlayerState playerState;
    private PlayerAnimations anim;
    private Weapon weapon;
    private Animator animator;
    private PlayerPunch punch;
    private Coroutine attackCoroutine;

    void Awake()
    {
        playerState = GetComponent<PlayerState>();
        anim = GetComponent<PlayerAnimations>();
        animator = GetComponentInChildren<Animator>();
        punch = GetComponentInChildren<PlayerPunch>();
    }

    public void SetWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
    }
    public void Attack()
    {
        StartAttack();
    }

    private void StartAttack()
    {
        if (!playerState.Equipped && !playerState.isAttacking)
        {
            anim.Punch();
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            playerState.isAttacking = true;
            attackCoroutine = StartCoroutine(EndAttackAfter(1.5f));

            punch.StartUnarmedAtk();

            return;
        }
        if (playerState.Equipped && !playerState.isAttacking)
        {
            anim.EquippedAttack();
            if (attackCoroutine != null) StopCoroutine(attackCoroutine);
            playerState.isAttacking = true;
            attackCoroutine = StartCoroutine(EndAttackAfter(0.75f));

            weapon.StartAttack();

            return;
        }
    }

    private IEnumerator EndAttackAfter(float duration)
    {
        yield return new WaitForSeconds(duration);
        playerState.isAttacking = false;
    }
}
