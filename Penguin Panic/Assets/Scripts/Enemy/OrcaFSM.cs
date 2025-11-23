using UnityEngine;

public class OrcaFSM : MonoBehaviour
{
    //References
    [SerializeField] private OrcaController controller;
    [SerializeField] private GameObject player;

    [Header("Probabilities")]
    [SerializeField] private float chanceToPatrol = 0.1f;
    [SerializeField] private float chanceToIdleFromPatrol = 0.05f;
    [SerializeField] private float chanceToChangeBehavior = 0.05f;

    [Header("Timers")]
    [SerializeField] private float visionLostTime = 3f;
    [SerializeField] private float circleDuration = 5f;

    //Timers
    private float statesTimer = 0f;
    private float sensesTimer = 0f;
    private float circleTimer = 0f;

    //Senses
    private bool hasVision = false;
    private bool hasHearing = false;

    //State machine
    private StateMachine stateMachine;

    private void Start()
    {
        //Get controller
        if (controller == null) { controller = GetComponent<OrcaController>(); }

        //Get player
        if (player == null) { player = GameManager.Instance.Player; }
        
        //States
        stateMachine = new StateMachine();

        //Idle
        var idle = stateMachine.CreateState("Idle");
        //Stop movement on enter
        idle.onEnter = () =>
        {
            controller.Stop();
            statesTimer = 0f;
        };
        idle.onStay = () =>
        {
            //Chase if sees or hears player
            if (hasVision || hasHearing)
            {
                stateMachine.TransitionTo("Chase");
                return;
            }

            //After 1 second, randomly change state
            statesTimer += Time.fixedDeltaTime;
            if (statesTimer >= 1f)
            {
                statesTimer = 0f;
                //Randomly patrol
                if (Random.value < chanceToPatrol)
                {
                    stateMachine.TransitionTo("Patrol");
                }
            }
        };

        //Patrol
        var patrol = stateMachine.CreateState("Patrol");
        patrol.onEnter = () => 
        {
            statesTimer = 0f;
        };
        patrol.onStay = () =>
        {
            controller.Patrol();

            //Chase if sees or hears player
            if (hasVision || hasHearing)
            {
                stateMachine.TransitionTo("Chase");
                return;
            }

            //After 1 second, randomly idle or change behavior
            statesTimer += Time.fixedDeltaTime;
            if (statesTimer >= 1f)
            {
                statesTimer = 0f;
                if (Random.value < chanceToIdleFromPatrol)
                    stateMachine.TransitionTo("Idle");
                if (Random.value < chanceToChangeBehavior)
                    controller.RandomizeBehavior();
            }
        };

        //Chase
        var chase = stateMachine.CreateState("Chase");
        chase.onStay = () =>
        {
            controller.Chase(player.transform);

            //Start counting vision lost time
            if (!hasVision && !hasHearing)
            {
                sensesTimer += Time.fixedDeltaTime;
                //Lose player after timer
                if (sensesTimer >= visionLostTime)
                {
                    sensesTimer = 0f;
                    stateMachine.TransitionTo(Random.value < 0.5f ? "Idle" : "Patrol");
                }
            }
            //Reset timer if player is seen or heard
            else sensesTimer = 0f;

            //Transition to Circle if player is not swimming
            var playerController = player.GetComponent<PlayerController>();
            if (playerController != null && !playerController.IsSwimming)
            {
                stateMachine.TransitionTo("Circle");
            }
        };

        //Circle
        var circle = stateMachine.CreateState("Circle");
        //Reset circle timer on enter
        circle.onEnter = () => circleTimer = 0f;
        circle.onStay = () =>
        {
            controller.CircleAround(player.transform);
            //Increment circle timer
            circleTimer += Time.fixedDeltaTime;
            if (circleTimer >= circleDuration)
            {
                stateMachine.TransitionTo(Random.value < 0.5f ? "Idle" : "Patrol");
            }
        };
    }

    private void FixedUpdate()
    {
        stateMachine.Update();
    }

    public void SetVision(bool vision) => hasVision = vision;
    public void SetHearing(bool hearing) => hasHearing = hearing;
}
