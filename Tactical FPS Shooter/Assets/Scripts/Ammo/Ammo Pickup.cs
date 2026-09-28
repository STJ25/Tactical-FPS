using UnityEngine;

public class AmmoPickup : MonoBehaviour
{
    public int ammoAmount = 30; // amount added to reserve
    //public AudioClip pickupSound;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        PlayerInventory inventory = other.GetComponent<PlayerInventory>();
        if (inventory == null) return;

        inventory.AddAmmo(ammoAmount);

        //// play pickup sound
        //if (pickupSound)
        //{
        //    AudioSource.PlayClipAtPoint(pickupSound, transform.position);
        //}

        Destroy(gameObject);
    }
}
