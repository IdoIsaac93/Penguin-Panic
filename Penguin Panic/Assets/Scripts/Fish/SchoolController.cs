using System;
using System.Collections.Generic;
using UnityEngine;

public class SchoolController : MonoBehaviour
{
    [SerializeField] private Vector2 bound;
    [SerializeField] private float speed = 5.0f;
    [SerializeField] private float targetReachedRadius = 1.0f;

    [Header("Avoidance Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float avoidPlayerRadius = 5.0f;

    [Header("Fish Settings")]
    public List<FishController> fishList = new List<FishController>();

    //Scoring
    [Header("Scoring Variables")]
    [SerializeField] private int scoreAmount = 10;
    public static event Action<int> OnSchoolCaught;

    private Vector3 nextMovementPoint;
    private Rigidbody rb;
    private LevelManager levelManager;

    public LevelManager LevelManager { get => levelManager; set => levelManager = value; }

    private void Start()
    {
        //Get player
        if (player == null) { player = GameManager.Instance.Player.transform; }
        rb = GetComponent<Rigidbody>();
        CalculateNextMovementPoint();


        //Populate list with all child fish
        fishList.AddRange(GetComponentsInChildren<FishController>());
        foreach (var fish in fishList)
        {
            //Give each fish a reference to its school
            fish.SetSchool(this);
            fish.RebuildFishList(fishList);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = transform.forward * speed;

        //Rotate toward target
        Vector3 dir = nextMovementPoint - transform.position;
        dir.y = 0f;
        float angle = Vector3.SignedAngle(transform.forward, dir.normalized, Vector3.up);
        float rotationStep = angle * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, rotationStep, 0f));

        //Pick new point if close
        if (Vector3.Distance(nextMovementPoint, transform.position) <= targetReachedRadius)
        {
            CalculateNextMovementPoint();
        }

        //Lock y axis
        rb.position = new Vector3(rb.position.x, 0f, rb.position.z);
    }

    void CalculateNextMovementPoint()
    {
        //Generate random point within bounds
        float posX = UnityEngine.Random.Range(-bound.x, bound.x);
        float posZ = UnityEngine.Random.Range(-bound.y, bound.y);
        nextMovementPoint = new Vector3(posX, 0f, posZ);

        //Avoid player if too close
        Vector3 playerPos = player.position;
        playerPos.y = 0f;
        if (Vector3.Distance(nextMovementPoint, playerPos) <= avoidPlayerRadius)
        {
            CalculateNextMovementPoint();
        }
    }

    //Called by a fish when it is caught
    public void RemoveFish(FishController fish)
    {
        if (fishList.Contains(fish))
        {
            fishList.Remove(fish);

            //Rebuild lists in remaining fish
            foreach (var f in fishList)
            {
                f.RebuildFishList(fishList);
            }

            // Destroy school if empty
            if (fishList.Count == 0)
            {
                OnSchoolCaught?.Invoke(scoreAmount);
                levelManager.RemoveSchool(gameObject);
                Destroy(gameObject);
            }
        }
    }
}