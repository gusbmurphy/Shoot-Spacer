using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Fighter : Enemy
{
  [SerializeField] float aggroRange = 12f;
  [SerializeField] float attackRange = 6f;

  [Header("Weapon Properties")]
  [SerializeField] GameObject attackProjectile;
  [SerializeField] GameObject projectileSocket;
  [SerializeField] [Tooltip("Projectiles per minute.")] float fireRate = 90f;

  private ParticleSystem.EmissionModule gunEmission;

  bool attacking = false;

  public override void RegularBehavior()
  {
    if (player)
    {
      if (DistanceToPlayer() <= aggroRange)
      {
        RotateTowards(player.transform);

        if (DistanceToPlayer() >= attackRange)
        {
          if (attacking)
          {
            CancelInvoke();
            attacking = false;
          }
          Thrust(Vector3.forward);
        }
        else
        {
          if (DistanceToPlayer() <= attackRange)
          {
            if (!attacking)
            {
              InvokeRepeating("Fire", 0f, 60f / fireRate);
              attacking = true;
            }
          }
          if (shipRigidbody.velocity.magnitude > minimumAnchorSpeed)
          {
            Thrust(Vector3.back);
          }
        }
      }
    }
    else attacking = false;
  }

  private void Fire()
  {
    if (player)
    {
      GameObject projectileObject = Instantiate(attackProjectile, projectileSocket.transform.position, projectileSocket.transform.rotation);
      Projectile projectileComponent = projectileObject.GetComponent<Projectile>();

      Vector3 unitVectorToPlayer = (player.transform.position - projectileSocket.transform.position).normalized;
      projectileObject.GetComponent<Rigidbody>().velocity = unitVectorToPlayer * projectileComponent.projectileSpeed;
    }
  }

  private void RotateTowards(Transform target)
  {
    // TODO how does this work?
    // TODO change this to be physics based? Like with thrusting?
    Vector3 targetDir = target.position - transform.position;

    float step = rotationSpeed * Time.deltaTime;

    Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);

    transform.rotation = Quaternion.LookRotation(newDir);
  }

  private float DistanceToPlayer() { return Vector3.Distance(transform.position, player.transform.position); }

  private void Thrust(Vector3 direction) { shipRigidbody.AddRelativeForce(direction * thrustForce * Time.deltaTime); }
}