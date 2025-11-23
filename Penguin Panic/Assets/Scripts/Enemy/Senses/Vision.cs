using UnityEngine;

public class Vision : Sense
{
    [Header("Vision Settings")]
    [SerializeField] private float FieldOfView = 45;
    [SerializeField] private float ViewDistance = 10f;
    [SerializeField] private Transform playerTrans;

    private OrcaFSM orcaFSM;

    protected override void Initialize()
    {
        //Find player position
        if (playerTrans == null) { playerTrans = GameManager.Instance.Player.transform;}
        //Get the Orca FSM
        orcaFSM = GetComponent<OrcaFSM>();
    }

    protected override void UpdateSense()
    {
        elapsedTime += Time.deltaTime;
        // Detect perspective sense if within the detection rate
        if (elapsedTime >= detectionRate)
        {
            Look();
            elapsedTime = 0.0f;
        }
    }

    void Look()
    {
        if (playerTrans == null) return;
        bool hasVision = false;

        //Direction from Orca to player
        Vector3 rayDirection = (playerTrans.position - transform.position);
        rayDirection.y = 0f;
        rayDirection.Normalize();

        //Check if within field of view
        float angle = Vector3.Angle(rayDirection, transform.forward);

        if (angle <= FieldOfView)
        {
            //Perform raycast
            int layerMask = LayerMask.GetMask("Player");
            RaycastHit hit;
            if (Physics.Raycast(transform.position, rayDirection, out hit, ViewDistance, layerMask))
            {
                if (hit.collider != null && hit.collider.CompareTag("Player"))
                {
                    hasVision = true;
                }
            }
        }
        orcaFSM.SetVision(hasVision);
    }

    //Draw field of view
    void OnDrawGizmos()
    {
        if (!Application.isEditor || playerTrans == null) return;

        Vector3 origin = transform.position;
        Vector3 forward = transform.forward * ViewDistance;

        // Draw line to player
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, playerTrans.position);

        // Left/right boundary rays
        Quaternion leftRot = Quaternion.Euler(0, -FieldOfView * 0.5f, 0);
        Quaternion rightRot = Quaternion.Euler(0, FieldOfView * 0.5f, 0);

        Vector3 leftRay = leftRot * forward;
        Vector3 rightRay = rightRot * forward;

        Gizmos.color = Color.green;
        Gizmos.DrawLine(origin, origin + forward);
        Gizmos.DrawLine(origin, origin + leftRay);
        Gizmos.DrawLine(origin, origin + rightRay);
    }
}