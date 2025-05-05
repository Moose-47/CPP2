using System.Collections;
using UnityEngine;

public class MutantEnemy : EnemyBaseClass
{
    [Header("Attack Settings")]
    public float attackCooldown = 2f;
    private float lastAttackTime;
    public GameObject attackhitBox;

    private Transform player;
    private Animator anim;
    private Rigidbody rb;
    public LayerMask groundLayerMask;
    private bool isAttacking = false;
    public float gravityForce = 9.81f;
    #region player spawning
    private void OnEnable()
    {
        GameManager.Instance.OnPlayerSpawned += OnPlayerSpawnedCallback;
    }
    private void OnDisable()
    {
        GameManager.Instance.OnPlayerSpawned -= OnPlayerSpawnedCallback;
    }
    private void OnPlayerSpawnedCallback(Player playerObj)
    {
        player = playerObj.transform;
    }
    #endregion
    protected override void Awake()
    {
        base.Awake();

        if (player == null)
        {
            player = GameObject.FindWithTag("Player")?.transform;
        }
        if (player == null) Debug.Log(player);

        rb = GetComponentInParent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    protected override void HandleMovement()
    {
        if (player == null || anim.GetCurrentAnimatorStateInfo(0).IsName("E atk")) return;

        if (!IsGrounded()) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer > followRange)
        {
            anim.SetBool("isWalking", false);  // Stop walking if player is out of range
            return;
        }

        Vector3 direction = (player.position - transform.position).normalized;
        direction.y = 0;  // Keep the movement direction flat (no up/down movement)

        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 2f * Time.deltaTime);
        }
        if (distanceToPlayer < atkRange && isAttacking && !anim.GetCurrentAnimatorStateInfo(0).IsName("E atk"))
        {
            anim.SetBool("idle", true);
            anim.SetBool("isWalking", false);
        }
        if (distanceToPlayer > atkRange)
        {
            anim.SetBool("isWalking", true);
            anim.SetBool("idle", false);
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);  // Move enemy

            if (!IsGrounded())
            {
                rb.AddForce(Vector3.down * gravityForce, ForceMode.Acceleration);  // Simulate gravity
            }
        }
        else
        {
            anim.SetBool("isWalking", false);  // Stop walking when in attack range
        }
    }

    protected override void HandleAttack()
    {
        if (player == null || isAttacking || !IsGrounded()) return; 

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= atkRange && Time.time >= lastAttackTime + attackCooldown)
        {
            isAttacking = true; 
            StartCoroutine(AttackCoroutine()); 
        }
    }
    bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f, groundLayerMask))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);
            if (angle < 30)  //slope angle
            {
                return true;
            }
        }
        return false;
    }
    private IEnumerator AttackCoroutine()
    {
        anim.SetTrigger("attack");
        lastAttackTime = Time.time;

        yield return new WaitForSeconds(0.2f);
        attackhitBox.SetActive(true);

        yield return new WaitForSeconds(0.5f);
        attackhitBox.SetActive(false);

        yield return new WaitForSeconds(6f);

        isAttacking = false;
    }

    protected override void Die()
    {
        if (isDead) return;

        isDead = true;

        anim.SetTrigger("die");

        //Collider col = GetComponentInParent<CapsuleCollider>();
        //if (col != null) col.enabled = false;

        StopAllCoroutines();

        base.Die();
    }
}
