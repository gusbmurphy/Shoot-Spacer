using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO the player is able to move the reticle off of the screen through the bottom and into the vertical plane

public class Reticle : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 10f;
    [SerializeField][Tooltip("Graphic for player intended facing.")] Sprite intentSprite;
    [SerializeField][Tooltip("Graphic for actual ship facing.")] Sprite aimSprite;
    [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
    [SerializeField] float initialDistanceFromPlayer = 5f;

    GameObject player = null;
    PlayerMovement playerMovement;
    Camera mainCamera;

    GameObject intentRotation;
    GameObject intentPosition;
    GameObject aimPosition;

    // These vectors correspond to the directions relative to the camera
    private Vector3 cameraForward;
    private Vector3 cameraBack;
    private Vector3 cameraRight;
    private Vector3 cameraLeft;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

        Vector3 initialReticlePosition = player.transform.position + new Vector3(0, 0, initialDistanceFromPlayer);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        CreateIntentReticle(initialReticlePosition);
        CreateAimReticle(initialReticlePosition);

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

    private void CreateIntentReticle(Vector3 initialPosition)
    {
        GameObject intentReticle = new GameObject("Intent Reticle");
        intentReticle.transform.position = initialPosition;
        intentReticle.transform.localScale = new Vector3(.25f, .25f, 1f); // TODO is this the only way to make the sprite look rite?

        SpriteRenderer intentReticleSR = intentReticle.AddComponent<SpriteRenderer>();
        intentReticleSR.sprite = intentSprite;

        intentReticle.transform.rotation = Quaternion.Euler(90, 0, 0); // Correct the rotation so the sprite is "flat"

        // The "intentRotation" GameObject exists so that the reticle can be made to "face" the ship while it's Z-rotation is weird
        intentRotation = new GameObject("Intent Rotation");
        intentRotation.transform.position = initialPosition;

        intentReticle.transform.SetParent(intentRotation.transform);

        // The "intentPosition" is used so that the reticle can be moved relative to the camera's facing regardless of it's rotation
        intentPosition = new GameObject("Intent Position");
        intentPosition.transform.position = initialPosition;
        intentPosition.transform.eulerAngles = new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0);

        intentPosition.transform.SetParent(gameObject.transform);
        intentRotation.transform.SetParent(intentPosition.transform);
    }

    private void CreateAimReticle(Vector3 initialPosition)
    {
        GameObject aimReticle = new GameObject("Aim Reticle");
        aimReticle.transform.position = initialPosition;
        aimReticle.transform.localScale = new Vector3(.25f, .25f, 1f); // TODO is this the only way to make the sprite look rite?

        SpriteRenderer aimReticleSR = aimReticle.AddComponent<SpriteRenderer>();
        aimReticleSR.sprite = aimSprite;

        aimReticle.transform.rotation = Quaternion.Euler(90, 0, 0); // Correct the rotation so the sprite is "flat"

        // The "intent" GameObject exists so that the reticle can be made to "face" the ship while it's Z-rotation is weird
        aimPosition = new GameObject("Aim Position");
        aimPosition.transform.position = initialPosition;

        aimPosition.transform.SetParent(gameObject.transform);
        aimReticle.transform.SetParent(aimPosition.transform);
    }

    void Update()
    {
        if (player)
        {
            MoveIntentReticle(); // TODO this method should only be called IF the mouse moves
            MoveAimReticle();
            ClampReticlesWithinView();
            LockReticlesYPosition();
        }
        else
        {
            Destroy(intentPosition);
            Destroy(intentRotation);
            Destroy(aimPosition);
        }
    }

    private void LockReticlesYPosition()
    {
        intentPosition.transform.position = new Vector3(intentPosition.transform.position.x, 0, intentPosition.transform.position.z);
        intentRotation.transform.position = new Vector3(intentRotation.transform.position.x, 0, intentRotation.transform.position.z);
        aimPosition.transform.position = new Vector3(aimPosition.transform.position.x, 0, aimPosition.transform.position.z);
    }

    private void LateUpdate()
    {
        if (player) AdjustPlayerAim();
    }

    private void ClampReticlesWithinView()
    {
        // TODO how does this work?
        Vector3 intentPos = Camera.main.WorldToViewportPoint(intentRotation.transform.position);
        intentPos.x = Mathf.Clamp01(intentPos.x);
        intentPos.y = Mathf.Clamp01(intentPos.y);
        intentRotation.transform.position = Camera.main.ViewportToWorldPoint(intentPos);

        Vector3 aimPos = Camera.main.WorldToViewportPoint(aimPosition.transform.position);
        aimPos.x = Mathf.Clamp01(aimPos.x);
        aimPos.y = Mathf.Clamp01(aimPos.y);
        aimPosition.transform.position = Camera.main.ViewportToWorldPoint(aimPos);
    }

    private void MoveIntentReticle()
    {
        // TODO make this work cross-platform
        Transform positionTransform = intentPosition.transform;
        Transform rotationTransform = intentRotation.transform;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        Vector3 newPosition = new Vector3(mouseX, 0, mouseY);

        //if (CheckPositionInView(newPosition))
        //{
        positionTransform.Translate(newPosition);
        rotationTransform.transform.LookAt(player.transform);
        //}
    }

    private void MoveAimReticle()
    {
        Vector3 currentPosition = aimPosition.transform.position;
        Vector3 intentReticlePosition = intentRotation.transform.position;

        Vector3 newPosition = Vector3.MoveTowards(currentPosition, intentReticlePosition, playerMovement.rotationSpeed);

        aimPosition.transform.position = newPosition;
        aimPosition.transform.LookAt(player.transform);
    }

    //private bool CheckPositionInView(Vector3 target)
    //{
    //    Vector3 screenPoint = mainCamera.WorldToViewportPoint(target);
    //    return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    //}

    private void AdjustPlayerAim()
    {
        player.transform.LookAt(aimPosition.transform);
    }
}
