using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reticle : MonoBehaviour
{
    [SerializeField][Tooltip("Graphic for player intended facing.")] Texture2D intent = null;
    [SerializeField][Tooltip("Graphic for actual ship facing.")] Texture2D aim = null;
    [SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

    GameObject player = null;
    PlayerController playerController;
    // TODO Consider making reticle not just map to cursor but move from input.
    void Start()
    {
        Cursor.SetCursor(intent, cursorHotspot, CursorMode.Auto);
        player = GameObject.FindGameObjectWithTag("Player");
        playerController = player.GetComponent<PlayerController>();
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
