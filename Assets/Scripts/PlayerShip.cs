using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Rigidbody))]

public class PlayerShip : MonoBehaviour, IDamageable
{
    [Header("Ship")]
    [SerializeField] int hitPoints = 10;

    [Header("Movement")]
    [SerializeField] float thrustForce = 100f;
    [SerializeField] float maxNormalSpeed = 10f;
    private float currentMaxSpeed;
    [SerializeField] public float rotationSpeed = 5f;

    //[Header("Dash")]
    //[SerializeField] [Tooltip("Multiple of thrust force applied for dash.")] float dashForceMultiplier = 5f;
    //[SerializeField] [Tooltip("In milliseconds.")] int dashDuration = 500;
    //[SerializeField] float maxDashSpeed = 50f;
    //[SerializeField] [Tooltip("In milliseconds.")] int dashCooldown = 3000;
    //[SerializeField] [Tooltip("Maximum milliseconds elapsed between consecutive key presses to dash.")] int dashThreshhold = 1000;

    [Header("Weapon")]
    [SerializeField] GameObject attackProjectile;
    [SerializeField] GameObject projectileSocket;
    [SerializeField] [Tooltip("Projectiles per minute.")] float fireRate = 90f;

    private Rigidbody shipRigidbody;
    private ParticleSystem.EmissionModule gunEmission;

    // These vectors correspond to the directions relative to the camera
    private Vector3 cameraForward;
    private Vector3 cameraBack;
    private Vector3 cameraRight;
    private Vector3 cameraLeft;

    private KeyCode[] movementKeyCodes = { KeyCode.W, KeyCode.A, KeyCode.S, KeyCode.D };

    void Start()
    {
        shipRigidbody = GetComponent<Rigidbody>();
        SetUpCameraDirections();
        currentMaxSpeed = maxNormalSpeed;
    }

    // SetUpCameraDirections() does the work of finding the camera and ensuring that the player controls move relative to the camera's perspective
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
        // TODO is there a better way to get this firing done than InvokeRepeating? Same goes for the enemies
        if (Input.GetButtonDown("Fire1")) InvokeRepeating("Fire", 0f, 60f / fireRate);
        else if (Input.GetButtonUp("Fire1")) CancelInvoke();

        CheckForThrust();

        //foreach (KeyCode key in movementKeyCodes) if (Input.GetKeyDown(key)) CheckForDash(key);
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

    private void Fire()
    {
        GameObject projectileObject = Instantiate(attackProjectile, projectileSocket.transform.position, projectileSocket.transform.rotation);
        Projectile projectileComponent = projectileObject.GetComponent<Projectile>();

        Vector3 unitDirectionVector = (projectileSocket.transform.position - transform.position).normalized;
        projectileObject.GetComponent<Rigidbody>().velocity = unitDirectionVector * projectileComponent.projectileSpeed;
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

    void IDamageable.TakeDamage(int damage)
    {
        hitPoints--;
        print(gameObject.name + " was hit down to " + hitPoints + " hitpoints.");
        // TODO give visual hit feedback
        if (hitPoints < 1)
        {
            print(gameObject.name + " was destroyed.");
            Destroy(this.gameObject);
            // TODO give visual destruction feedback
            // TODO make a game end state
        }
    }
}
