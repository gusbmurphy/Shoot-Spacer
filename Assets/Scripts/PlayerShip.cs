﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]

public class PlayerShip : MonoBehaviour
{
    [Header("Ship Properties")]
    [SerializeField] GameObject ship;
    [SerializeField] ParticleSystem gun;
    [SerializeField] float thrustForce = 100f;
    [SerializeField] float maxSpeed = 10;
    [SerializeField] public float rotationSpeed = 5f;

    private Rigidbody shipRigidbody;
    private ParticleSystem.EmissionModule gunEmission;

    // These vectors correspond to the directions relative to the camera
    private Vector3 cameraForward;
    private Vector3 cameraBack;
    private Vector3 cameraRight;
    private Vector3 cameraLeft;

    void Start()
    {
        shipRigidbody = ship.GetComponent<Rigidbody>();
        gunEmission = gun.emission;

        SetUpCameraDirections();
    }

    private void SetUpCameraDirections()
    {
        GameObject cameraCompass = new GameObject("Camera Compass");
        // "Correct" the compass so that it lies flat in the game plane
        cameraCompass.transform.eulerAngles = new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0);

        cameraForward = cameraCompass.transform.forward;
        cameraBack = -cameraCompass.transform.forward;
        cameraRight = cameraCompass.transform.right;
        cameraLeft = -cameraCompass.transform.right;

        Destroy(cameraCompass); // We don't need the Compass anymore!
    }

    void Update()
    {
        CheckForInput();
        ClampSpeed();
    }

    private void CheckForInput()
    {
        if (Input.GetButtonDown("Fire1")) { Fire(); }

        if (Input.GetKey(KeyCode.A)) { Thrust(cameraLeft); }
        if (Input.GetKey(KeyCode.D)) { Thrust(cameraRight); }
        if (Input.GetKey(KeyCode.W)) { Thrust(cameraForward); }
        if (Input.GetKey(KeyCode.S)) { Thrust(cameraBack); }
    }

    private void Fire()
    {
        gun.Emit(1);
    }

    public void Thrust(Vector3 direction)
    {
        shipRigidbody.AddForce(direction * thrustForce * Time.deltaTime);
    }

    private void ClampSpeed()
    {
        if (shipRigidbody.velocity.magnitude > maxSpeed)
        {
            shipRigidbody.velocity = Vector3.ClampMagnitude(shipRigidbody.velocity, maxSpeed);
        }
    }
}