using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
  public float width = 1f;
  public int damage = 100;

  private LineRenderer lineRenderer;
  void Start()
  {
    // Gather positions from all of the barrier columns (which are children of the Barrier object), and correct the Y-coordinate so the beams made are on the same plane as gameplay.
    Transform[] columns = GetComponentsInChildren<Transform>();
    List<Vector3> nodes = new List<Vector3>();
    Vector3 newVector;
    foreach (Transform column in columns)
    {
      newVector = column.position;
      newVector.y = 0;
      nodes.Add(newVector);
    }
    nodes.RemoveAt(0); // Since GetComponentsInChildren() will first get the transform of the parent, we need to removed that first entry.
    nodes.Add(nodes[0]); // Close the barrier by adding the first node to the end.

    // Create the beams with a Line Renderer.
    lineRenderer = gameObject.AddComponent<LineRenderer>();
    lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    lineRenderer.widthMultiplier = width;
    lineRenderer.positionCount = nodes.Count;
    lineRenderer.SetPositions(nodes.ToArray());

    // Add BoxColliders to each segment of the line, we'll also be calculating various attributes for it. Thanks to this blog post: http://www.theappguruz.com/blog/add-collider-to-line-renderer-unity
    BoxCollider newCollider;
    GameObject newObject;
    float length;
    Vector3 midPoint, start, end;
    float angle;
    for (int i = 0; i < (nodes.Count - 1); i++) // We're using (nodes.Count - 1) because the last node will be a repeat of the first.
    {
      newObject = new GameObject("Barrier Collider");
      newCollider = newObject.AddComponent<BoxCollider>();
      newCollider.transform.parent = columns[i + 1]; // Right now, the columns will still include the parent transform, so we're correcting for that here.

      start = nodes[i];
      end = nodes[i + 1];

      length = Vector3.Distance(start, end);
      newCollider.size = new Vector3(length, width, width);

      midPoint = (start + end) / 2;
      newCollider.transform.position = midPoint;

      // TODO: Understand what's going on with this angle business!
      angle = (Mathf.Abs(start.z - end.z) / Mathf.Abs(start.x - end.x));
      if ((start.z > end.z && start.x < end.x) || (end.z > start.z && end.x < start.x))
      {
        angle *= -1;
      }
      angle = -Mathf.Rad2Deg * Mathf.Atan(angle); // Okay why did I have to make this negative?
      newCollider.transform.Rotate(0, angle, 0);

      // Set the collider to be a trigger.
      newCollider.isTrigger = true;
      newObject.AddComponent<BarrierCollision>().damage = damage;
    }
  }
}
