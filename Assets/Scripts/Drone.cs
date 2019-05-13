using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

// TODO shouldn't Drone just be a type of enemy? How do I make "subclasses"?

public class Drone : MonoBehaviour
{
    [SerializeField] int hitPoints = 1;
    [SerializeField] float thrustForce = 500f;
    [SerializeField] float maxSpeed = 12f;
    [SerializeField] float minimumAnchorSpeed = 0.5f;
    [SerializeField] [Tooltip("In radians per second.")] float rotationSpeed = 100f;
    [SerializeField] float aggroRange = 18f;

    ParticleSystem gun;
    private Rigidbody shipRigidbody;

    GameObject player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        shipRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (DistanceToPlayer() <= aggroRange)
        {
            RotateTowards(player.transform);
            Thrust(Vector3.forward);
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

