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

    GameObject intent;
    GameObject aim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerShip>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        CreateIntentReticle();
        CreateAimReticle();
    }

    private void CreateIntentReticle()
    {
        GameObject intentReticle = new GameObject("Intent Reticle");
        intentReticle.transform.position = new Vector3(0, 0, 0);
        intentReticle.transform.localScale = new Vector3(.25f, .25f, 1f);

        SpriteRenderer intentReticleSR = intentReticle.AddComponent<SpriteRenderer>();
        intentReticleSR.sprite = intentSprite;

        intentReticle.transform.rotation = Quaternion.Euler(90, 0, 0); // Correct the rotation so the sprite is "flat"

        // The "intent" GameObject exists so that the reticle can be made to "face" the ship while it's Z-rotation is weird
        intent = new GameObject("Intent");
        intentReticle.transform.position = new Vector3(0, 0, 0);

        intentReticle.transform.SetParent(intent.transform);
    }

    private void CreateAimReticle()
    {
        GameObject aimReticle = new GameObject("Aim Reticle");
        aimReticle.transform.position = new Vector3(0, 0, 0);
        aimReticle.transform.localScale = new Vector3(.25f, .25f, 1f);

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
        // TODO reticles should rotate so that they align with ship's facing
        MoveIntentReticle();
        MoveAimReticle();
        AdjustPlayerAim();
    }

    private void MoveIntentReticle()
    {
        // TODO make this work cross-platform
        // TODO make this scale with frame rate
        // TODO lock the intent reticle to within the screen, and then implement scrolling on screen edges
        Transform intentTransform = intent.transform;

        float newX = (intentTransform.position.x + Input.GetAxis("Mouse X")) * mouseSensitivity;
        float newZ = (intentTransform.position.z + Input.GetAxis("Mouse Y")) * mouseSensitivity;

        intentTransform.position = new Vector3(newX, 0, newZ);

        intentTransform.transform.LookAt(player.transform);
    }

    private void MoveAimReticle()
    {
        Vector3 currentPosition = aim.transform.position;
        Vector3 intentReticlePosition = intent.transform.position;

        Vector3 newPosition = Vector3.MoveTowards(currentPosition, intentReticlePosition, playerController.rotationSpeed);

        aim.transform.position = newPosition;
        aim.transform.LookAt(player.transform);
    }

    private void AdjustPlayerAim()
    {
        player.transform.LookAt(aim.transform);
    }
}
