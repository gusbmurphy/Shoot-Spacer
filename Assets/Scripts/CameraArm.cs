using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraArm : MonoBehaviour
{
    GameObject player = null;

    void Start() { player = GameObject.FindWithTag("Player"); }

    void LateUpdate() 
    { 
        transform.position = player.transform.position;
        transform.rotation = player.transform.rotation;
    }
}

// TODO Have camera track the aiming reticle—not ship movement.