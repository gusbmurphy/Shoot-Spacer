using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Enemy : MonoBehaviour, IDamageable
{
  [Header("Behavior")]
  public int hitPoints = 5;

  [Header("Ship Properties")]
  [Tooltip("In radians per second.")] public float rotationSpeed = 100f;
  public float thrustForce = 500f;
  public float maxSpeed = 8f;
  public float minimumAnchorSpeed = 0.5f;
  protected Rigidbody shipRigidbody;
  // bool attacking = false;

  protected GameObject player; // TODO when the player is destroyed what happens to this?
  protected GameManager gm;

  public event Action OnDeath;

  void Start()
  {
    player = GameObject.FindGameObjectWithTag("Player");
    gm = FindObjectOfType<GameManager>();
    shipRigidbody = GetComponent<Rigidbody>();
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

  void IDamageable.TakeDamage(int damage)
  {
    hitPoints -= damage;

    if (hitPoints < 1)
    {
      if (OnDeath != null) OnDeath();
      Destroy(this.gameObject);
      // TODO give visual destruction feedback
    }
  }
}