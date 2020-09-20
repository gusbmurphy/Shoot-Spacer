using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour, IDamageable
{
  [SerializeField] int startingHitPoints = 10;

  [Header("Movement")]
  [SerializeField] int thrustForce = 2500;
  [SerializeField] float maxNormalSpeed = 10f;
  private float currentMaxSpeed;
  [SerializeField] public float rotationSpeed = 1f;

  [Header("Attack")]
  [SerializeField] GameObject attackProjectile;
  [SerializeField] GameObject projectileSocket;
  [SerializeField] [Tooltip("Projectiles per minute.")] int fireRate = 500;

  [Header("HUD")]
  [SerializeField] Slider healthBar;
  [SerializeField] GameManager gameManager;

  [Header("Radar")]
  [SerializeField] int numOfVertices = 30;
  [SerializeField] float radius = 4f;
  [SerializeField] float width = 0.1f;
  [Tooltip("Distance that radar noise effects radar points.")]
  [SerializeField] float noiseRadius = 0.5f;

  [Header("Cheats")]
  [SerializeField] bool godModeOn = false;

  private Rigidbody shipRigidbody;
  private GameObject playerShip;
  private int currentHitPoints;
  private EnemySpawn[] spawns;
  private List<Transform> enemyTransforms = new List<Transform>();
  private Vector3[] feedbackPoints;
  private LineRenderer lineRenderer;

  void Start()
  {
    currentHitPoints = startingHitPoints;
    healthBar.maxValue = startingHitPoints;
    healthBar.value = startingHitPoints;
    shipRigidbody = GetComponentInChildren<Rigidbody>();
    playerShip = GameObject.FindGameObjectWithTag("PlayerShip");
    SetUpCameraDirections();
    currentMaxSpeed = maxNormalSpeed;
    DrawCircle();
    AddSpawnHandlers();
  }

  void Update()
  {
    // TODO: Is there a better way to get this firing done than InvokeRepeating? Same goes for the enemies.
    // TODO: There has to be a better way than using these string references, right?
    if (Input.GetButtonDown("Fire1")) InvokeRepeating("Fire", 0f, 60f / fireRate);
    else if (Input.GetButtonUp("Fire1")) CancelInvoke();
    CheckForInput();
    ClampSpeed();
    if (enemyTransforms.Count > 0) CreateRadarFeedback(enemyTransforms.Select(transform => transform.position).ToArray());
  }

  private void LateUpdate() 
  {
    transform.position = playerShip.transform.position;
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

  // These vectors correspond to the directions relative to the camera
  private Vector3 cameraForward;
  private Vector3 cameraBack;
  private Vector3 cameraRight;
  private Vector3 cameraLeft;

  private void SetUpCameraDirections()
  {
    GameObject cameraCompass = new GameObject("Camera Compass");
    // "Correct" the compass so that it lies flat in the game plane
    cameraCompass.transform.eulerAngles = new Vector3(0, GameObject.FindGameObjectWithTag("MainCamera").transform.eulerAngles.y, 0);

    cameraForward = cameraCompass.transform.forward;
    cameraBack = -cameraCompass.transform.forward;
    cameraRight = cameraCompass.transform.right;
    cameraLeft = -cameraCompass.transform.right;

    Destroy(cameraCompass); // We don't need the Compass anymore!
  }

  private void CheckForInput()
  {
    CheckForThrust();
    //foreach (KeyCode key in movementKeyCodes) if (Input.GetKeyDown(key)) CheckForDash(key);
  }

  private void CheckForThrust()
  {
    if (Input.GetKey(KeyCode.A)) Thrust(cameraLeft);
    if (Input.GetKey(KeyCode.D)) Thrust(cameraRight);
    if (Input.GetKey(KeyCode.W)) Thrust(cameraForward);
    if (Input.GetKey(KeyCode.S)) Thrust(cameraBack);
  }

  public void Thrust(Vector3 direction)
  {
    shipRigidbody.AddForce(direction * thrustForce * Time.deltaTime);
  }

  public void Thrust(Vector3 direction, float force)
  {
    print("Dashing.");
    shipRigidbody.AddForce(direction * force);
  }

  private void ClampSpeed()
  {
    if (shipRigidbody.velocity.magnitude > maxNormalSpeed) 
      shipRigidbody.velocity = Vector3.ClampMagnitude(shipRigidbody.velocity, maxNormalSpeed);
  }

  void IDamageable.TakeDamage(int damage)
  {
    if (godModeOn) return;

    currentHitPoints -= damage;
    healthBar.value = currentHitPoints;
    // TODO: Give visual hit feedback.
    if (currentHitPoints <= 0) Die();
  }

  private void Die()
  {
    Destroy(gameObject);
    // TODO: Give visual hit feedback.
    gameManager.GameOver();
  }

  private void Fire()
  {
    GameObject projectileObject = Instantiate(attackProjectile, projectileSocket.transform.position, projectileSocket.transform.rotation);
    Projectile projectileComponent = projectileObject.GetComponent<Projectile>();

    Vector3 unitDirectionVector = (projectileSocket.transform.position - transform.position).normalized;
    projectileObject.GetComponent<Rigidbody>().velocity = unitDirectionVector * projectileComponent.projectileSpeed;
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
}
