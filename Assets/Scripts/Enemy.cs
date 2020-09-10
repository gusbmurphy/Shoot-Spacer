using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Enemy : MonoBehaviour, IDamageable
{
    [Header("Behavior")]
    [SerializeField] int hitPoints = 5;
    [SerializeField] float aggroRange = 12f;
    [SerializeField] float attackRange = 6f;

    [Header("Ship Properties")]
    [SerializeField][Tooltip("In radians per second.")] float rotationSpeed = 100f;
    [SerializeField] float thrustForce = 500f;
    [SerializeField] float maxSpeed = 8f;
    [SerializeField] float minimumAnchorSpeed = 0.5f;

    [Header("Weapon Properties")]
    [SerializeField] GameObject attackProjectile;
    [SerializeField] GameObject projectileSocket;
    [SerializeField][Tooltip("Projectiles per minute.")] float fireRate = 90f;

    [Header("Effects")]
    // [SerializeField] ParticleSystem damageEffect = null;

    private ParticleSystem.EmissionModule gunEmission;
    private Rigidbody shipRigidbody;

    bool attacking = false;

    GameObject player; // TODO when the player is destroyed what happens to this?
    GameManager gm;

    public event Action onDeath;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        gm = FindObjectOfType<GameManager>();
        shipRigidbody = GetComponent<Rigidbody>();
    }

    void Update()
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

    void IDamageable.TakeDamage(int damage)
    {
        // var currentEffect = Instantiate(damageEffect, transform.position, Quaternion.identity);
        // Destroy(currentEffect.gameObject, currentEffect.main.duration);

        hitPoints -= damage;

        if (hitPoints < 1)
        {
            gm.EnemyDeath();
            Destroy(this.gameObject);
            // TODO give visual destruction feedback
        }
    }
}