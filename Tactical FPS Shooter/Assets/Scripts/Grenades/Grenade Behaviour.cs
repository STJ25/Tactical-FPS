using UnityEngine;
using System.Collections;

public class GrenadeBehaviour : MonoBehaviour
{
    public GrenadeData data;
    public bool IsEquipped = false;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void SetEquipped(bool equipped)
    {
        IsEquipped = equipped;

        rb.isKinematic = equipped;
        rb.detectCollisions = !equipped;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = !equipped;
    }


    public void Throw(Transform throwOrigin)
    {
        transform.SetParent(null);

        rb.isKinematic = false;
        rb.detectCollisions = true;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;

        Vector3 dir = throwOrigin.forward * data.throwForce
                    + throwOrigin.up * data.upwardForce;

        rb.AddForce(dir, ForceMode.Impulse);

        StartCoroutine(ExplodeAfterDelay());
    }


    private IEnumerator ExplodeAfterDelay()
    {
        yield return new WaitForSeconds(data.fuseTime);
        Explode();
    }

    private void Explode()
    {
        // Explosion FX
        if (data.explosionEffect)
            Instantiate(data.explosionEffect, transform.position, Quaternion.identity);

        //explosion sound
        if (data.ExplosionSound != null)
            AudioSource.PlayClipAtPoint(data.ExplosionSound, transform.position);

        // Explosion damage
        Collider[] hits = Physics.OverlapSphere(transform.position, data.explosionRadius);

        foreach (Collider hit in hits)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();
            if (dmg != null)
                dmg.TakeDamage(data.explosionDamage);
            
            // Add explosion force to nearby rigidbodies
            Rigidbody rb = hit.attachedRigidbody;
            if (rb != null)
                rb.AddExplosionForce(data.explosionForce, transform.position, data.explosionRadius,data.explosionUpwardModifier,ForceMode.Impulse);
        }

        Destroy(gameObject);
    }
}
