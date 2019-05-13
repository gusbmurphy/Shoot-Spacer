using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Enemy : MonoBehaviour
{
    [SerializeField] int hitPoints = 5;
    [SerializeField] float thrustForce = 500f;
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float minimumAnchorSpeed = 0.5f;
    [SerializeField][Tooltip("In radians per second.")] float rotationSpeed = 100f;
    [SerializeField] float aggroRange = 12f;
    [SerializeField] float attackRange = 6f;

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

            if (DistanceToPlayer() >= attackRange) 
            { 
                Thrust(Vector3.forward);
            }
            else
            {
                if (shipRigidbody.velocity.magnitude > minimumAnchorSpeed)
                {
                    Thrust(Vector3.back);
                }
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
}
