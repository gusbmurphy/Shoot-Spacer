using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField] float mouseSensitivity = 1f;
    [SerializeField][Tooltip("Graphic for player intended facing.")] Sprite intentSprite;
    [SerializeField][Tooltip("Graphic for actual ship facing.")] Sprite aimSprite;
    [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
    [SerializeField] float initialDistanceFromPlayer = 5f;

    GameObject player = null;
    PlayerShip playerController;
    GameObject mainCamera;

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
        playerController = player.GetComponent<PlayerShip>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

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

        intentPosition.transform.SetParent(mainCamera.transform); // The position object's parent is set to the main camera so that as the camera moves, the reticle moves with it
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

        aimPosition.transform.SetParent(mainCamera.transform); // The position object's parent is set to the main camera so that as the camera moves, the reticle moves with it
        aimReticle.transform.SetParent(aimPosition.transform);
    }

    void Update()
    {
        MoveIntentReticle(); // TODO this method should only be called IF the mouse moves
        MoveAimReticle();
        ClampReticlesWithinView();
    }

    private void LateUpdate()
    {
        AdjustPlayerAim();
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
        // TODO make this scale with frame rate
        Transform positionTransform = intentPosition.transform;
        Transform rotationTransform = intentRotation.transform;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        positionTransform.Translate(new Vector3(mouseX, 0, mouseY));

        rotationTransform.transform.LookAt(player.transform);
    }

    private void MoveAimReticle()
    {
        Vector3 currentPosition = aimPosition.transform.position;
        Vector3 intentReticlePosition = intentRotation.transform.position;

        Vector3 newPosition = Vector3.MoveTowards(currentPosition, intentReticlePosition, playerController.rotationSpeed);

        aimPosition.transform.position = newPosition;
        aimPosition.transform.LookAt(player.transform);
    }

    private void AdjustPlayerAim()
    {
        player.transform.LookAt(aimPosition.transform);
    }
}
