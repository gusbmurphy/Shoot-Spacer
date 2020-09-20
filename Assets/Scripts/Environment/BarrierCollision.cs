using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrierCollision : MonoBehaviour
{
    public int damage;
    private void OnTriggerEnter(Collider other)
    {
        var damageable = other.gameObject.GetComponent(typeof(IDamageable));
        if (damageable)
        {
            (damageable as IDamageable).TakeDamage(damage);
        }
    }
}
