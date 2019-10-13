using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] GameObject attackProjectile;
    [SerializeField] GameObject projectileSocket;
    [SerializeField] [Tooltip("Projectiles per minute.")] int fireRate = 500;
    // Start is called before the first frame update
    void Update()
    {
        // TODO is there a better way to get this firing done than InvokeRepeating? Same goes for the enemies
        if (Input.GetButtonDown("Fire1")) InvokeRepeating("Fire", 0f, 60f / fireRate); 
        else if (Input.GetButtonUp("Fire1")) CancelInvoke();
    }

    private void Fire()
    {
        GameObject projectileObject = Instantiate(attackProjectile, projectileSocket.transform.position, projectileSocket.transform.rotation);
        Projectile projectileComponent = projectileObject.GetComponent<Projectile>();

        Vector3 unitDirectionVector = (projectileSocket.transform.position - transform.position).normalized;
        projectileObject.GetComponent<Rigidbody>().velocity = unitDirectionVector * projectileComponent.projectileSpeed;
    }
}
