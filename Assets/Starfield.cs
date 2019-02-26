using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Starfield : MonoBehaviour
{
    public Transform mainCameraTransform;

    public int MaxStars = 100;
    public float StarSize = 0.1f;
    public float StarSizeRange = 0.5f;
    public float FieldWidth = 20f;
    public float FieldHeight = 25f;
    public bool Colorize = false;

    ParticleSystem Particles;
    ParticleSystem.Particle[] Stars;

    private float xOffset;
    private float zOffset;

    void Awake()
    {
        Stars = new ParticleSystem.Particle[MaxStars];
        Particles = GetComponent<ParticleSystem>();

        Assert.IsNotNull(Particles, "Particle system missing from object!");
        Assert.IsNotNull(mainCameraTransform, "Main camera transform missing from object!");

        xOffset = FieldWidth * 0.5f; // Offset the coordinates to distribute the spread
        zOffset = FieldHeight * 0.5f; // around the object's center

        for (int i = 0; i < MaxStars; i++)
        {
            float randSize = Random.Range(StarSizeRange, StarSizeRange + 1f); // Randomize star size within parameters
            float scaledColor = (true == Colorize) ? randSize - StarSizeRange : 1f; // If coloration is desired, color based on size

            Stars[i].position = GetRandomInRectangle(FieldWidth, FieldHeight) + transform.position;
            Stars[i].startSize = StarSize * randSize;
            Stars[i].startColor = new Color(1f, scaledColor, scaledColor, 1f);
        }

        Particles.SetParticles(Stars, Stars.Length); // Write data to the particle system
    }

    Vector3 GetRandomInRectangle(float width, float height)
    {
        float x = Random.Range(0, width);
        float z = Random.Range(0, height);
        return new Vector3(x - xOffset, 0, z - zOffset);
    }

    void Update()
    {
        for (int i = 0; i < MaxStars; i++)
        {
            Vector3 pos = Stars[i].position + transform.position;

            if (pos.x < (mainCameraTransform.position.x - xOffset))
            {
                pos.x += FieldWidth;
            }
            else if (pos.x > (mainCameraTransform.position.x + xOffset))
            {
                pos.x -= FieldWidth;
            }

            if (pos.z < (mainCameraTransform.position.z - zOffset))
            {
                pos.z += FieldHeight;
            }
            else if (pos.z > (mainCameraTransform.position.z + zOffset))
            {
                pos.z -= FieldHeight;
            }

            Stars[i].position = pos - transform.position;
        }

        Particles.SetParticles(Stars, Stars.Length);
    }
}

