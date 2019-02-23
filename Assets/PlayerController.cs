using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] float thrustForce = 100f;
    [SerializeField] float rotationSpeed = 5f;

    Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
    }

    private void CheckForInput()
    {
        if (Input.GetKey(KeyCode.A)) { MoveLeft(); }
        if (Input.GetKey(KeyCode.D)) { MoveRight(); }
        if (Input.GetKey(KeyCode.W)) { MoveForward(); }
        if (Input.GetKey(KeyCode.S)) { MoveBackward(); }

        if (Input.GetAxis("Mouse X") < 0 || Input.GetAxis("Mouse X") > 0) { Rotate(); }
    }

    private void Rotate()
    {
        float yaw = rotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        transform.Rotate(0, yaw, 0);
    }

    private void MoveRight()
    {
        rigidbody.AddRelativeForce(Vector3.right * thrustForce * Time.deltaTime);
    }

    private void MoveLeft()
    {
        rigidbody.AddRelativeForce(Vector3.left * thrustForce * Time.deltaTime);
    }

    private void MoveForward()
    {
        rigidbody.AddRelativeForce(Vector3.forward * thrustForce * Time.deltaTime);
    }

    private void MoveBackward()
    {
        rigidbody.AddRelativeForce(-Vector3.forward * thrustForce * Time.deltaTime);
    }

    private void RotateRight()
    {
        rigidbody.freezeRotation = true;

        float rotationThisFrame = rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up * rotationThisFrame);

        rigidbody.freezeRotation = false;
    }

    private void RotateLeft()
    {
        rigidbody.freezeRotation = true;

        float rotationThisFrame = rotationSpeed * Time.deltaTime;
        transform.Rotate(-Vector3.up * rotationThisFrame);

        rigidbody.freezeRotation = false;
    }
}
