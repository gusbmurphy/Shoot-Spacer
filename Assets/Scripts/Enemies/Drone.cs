using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

// TODO shouldn't Drone just be a type of enemy? How do I make "subclasses"?

public class Drone : Enemy
{
  [Header("Attack Properties")]
  [SerializeField] float aggroRange = 18f;
  [SerializeField] int selfDestructDamage = 5;

  public override void RegularBehavior()
  {
    if (player)
    {
      if (DistanceToPlayer() <= aggroRange)
      {
        RotateTowards(player.transform);
        Thrust(Vector3.forward);
      }
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

  private void Thrust(Vector3 direction)
  {
    shipRigidbody.AddRelativeForce(direction * thrustForce * Time.deltaTime);
  }

  private void OnParticleCollision(GameObject other)
  {
    hitPoints--;
    print(gameObject.name + " was hit down to " + hitPoints + " hitpoints.");
    // TODO give visual hit feedback
    if (hitPoints < 1)
    {
      print(gameObject.name + " was destroyed.");
      Destroy(this.gameObject);
      // TODO give visual destruction feedback
    }
  }

  void OnTriggerEnter(Collider coll)
  {
    var damageable = coll.gameObject.GetComponent(typeof(IDamageable));
    if (damageable)
    {
      (damageable as IDamageable).TakeDamage(selfDestructDamage);
      // var currentEffect = Instantiate(hitEffect, transform.position, Quaternion.identity);
      // Destroy(currentEffect.gameObject, currentEffect.main.duration);
      Destroy(this.gameObject);
    }
  }

  private void OnCollisionEnter(Collision collision)
  {
    if (collision.gameObject == GameObject.FindGameObjectWithTag("Player"))
    {
      SelfDestruct();
    }
  }

  private void SelfDestruct()
  {
    // TODO do damage to the player
    Destroy(this.gameObject);
  }
}

