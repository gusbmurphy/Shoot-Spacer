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

    GameObject player = null;
    PlayerShip playerController;
    GameObject mainCamera;

    GameObject intentRotation;
    GameObject intentPosition;
    GameObject aim;

    // TODO this way of making the controls conform to the perspective of the camera doesn't feel right, make it better
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

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // TODO fix the reticle "flicking" to the center because of cursor locking
        CreateIntentReticle();
        CreateAimReticle();

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

        // Destroy(cameraCompass); // We don't need the Compass anymore!
    }

    private void CreateIntentReticle()
    {
        GameObject intentReticle = new GameObject("Intent Reticle");
        intentReticle.transform.position = new Vector3(0, 0, 0);
        intentReticle.transform.localScale = new Vector3(.25f, .25f, 1f); // TODO is this the only way to make the sprite look rite?

        SpriteRenderer intentReticleSR = intentReticle.AddComponent<SpriteRenderer>();
        intentReticleSR.sprite = intentSprite;

        intentReticle.transform.rotation = Quaternion.Euler(90, 0, 0); // Correct the rotation so the sprite is "flat"

        // The "intentRotation" GameObject exists so that the reticle can be made to "face" the ship while it's Z-rotation is weird
        intentRotation = new GameObject("Intent Rotation");
        intentRotation.transform.position = new Vector3(0, 0, 0);

        intentReticle.transform.SetParent(intentRotation.transform);

        // The "intentPosition" is used so that the reticle can be moved relative to the camera's facing regardless of it's rotation
        intentPosition = new GameObject("Intent Position");
        intentPosition.transform.position = new Vector3(0, 0, 0);
        intentPosition.transform.eulerAngles = new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0);

        intentRotation.transform.SetParent(intentPosition.transform);
    }

    private void CreateAimReticle()
    {
        GameObject aimReticle = new GameObject("Aim Reticle");
        aimReticle.transform.position = new Vector3(0, 0, 0);
        aimReticle.transform.localScale = new Vector3(.25f, .25f, 1f); // TODO is this the only way to make the sprite look rite?

        SpriteRenderer aimReticleSR = aimReticle.AddComponent<SpriteRenderer>();
        aimReticleSR.sprite = aimSprite;

        aimReticle.transform.rotation = Quaternion.Euler(90, 0, 0); // Correct the rotation so the sprite is "flat"

        // The "intent" GameObject exists so that the reticle can be made to "face" the ship while it's Z-rotation is weird
        aim = new GameObject("Aim");
        aimReticle.transform.position = new Vector3(0, 0, 0);

        aimReticle.transform.SetParent(aim.transform);
    }

    void Update()
    {
        MoveIntentReticle(); // TODO this method should only be called IF the mouse moves
        MoveAimReticle();
        ClampReticlesWithinView();
        AdjustPlayerAim();
    }

    private void ClampReticlesWithinView()
    {
        // TODO how does this work?
        Vector3 intentPos = Camera.main.WorldToViewportPoint(intentRotation.transform.position);
        intentPos.x = Mathf.Clamp01(intentPos.x);
        intentPos.y = Mathf.Clamp01(intentPos.y);
        intentRotation.transform.position = Camera.main.ViewportToWorldPoint(intentPos);

        Vector3 aimPos = Camera.main.WorldToViewportPoint(aim.transform.position);
        aimPos.x = Mathf.Clamp01(aimPos.x);
        aimPos.y = Mathf.Clamp01(aimPos.y);
        aim.transform.position = Camera.main.ViewportToWorldPoint(aimPos);
    }

    private void MoveIntentReticle()
    {
        // TODO make this work cross-platform
        // TODO make this scale with frame rate
        // TODO fix reticle moving into Y-plane
        Transform positionTransform = intentPosition.transform;
        Transform rotationTransform = intentRotation.transform;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        positionTransform.Translate(new Vector3(mouseX, 0, mouseY));

        rotationTransform.transform.LookAt(player.transform);
    }

    private void MoveAimReticle()
    {
        Vector3 currentPosition = aim.transform.position;
        Vector3 intentReticlePosition = intentRotation.transform.position;

        Vector3 newPosition = Vector3.MoveTowards(currentPosition, intentReticlePosition, playerController.rotationSpeed);

        aim.transform.position = newPosition;
        aim.transform.LookAt(player.transform);
    }

    private void AdjustPlayerAim()
    {
        player.transform.LookAt(aim.transform);
    }
}
