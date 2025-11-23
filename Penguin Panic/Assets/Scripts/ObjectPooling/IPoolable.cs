public interface IPoolable
{
    // Called once when the object is first created
    void OnCreatedPool();

    // Called whenever the pool hands out this instance
    void OnSpawnFromPool();

    // Called when returning to pool
    void OnReturnToPool();
}