using UnityEngine;

public abstract class EnemyBaseClass : MonoBehaviour
{
    [Header("Enemy Stats")]
    public int maxHealth;
    public int currentHealth;
    public float moveSpeed;

    [Header("Combat Settings")]
    public int meleeDmg;
    public int rangedDmg;
    public float atkRange;
    public float followRange;

    protected bool isDead = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Awake()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (isDead) return;

        HandleMovement();
        HandleAttack();
    }

    protected abstract void HandleMovement();
    protected abstract void HandleAttack();
    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject, 5f);
        Destroy(transform.parent?.gameObject, 5f);
    }
    public virtual void TakeDamage(int DamageValue, DamageType damageType = DamageType.Default)
    {
        if (isDead) return;
        Debug.Log($"Damage Received: {DamageValue} | Current Health: {currentHealth}");

        switch (damageType)
        {
            case DamageType.Melee:
                DamageValue -= 2; 
                break;

            case DamageType.Ranged:
                DamageValue -= 1;
                break;
        }

        currentHealth -= DamageValue;
        Debug.Log($"After Damage: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public enum DamageType
    {
        Default,
        Melee,
        Ranged
    }
}
