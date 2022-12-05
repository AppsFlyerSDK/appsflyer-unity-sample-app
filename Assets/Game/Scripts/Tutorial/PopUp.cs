using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    #region Inspector
    [SerializeField] private Image popUpScreen;
    #endregion

    #region Methods
    public void ActivateSelf()
    {
        gameObject.SetActive(true);
        popUpScreen.gameObject.SetActive(true);
       

    }

    public void UnactivateSelf()
    {
        popUpScreen.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }
    #endregion
}
