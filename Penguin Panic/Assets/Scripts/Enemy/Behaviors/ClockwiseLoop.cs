public class ClockwiseLoop : Behavior
{
    public override int GetNextPoint(int currentIndex)
    {
        // Move to the next point in the path, looping back to the start if at the end
        if (currentIndex < path.Length - 1)
            currentIndex++;
        else
            currentIndex = 0;
        return currentIndex;
    }
}
