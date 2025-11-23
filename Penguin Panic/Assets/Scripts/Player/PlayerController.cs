using System;
using UnityEngine;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    //Movement settings
    [Header("Movement Settings")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float swimSpeed = 10f;
    [SerializeField] private float rotationSpeed = 180f;

    //Drag settings
    [Header("Drag Settings")]
    [SerializeField] private float waterDrag = 1f;
    [SerializeField] private float iceDrag = 5f;
    
    //Input reader
    [Header("Input Settings")]
    [SerializeField] private InputReader inputReader;

    //Event
    public static event Action OnEnterWater;

    //Internal variables
    private bool isSwimming = false;
    private Vector2 movementDirection;
    private Rigidbody playerRigidbody;
    private IcebergController currentIceberg;

    public bool IsSwimming => isSwimming;

    private void Start()
    {
        inputReader.EnablePlayerActions();
        playerRigidbody = GetComponent<Rigidbody>();
    }

    //Enable and Disable input reader
    private void OnEnable()
    {
        inputReader.Move += Move;
    }

    private void OnDisable()
    {
        inputReader.Move -= Move;
        inputReader.DisablePlayerActions();
    }

    private void Move(Vector2 direction)
    {
        movementDirection = direction;
    }

    private void FixedUpdate()
    {
        float speed = isSwimming ? swimSpeed : walkSpeed;

        //Forward/backward movement
        float forwardInput = movementDirection.y;
        Vector3 forward = transform.forward;
        playerRigidbody.AddForce(forward * forwardInput * speed);

        //Rotation
        float turnInput = movementDirection.x;
        float rotationAmount = turnInput * rotationSpeed * Time.fixedDeltaTime;
        playerRigidbody.MoveRotation(playerRigidbody.rotation * Quaternion.Euler(0f, rotationAmount, 0f));

        //If on iceberg, move with it
        if (currentIceberg != null)
        {
            playerRigidbody.MovePosition(playerRigidbody.position + currentIceberg.Velocity * Time.fixedDeltaTime);
        }

        //Lock y position
        playerRigidbody.position = new Vector3(playerRigidbody.position.x, 0f, playerRigidbody.position.z);
    }

    //Trigger logic
    private void OnTriggerEnter(Collider other)
    {
        //switch to walking
        if (other.CompareTag("Iceberg"))
        {
            isSwimming = false;
            playerRigidbody.linearDamping = iceDrag;
            currentIceberg = other.GetComponent<IcebergController>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //switch to swimming
        if (other.CompareTag("Iceberg"))
        {
            isSwimming = true;
            playerRigidbody.linearDamping = waterDrag;
            currentIceberg = null;
            OnEnterWater?.Invoke();
        }
    }
}