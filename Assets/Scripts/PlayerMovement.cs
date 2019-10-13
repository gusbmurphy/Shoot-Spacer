using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class PlayerMovement : MonoBehaviour
{
    private Rigidbody shipRigidbody;

    [SerializeField] int thrustForce = 2500;
    [SerializeField] float maxNormalSpeed = 10f;
    private float currentMaxSpeed;
    [SerializeField] public float rotationSpeed = 1f;

    // Start is called before the first frame update
    void Start()
    {
        shipRigidbody = GetComponent<Rigidbody>();
        SetUpCameraDirections();
        currentMaxSpeed = maxNormalSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CheckForInput();
        ClampSpeed();
    }

    // These vectors correspond to the directions relative to the camera
    private Vector3 cameraForward;
    private Vector3 cameraBack;
    private Vector3 cameraRight;
    private Vector3 cameraLeft;

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

    private void CheckForInput()
    {
        CheckForThrust();
        //foreach (KeyCode key in movementKeyCodes) if (Input.GetKeyDown(key)) CheckForDash(key);
    }

    private void CheckForThrust()
    {
        if (Input.GetKey(KeyCode.A))
        {
            Thrust(cameraLeft);
        }
        if (Input.GetKey(KeyCode.D))
        {
            Thrust(cameraRight);
        }
        if (Input.GetKey(KeyCode.W))
        {
            Thrust(cameraForward);
        }
        if (Input.GetKey(KeyCode.S))
        {
            Thrust(cameraBack);
        }
    }

    public void Thrust(Vector3 direction)
    {
        shipRigidbody.AddForce(direction * thrustForce * Time.deltaTime);
    }

    public void Thrust(Vector3 direction, float force)
    {
        print("Dashing.");
        shipRigidbody.AddForce(direction * force);
    }

    private void ClampSpeed()
    {
        if (shipRigidbody.velocity.magnitude > maxNormalSpeed)
        {
            shipRigidbody.velocity = Vector3.ClampMagnitude(shipRigidbody.velocity, maxNormalSpeed);
        }
    }

    //private KeyCode dashKey = KeyCode.F; // The 'F' KeyCode is used to represent an empty dashKey
    //private DateTime dashKeyPressTime;
    //private DateTime lastDashTime = new DateTime(1991, 12, 14); // Just an arbitrary DateTime to start with

    //private void CheckForDash(KeyCode key)
    //{
    //    if (key != dashKey)
    //    {
    //        dashKey = key;
    //        dashKeyPressTime = DateTime.Now;
    //    }
    //    else if (CheckDashTimes()) Dash(key);
    //}

    //private bool CheckDashTimes()
    //{
    //    return DateTime.Now.Subtract(dashKeyPressTime).TotalMilliseconds < dashThreshhold
    //            && DateTime.Now.Subtract(lastDashTime).TotalMilliseconds > dashCooldown;
    //}

    //private void Dash(KeyCode key)
    //{
    //    switch(key)
    //    {
    //        case KeyCode.W:
    //            Thrust(cameraForward, thrustForce * dashForceMultiplier);
    //            break;
    //        case KeyCode.A:
    //            Thrust(cameraLeft, thrustForce * dashForceMultiplier);
    //            break;
    //        case KeyCode.S:
    //            Thrust(cameraBack, thrustForce * dashForceMultiplier);
    //            break;
    //        case KeyCode.D:
    //            Thrust(cameraRight, thrustForce * dashForceMultiplier);
    //            break;
    //        default:
    //            throw new Exception("Invalid KeyCode passed to Dash().");
    //    }

    //    lastDashTime = DateTime.Now;
    //    dashKey = KeyCode.F;
    //}

    //private IEnumerator OpenSpeedClampForDash()
    //{
    //    currentMaxSpeed = maxDashSpeed;
    //    yield return new WaitForSecondsRealtime(dashDuration / 1000f);
    //    currentMaxSpeed = maxNormalSpeed;
    //}
}
