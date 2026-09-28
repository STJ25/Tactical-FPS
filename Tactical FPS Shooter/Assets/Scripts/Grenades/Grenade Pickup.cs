using UnityEngine;

public class GrenadePickup : MonoBehaviour
{
    public GrenadeBehaviour grenade;

    private void Reset()
    {
        grenade = GetComponent<GrenadeBehaviour>();
    }

    public void OnPickup(PlayerInventory inventory)
    {
        // Prevent picking up grenades already in-hand
        if (grenade.IsEquipped)
            return;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        inventory.EquipGrenade(grenade);
    }
}
