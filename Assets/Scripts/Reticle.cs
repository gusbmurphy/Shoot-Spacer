using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField][Tooltip("Graphic for player intended facing.")] Sprite intentSprite = null;
    [SerializeField][Tooltip("Graphic for actual ship facing.")] Sprite aimSprite = null;
    [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

    GameObject player = null;
    PlayerController playerController;
    GameObject mainCamera;
    GameObject intentReticle;
    GameObject aimReticle;
    SpriteRenderer intentReticleSR;
    SpriteRenderer aimReticleSR;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

        CreateIntentReticle();
    }

    private void CreateIntentReticle()
    {
        intentReticle = new GameObject("Intent Reticle");
        intentReticle.transform.position = new Vector3(0, 0, 0);

        intentReticleSR = intentReticle.AddComponent<SpriteRenderer>();
        intentReticleSR.sprite = intentSprite;

        intentReticle.transform.rotation = Quaternion.Euler(90, 0, 0); // Correct the rotation so the sprite is "flat"
    }

    void Update()
    {
        ProcessMouseMovement();
    }

    private void ProcessMouseMovement()
    {
        /* Raycast into the scene from the camera to find the intersection with 
         * the play area. Tell the ship to begin facing towards that point. */
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        int playAreaLayerMask = 1 << 9;

        if (Physics.Raycast(ray, 100f, playAreaLayerMask))
        {
            RaycastHit[] raycastHits = Physics.RaycastAll(ray, 100f, playAreaLayerMask);
            if (raycastHits.Length > 1) 
            { 
                Debug.Log("Somehow hit more than one Play Area?");
            }

            playerController.SetIntendedFacing(raycastHits[0].point);
        }
        else
        {
            Debug.Log("Did not hit the play area.");
            return;
        }
    }
}
