using UnityEngine;
public class Path : MonoBehaviour
{
    //Settings
    [SerializeField] private bool isDebug = true;

    //Waypoints
    public Transform[] waypoints;

    public int Length { get { return waypoints.Length; } }

    public Vector3 GetPoint(int index)
    {
        return waypoints[index].position;
    }

    //Draw the waypoints and connections
    void OnDrawGizmos()
    {
        if (!isDebug)
            return;
        for (int i = 1; i < waypoints.Length; i++)
        {
            Debug.DrawLine(waypoints[i - 1].position, waypoints[i].position, Color.red);
        }
        Debug.DrawLine(waypoints[this.Length - 1].position, waypoints[0].position,
        Color.red);
    }
}