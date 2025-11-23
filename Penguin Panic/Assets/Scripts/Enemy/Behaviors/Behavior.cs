using UnityEngine;

public abstract class Behavior
{
    protected Path path;
    public void SetPath(Path newPath) { path = newPath; }
    public abstract int GetNextPoint(int currentIndex);
}