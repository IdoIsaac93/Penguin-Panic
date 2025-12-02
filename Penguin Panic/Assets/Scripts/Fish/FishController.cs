using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{
    [Header("Speed Variables")]
    [SerializeField] private float minSpeed = 2.0f;
    [SerializeField] private float turnSpeed = 2.0f;
    [SerializeField] private float randomFreq = 2.0f;
    [SerializeField] private float randomForce = 2.0f;
    [SerializeField] private float maxSpeed = 5.0f;

    //Alignment variables
    [Header("Alignment Variables")]
    [SerializeField] private float toOriginForce = 1.0f;
    [SerializeField] private float toOriginRange = 5.0f;

    //Seperation variabes
    [Header("Seperation Variables")]
    [SerializeField] private float avoidanceRadius = 1.0f;
    [SerializeField] private float avoidanceForce = 1.0f;

    //Cohesion variables
    [Header("Cohesion Variables")]
    [SerializeField] private float followVelocity = 1.0f;
    [SerializeField] private float followRadius = 3.0f;

    //Player avoidance variables
    [Header("Player Avoidance Variables")]
    [SerializeField] private float playerAvoidanceRadius = 2.0f;
    [SerializeField] private float playerAvoidanceForce = 2.0f;

    //Scoring
    [Header("Scoring Variables")]
    [SerializeField] private int scoreAmount = 10;
    public static event Action<int> OnFishCaught;

    //Water spray
    [SerializeField] private ParticleSystem sprayParticles;
    private ParticleSystem.EmissionModule emission;

    private SchoolController school;
    private Vector3 velocity;
    private Vector3 randomPush;
    private Vector3 originPush;
    private Transform[] objects;
    private FishController[] otherFish;
    private Rigidbody rb;

    public void SetSchool(SchoolController s) { school = s; }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();

        velocity = UnityEngine.Random.insideUnitCircle;
        velocity.y = 0f;
        velocity *= minSpeed;

        emission = sprayParticles.emission;

        StartCoroutine(UpdateRandom());
    }

    IEnumerator UpdateRandom()
    {
        while (true)
        {
            randomPush = UnityEngine.Random.insideUnitSphere;
            randomPush.y = 0f;
            randomPush *= randomForce;
            yield return new WaitForSeconds(randomFreq);
        }
    }

    public void RebuildFishList(List<FishController> fishList)
    {
        objects = new Transform[fishList.Count];
        otherFish = new FishController[fishList.Count];
        for (int i = 0; i < fishList.Count; i++)
        {
            objects[i] = fishList[i].transform;
            otherFish[i] = fishList[i];
        }
    }

    private void Update()
    {
        emission.rateOverTime = rb.linearVelocity.magnitude * 2f;
    }
    private void FixedUpdate()
    {
        if (school == null) { return; }

        //Internal variables
        Vector3 avgVelocity = Vector3.zero;
        Vector3 avgPosition = Vector3.zero;
        int count = 0;
        Vector3 myPosition = rb.position;

        for (int i = 0; i < objects.Length; i++)
        {
            Transform fishTransform = objects[i];

            //Skip self and fish in the list already caught
            if (fishTransform == null || fishTransform == transform) { continue; }

            Vector3 otherPosition = fishTransform.position;

            //Average position
            avgPosition += otherPosition;
            count++;

            //Magnitude
            Vector3 forceV = myPosition - otherPosition;
            forceV.y = 0f;
            float directionMagnitude = forceV.magnitude;

            if (directionMagnitude < followRadius)
            {
                if (directionMagnitude < avoidanceRadius && directionMagnitude > 0f)
                {
                    float forceMagnitude = 1.0f - (directionMagnitude / avoidanceRadius);
                    avgVelocity += (forceV / directionMagnitude) * forceMagnitude * avoidanceForce;
                }

                float followMagnitude = directionMagnitude / followRadius;
                avgVelocity += followVelocity * followMagnitude * otherFish[i].velocity.normalized;
            }

        }

        //Calculate average velocity
        Vector3 toAvg = count > 0 ? (avgPosition / count) - myPosition : Vector3.zero;

        //Direction to leader
        Vector3 forceOrigin = school.transform.position - transform.position;
        forceOrigin.y = 0f;
        float leaderDist = forceOrigin.magnitude;

        //Calculate velocity of flock
        if (leaderDist > 0)
            originPush = (leaderDist / toOriginRange) * toOriginForce * (forceOrigin / leaderDist);

        //Clamp speed
        if (velocity.magnitude < minSpeed && velocity.magnitude > 0)
            velocity = velocity.normalized * minSpeed;
        if (velocity.magnitude > maxSpeed)
            velocity = velocity.normalized * maxSpeed;

        //Calculate final velocity
        Vector3 wantedVel = velocity;
        wantedVel += randomPush * Time.fixedDeltaTime;
        wantedVel += originPush * Time.fixedDeltaTime;
        wantedVel += avgVelocity * Time.fixedDeltaTime;
        wantedVel += toAvg.normalized * Time.fixedDeltaTime;

        //Avoid icebergs
        AvoidIcebergs(ref wantedVel);

        //Avoid player
        AvoidPlayer(ref wantedVel);

        //Move the flock based on velocity
        velocity = Vector3.Lerp(velocity, wantedVel, turnSpeed * Time.fixedDeltaTime);
        float angle = Vector3.SignedAngle(transform.forward, velocity, Vector3.up);
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, angle, 0f));
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        //Lock y axis
        rb.position = new Vector3(rb.position.x, 0f, rb.position.z);
    }

    //Raycasts forward to avoid icebergs
    private void AvoidIcebergs(ref Vector3 wantedVel)
    {
        RaycastHit hit;
        if (Physics.SphereCast(rb.position, avoidanceRadius, transform.forward, out hit, avoidanceRadius * 2f, LayerMask.GetMask("Iceberg")))
        {
            Vector3 hitNormal = hit.normal;
            hitNormal.y = 0f;
            wantedVel += hitNormal * avoidanceForce;
        }
    }

    //Avoid player within avoidance radius
    private void AvoidPlayer(ref Vector3 wantedVel)
    {
        Collider[] nearbyPlayers = Physics.OverlapSphere(rb.position, playerAvoidanceRadius * 2f, LayerMask.GetMask("Player"));
        foreach (var player in nearbyPlayers)
        {
            Vector3 forceV = rb.position - player.transform.position;
            forceV.y = 0f;
            float dist = forceV.magnitude;
            if (dist > 0f)
            {
                float forceMagnitude = 1.0f - (dist / (playerAvoidanceRadius * 2f));
                wantedVel += (forceV / dist) * forceMagnitude * playerAvoidanceForce;
            }
        }
    }

    //Detect collision with player
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Fire event with score
            OnFishCaught?.Invoke(scoreAmount);

            //Tell school to remove this fish
            if (school != null) { school.RemoveFish(this); }

            // Destroy this fish
            Destroy(gameObject);
        }
    }

    //Visualise player avoidance radius
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, playerAvoidanceRadius);
    }
}