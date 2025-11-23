using UnityEngine;

public class Sense : MonoBehaviour
{
    [SerializeField] protected float detectionRate = 1.0f;
    protected float elapsedTime = 0.0f;
    protected virtual void Initialize() { }
    protected virtual void UpdateSense() { }
    void Start()
    {
        Initialize();
    }
    void Update()
    {
        UpdateSense();
    }
}
