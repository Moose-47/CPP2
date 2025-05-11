using UnityEngine;

public class healthPickup : MonoBehaviour, Ipickups
{
    public int healAmt;
    private bool used = false;
    public void Pickup(Player player)
    {
        if (used) return;

        GameManager.Instance.PlayerHealth += healAmt;
        used = true;    
        Debug.Log(GameManager.Instance.PlayerHealth);
        Destroy(gameObject);
    }
}
