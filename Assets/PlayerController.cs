using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class PlayerController : MonoBehaviour
{
    [Header("Ship Properties")]
    [SerializeField] GameObject ship;
    [SerializeField] ParticleSystem gun;
    [SerializeField] float thrustForce = 100f;
    [SerializeField] float maxSpeed = 10;
    [SerializeField] float rotationSpeed = 5f;

    // todo implement "delayed" camera following
    //[Header("Camera Properties")]
    //[SerializeField] Camera mainCamera;
    //[SerializeField] float cameraSpeed = 1f;

    //Vector3 cameraShipDifference;
    Rigidbody shipRigidbody;
    ParticleSystem.EmissionModule gunEmission;

    void Start()
    {
        shipRigidbody = ship.GetComponent<Rigidbody>();
        gunEmission = gun.emission;
        //cameraShipDifference = ship.transform.position - mainCamera.transform.position;
    }

    void Update()
    {
        CheckForInput();
        ClampSpeed();
        //MakeCameraFollow();
    }

    //private void MakeCameraFollow()
    //{
    //    float step = cameraSpeed * Time.deltaTime;
    //    Vector3 cameraTargetPos = ship.transform.position - cameraShipDifference;
    //    mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraTargetPos, step);
    //}

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
        if (Input.GetKey(KeyCode.S)) { Move(-Vector3.forward); }

        if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0) { Rotate(); }
    }

    private void Fire()
    {
        gun.Emit(1);
    }

    private void Rotate()
    {
        float yaw = rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
    }

    private void Move(Vector3 direction)
    {
        shipRigidbody.AddRelativeForce(direction * thrustForce * Time.deltaTime);
    }
}
