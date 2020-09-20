using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Big thank you to codinBlack (not sure if that's someone's name?) for their blog post on drawing circles:
// https://www.codinblack.com/how-to-draw-lines-circles-or-anything-else-using-linerenderer/

public class PlayerRadar : MonoBehaviour
{
  public LineRenderer lineRenderer;
  public int numOfVertices = 30;
  public float radius = 4f;
  public float width = 0.1f;
  [Tooltip("Distance that radar noise effects radar points.")]
  public float noiseRadius = 0.5f;
  private EnemySpawn[] spawns;
  private List<Transform> enemyTransforms = new List<Transform>();
  private Vector3[] feedbackPoints;

  void Start()
  {
    DrawCircle();
    AddSpawnHandlers();
  }

  private void DrawCircle()
  {
    lineRenderer = GetComponent<LineRenderer>();
    lineRenderer.useWorldSpace = false;
    lineRenderer.startWidth = width;
    lineRenderer.endWidth = width;
    lineRenderer.loop = true;
    lineRenderer.positionCount = numOfVertices;

    // Again, all credit to codinBlack for this math!
    Vector3 centerPoint = transform.position;
    float angle = 2 * Mathf.PI / numOfVertices;

    for (int i = 0; i < numOfVertices; i++)
    {
      Matrix4x4 rotationMatrix = new Matrix4x4
      (
        new Vector4(Mathf.Cos(angle * i), 0, Mathf.Sin(angle * i), 0),
        new Vector4(0, 1, 0, 0),
        new Vector4(-1 * Mathf.Sin(angle * i), 0, Mathf.Cos(angle * i), 0),
        new Vector4(0, 0, 0, 1)
      );
      Vector3 initialRelativePosition = new Vector3(radius, 0, 0);
      lineRenderer.SetPosition(i, centerPoint + rotationMatrix.MultiplyPoint(initialRelativePosition));
    }
  }

  private void AddSpawnHandlers()
  {
    spawns = GameObject.FindObjectsOfType<EnemySpawn>();
    void EnemySpawnHandler(Transform enemyTransform) { enemyTransforms.Add(enemyTransform); }
    void EnemyDeathHandler(Transform enemyTransform){ enemyTransforms.Remove(enemyTransform); }

    foreach (EnemySpawn spawn in spawns)
    {
      spawn.OnEnemySpawn += EnemySpawnHandler;
      spawn.OnSpawnedDeath += EnemyDeathHandler;
    }

  }

  private void Update()
  {
    if (enemyTransforms.Count > 0) CreateRadarFeedback(enemyTransforms.Select(transform => transform.position).ToArray());
  }

  private void CreateRadarFeedback(Vector3[] targets)
  {
    // Find intersections (feedbackPoint) between the radar ring and a line from the center to the targets,
    // which will be a line from the center to the target whose length is the radar ring's radius.
    feedbackPoints = targets.Select(target =>
      { return (target - transform.position).normalized * radius; }
      ).ToArray();

    Vector3[] newPositions = new Vector3[lineRenderer.positionCount];
    lineRenderer.GetPositions(newPositions);
    // Go through each position on the radar ring.
    for (int i = 0; i < newPositions.Length; i++)
    {
      Vector3 position = newPositions[i];

      int j = 0;
      bool isModified = false;
      // Now, loop through each of the feedback points, modifying the point on the ring
      // if it's within distance of one of them, and breaking if so.
      while (!isModified && j < feedbackPoints.Length)
      {
        Vector3 feedbackPoint = feedbackPoints[j];
        float distanceFromFeedback = (position - feedbackPoint).magnitude;
        if (distanceFromFeedback < noiseRadius)
        {
          float x = newPositions[i].x;
          float z = newPositions[i].z;
          newPositions[i].Set(x, 1f, z);
          isModified = true;
        }
        j++;
      }
      // If the point was not near a feedback point, make sure it is set to be "flat".
      if (!isModified)
      {
        float x = newPositions[i].x;
        float z = newPositions[i].z;
        newPositions[i].Set(x, 0f, z);
      }
    }

    lineRenderer.SetPositions(newPositions);
  }

  private void OnDrawGizmos()
  {
    Gizmos.color = Color.green;
    foreach (Vector3 fbPoint in feedbackPoints)
    {
      Gizmos.DrawSphere(fbPoint, 0.1f);
    }
  }
}
