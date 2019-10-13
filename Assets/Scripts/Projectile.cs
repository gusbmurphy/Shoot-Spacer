using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] public float projectileSpeed = 10f;
    [SerializeField] public int damage = 1;
    [SerializeField] public float lifeTime = 5f;
    [SerializeField] ParticleSystem hitEffect;

    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(lifeTime);
        Destroy(this.gameObject);
    }

    void OnTriggerEnter(Collider coll)
    {
        var damageable = coll.gameObject.GetComponent(typeof(IDamageable));
        if (damageable)
        {
            (damageable as IDamageable).TakeDamage(damage);
            var currentEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
            Destroy(currentEffect.gameObject, currentEffect.main.duration);
            Destroy(this.gameObject);
        }
    }
    // TODO make projectiles "explode" on contact, creating visual hit feedback and deleting the projectile
}
