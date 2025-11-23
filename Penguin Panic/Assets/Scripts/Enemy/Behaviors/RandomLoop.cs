using System.Collections.Generic;
using UnityEngine;

public class RandomLoop : Behavior
{
    private List<int> randomisedIndex;

    public override int GetNextPoint(int currentIndex)
    {
        //Initial randomisation
        if (randomisedIndex == null || randomisedIndex.Count == 0)
        {
            RandomisePath();
            //Start before the first index
            currentIndex = -1;
        }

        //Follow path and randomise when reaching end
        currentIndex++;
        if (currentIndex >= randomisedIndex.Count)
        {
            RandomisePath();
            currentIndex = 0;
        }

        return randomisedIndex[currentIndex];
    }

    //Randomise the path order
    private void RandomisePath()
    {
        randomisedIndex = new List<int>();

        for (int i = 0; i < path.Length; i++)
        {
            randomisedIndex.Add(i);
        }

        //Swap elements to randomise and avoid repeats
        for (int i = 0; i < randomisedIndex.Count; i++)
        {
            int temp = randomisedIndex[i];
            int randomIndex = Random.Range(i, randomisedIndex.Count);
            randomisedIndex[i] = randomisedIndex[randomIndex];
            randomisedIndex[randomIndex] = temp;
        }
    }
}