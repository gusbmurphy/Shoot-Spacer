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
        if (Input.GetKey(KeyCode.Space)) { ActivateThrust(); }
        if (Input.GetKey(KeyCode.A)) { RotateLeft(); }
        if (Input.GetKey(KeyCode.D)) { RotateRight(); }
    }

    private void ActivateThrust()
    {
        rigidbody.AddRelativeForce(Vector3.forward * thrustForce * Time.deltaTime);
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
