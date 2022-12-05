using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData
{

    public bool isFirstLaunch;
    public int highestScore;
    public int currentLevel;




    public PlayerData(bool isFirstLaunch, int highestScore, int currentLevel = 1)
    {
        this.isFirstLaunch = isFirstLaunch;
        this.highestScore = highestScore;
        this.currentLevel = currentLevel;
    }
}
