using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public float projectileSpeed = 10f;
    [SerializeField] public int damage = 1;

    void OnTriggerEnter(Collider coll)
    {
        var damageable = coll.gameObject.GetComponent(typeof(IDamageable));
        if (damageable) { (damageable as IDamageable).TakeDamage(damage); }
    }
    // TODO make projectiles go away after a time (so they're not just floating in space forever)
    // TODO make projectiles "explode" on contact, creating visual hit feedback and deleting the projectile
}
