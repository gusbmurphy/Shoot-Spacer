using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerShip : MonoBehaviour
{
    [Header("Ship Properties")]
    [SerializeField] GameObject ship;
    [SerializeField] ParticleSystem gun;
    [SerializeField] float thrustForce = 100f;
    [SerializeField] float maxSpeed = 10;
    [SerializeField] public float rotationSpeed = 5f;

    [Header("Technical")]
    [SerializeField] Transform intendedFacingTransform;

    private Rigidbody shipRigidbody;
    private ParticleSystem.EmissionModule gunEmission;
    private Vector3 intendedFacingVector;

    void Start()
    {
        shipRigidbody = ship.GetComponent<Rigidbody>();
        gunEmission = gun.emission;
    }

    void Update()
    {
        CheckForInput();
        ClampSpeed();
    }

    private void ClampSpeed()
    {
        if (shipRigidbody.velocity.magnitude > maxSpeed)
        {
            shipRigidbody.velocity = Vector3.ClampMagnitude(shipRigidbody.velocity, maxSpeed);
        }
    }

    private void CheckForInput()
    {
        if (Input.GetButtonDown("Fire1")) { Fire(); }

        if (Input.GetKey(KeyCode.A)) { Move(Vector3.left); }
        if (Input.GetKey(KeyCode.D)) { Move(Vector3.right); }
        if (Input.GetKey(KeyCode.W)) { Move(Vector3.forward); }
        if (Input.GetKey(KeyCode.S)) { Move(Vector3.back); }
    }

    private void Fire()
    {
        gun.Emit(1);
    }

    public void Move(Vector3 direction)
    {
        shipRigidbody.AddForce(direction * thrustForce * Time.deltaTime);
    }
}
