using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowerBehaviour : MonoBehaviour
{
    private void Update()
    {
        if (GameManager.PlayerHasFlower && gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
    }
}
