using UnityEngine;

public class IcebergController : MonoBehaviour, IPoolable
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 0.5f;
    [Tooltip("Up (0,0,1), Down (0,0,-1), Right (1,0,0), Left (-1,0,0)")]
    [SerializeField] private Vector3 moveDirection = Vector3.back;

    [Header("Bounds")]
    [SerializeField] private float xBound = 40f;
    [SerializeField] private float zBound = 30f;

    public Vector3 Velocity => moveDirection.normalized * moveSpeed;

    private void FixedUpdate()
    {
        //Move iceberg
        transform.Translate(moveDirection.normalized * moveSpeed * Time.fixedDeltaTime, Space.World);

        //Lock y position
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        Vector3 pos = transform.position;
        pos.y = 0f;
        transform.position = pos;

        //Check bounds
        if (Mathf.Abs(transform.position.x) > xBound || Mathf.Abs(transform.position.z) > zBound)
        {
            ObjectPooler.Instance.ReturnToPool(gameObject);
        }
    }

    public void OnCreatedPool()
    {    }

    public void OnSpawnFromPool()
    {    }

    public void OnReturnToPool()
    {    }
}