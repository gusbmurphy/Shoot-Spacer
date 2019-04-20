using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] int hitPoints = 5;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnParticleCollision(GameObject other)
    {
        hitPoints--;
        print(gameObject.name + " was hit down to " + hitPoints + " hitpoints.");
        // todo give visual hit feedback
        if (hitPoints < 1)
        {
            print(gameObject.name + " was destroyed.");
            Destroy(this.gameObject);
            // todo give visual destruction feedback
        }
    }
}
