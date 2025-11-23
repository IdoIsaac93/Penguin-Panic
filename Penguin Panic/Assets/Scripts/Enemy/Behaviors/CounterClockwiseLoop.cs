public class CounterClockwiseLoop : Behavior
{
    public override int GetNextPoint(int currentIndex)
    {
        // Move to the previous point in the path, looping back to the end if at the start
        if (currentIndex > 0)
            currentIndex--;
        else
            currentIndex = path.Length -1;
        return currentIndex;
    }
}
