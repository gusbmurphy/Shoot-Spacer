using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadarIndicator : MonoBehaviour
{
  public Enemy target;
  public Transform playerTransform;

  private void Start()
  {
    void DeathHandler() { Destroy(this.gameObject); }
    target.OnDeath += DeathHandler;
  }

  private void LateUpdate()
  {
    transform.position = playerTransform.position;
    transform.LookAt(target.transform);
  }
}
