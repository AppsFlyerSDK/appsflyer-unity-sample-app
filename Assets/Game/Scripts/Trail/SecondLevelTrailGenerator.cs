using UnityEngine;

public class SecondLevelTrailGenerator : TrailGenerator
{
    #region Methods
    override protected float chooseRandomTrailRotation()
    {
        float rand = Random.value;
        if (rand < .4f)
            return 90;
        if (rand < .8)
            return -90;
        return 0;
    }

    override protected TrailPartBehaviour chooseNextTrailPart()
    {

        int index = 0;
        if (_currentPartIndex == trailPartsList.Count - 1 || _trailsCount % 8 != 0)
        {
            index = Random.Range(0, trailPartsList.Count - 1);
            while (index == _currentPartIndex)
            {
                index = Random.Range(0, trailPartsList.Count - 1);
            }
        }
        else
        {
            index = trailPartsList.Count - 1;
        }
        UnactivateAllpartsBeside(_currentPartIndex);
        _currentPartIndex = index;
        return trailPartsList[index];

    }
    #endregion
}
