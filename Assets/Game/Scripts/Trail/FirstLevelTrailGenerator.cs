using UnityEngine;

public class FirstLevelTrailGenerator : TrailGenerator {

    #region Methods
    override protected float chooseRandomTrailRotation()
    {
        float rand = Random.value;
        if (rand < .25f)
            return 90;
        if (rand < .5)
            return -90;
        return 0;
    }

    override protected TrailPartBehaviour chooseNextTrailPart()
    {

        int index = 0;
        if (_currentPartIndex == trailPartsList.Count - 1 || _trailsCount % 10 != 0)
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
