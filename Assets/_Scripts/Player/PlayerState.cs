using UnityEngine;

public class PlayerState : MonoBehaviour
{
    [HideInInspector] public bool IsDead { get; private set; }
    [HideInInspector] public bool IsLockedOn { get; private set; }

    [HideInInspector] public Transform lockedOnTarget;

    [Header("Weapon")]
    public Transform weaponAttachPoint;
    private Weapon weapon;

    public void SetLockOn(Transform target)
    {
        IsLockedOn = true;
        lockedOnTarget = target;
        Debug.Log($"Player state lock on {IsLockedOn}");
    }
    public void ClearLockOn()
    {
        IsLockedOn = false;
        Debug.Log($"Player state lock on {IsLockedOn}");
    }
    public void TryDropWeapon(Vector3 forward)
    {
        if (weapon)
        {
            weapon.Drop(GetComponent<Collider>(), forward);
            weapon = null;
        }
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("Weapon") && weapon == null)
        {
            weapon = hit.gameObject.GetComponent<Weapon>();
            weapon.Equip(GetComponent<Collider>(), weaponAttachPoint);
        }
    }
    public void ReceiveDamage(int dmg)
    {
        GameManager.Instance.PlayerHealth -= dmg;
    }
    public void Die()
    {
        if (IsDead) return;
        IsDead = true;
        GetComponent<CharacterController>().enabled = false;
        Transform cameraTransform = transform.Find("Main Camera");
        if (cameraTransform != null)
        {
            cameraTransform.SetParent(null);
        }
        Destroy(gameObject); 
    }
}
