using System;
using UnityEngine;

public class OrcaController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private Path path;
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float waypointRadius = 5.0f;
    [SerializeField] private float waterDrag = 1f;

    [Header("Avoidance Settings")]
    [SerializeField] private float avoidanceRadius = 1.5f;
    [SerializeField] private float avoidanceForce = 5f;

    public static event Action OnPlayerCaught;

    private Rigidbody rb;
    private int curPathIndex = 0;
    private Vector3 target;

    // Behaviors
    private Behavior behavior;
    private ClockwiseLoop clockwiseLoop = new();
    private CounterClockwiseLoop counterClockwiseLoop = new();
    private RandomLoop randomLoop = new();

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearDamping = waterDrag;
        //Initial Behavior
        SetBehavior(clockwiseLoop);
    }

    private void Update()
    {
        target = path.GetPoint(curPathIndex);

        if (Vector3.Distance(transform.position, target) < waypointRadius)
        {
            curPathIndex = behavior.GetNextPoint(curPathIndex);
        }
    }

    //Movement Modes
    public void Stop()
    {
        rb.linearVelocity = Vector3.zero;
    }

    public void Patrol()
    {
        MoveTowards(target);
    }

    public void Chase(Transform player)
    {
        MoveTowards(player.position);
    }

    public void CircleAround(Transform player)
    {
        Vector3 offset = (transform.position - player.position).normalized;
        offset.y = 0f;
        Vector3 tangent = new Vector3(-offset.z, 0f, offset.x); // perpendicular vector
        Vector3 circleTarget = player.position + tangent * 3f; // radius 3
        circleTarget.y = 0f;
        MoveTowards(circleTarget);
    }

    private void MoveTowards(Vector3 destination)
    {
        Vector3 direction = destination - transform.position;
        direction.y = 0f;
        direction.Normalize();

        //Avoid icebergs
        direction = AvoidIcebergs(direction);

        float angle = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
        float rotationStep = rotationSpeed * Time.fixedDeltaTime;
        float clampedAngle = Mathf.Clamp(angle, -rotationStep, rotationStep);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, clampedAngle, 0f));

        rb.AddForce(transform.forward * speed);
    }

    private Vector3 AvoidIcebergs(Vector3 desiredDirection)
    {
        RaycastHit hit;
        if (Physics.SphereCast(rb.position, avoidanceRadius, transform.forward, out hit, avoidanceRadius * 2f, LayerMask.GetMask("Iceberg")))
        {
            //Push away from obstacles
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0f;
            desiredDirection += hitNormal * avoidanceForce;
            desiredDirection.Normalize();
        }
        return desiredDirection;
    }

    public void SetBehavior(Behavior newBehavior)
    {
        behavior = newBehavior;
        behavior.SetPath(path);
    }

    public void RandomizeBehavior()
    {
        int choice = UnityEngine.Random.Range(0, 3);
        switch (choice)
        {
            case 0: SetBehavior(clockwiseLoop); break;
            case 1: SetBehavior(counterClockwiseLoop); break;
            case 2: SetBehavior(randomLoop); break;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            //Eat the player
            OnPlayerCaught?.Invoke();
        }
    }
}