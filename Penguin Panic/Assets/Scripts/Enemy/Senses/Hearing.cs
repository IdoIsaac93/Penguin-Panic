using UnityEngine;

public class Hearing : Sense
{
    [Header("Hearing Settings")]
    [SerializeField] private float hearingRadius = 10f;

    private OrcaFSM orcaFSM;

    protected override void Initialize()
    {
        //Get the Orca FSM
        orcaFSM = GetComponent<OrcaFSM>();
    }

    protected override void UpdateSense()
    {
        elapsedTime += Time.deltaTime;
        //Detect hearing sense if within the detection rate
        if (elapsedTime >= detectionRate)
        {
            Listen();
            elapsedTime = 0.0f;
        }
    }

    void Listen()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, hearingRadius);
        bool playerHeard = false;
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                playerHeard = true;
                break;
            }
        }
        orcaFSM.SetHearing(playerHeard);
    }

    //Draw hearing radius
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }
}