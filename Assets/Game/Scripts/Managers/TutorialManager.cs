using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TutorialManager : MonoBehaviour
{

    #region Inspector
    [SerializeField] private GameObject background;
    [SerializeField] private PopUp[] popUps;
    [SerializeField] private Button[] nextButtons;
    [SerializeField] private Button[] previousButtons;
    #endregion

    #region Feilds
    private int _currentPopupIndex;
    private static TutorialManager _instance;
    private bool _isActiveself;
    #endregion

    #region Properties
    public static bool IsActiveSelf { get => _instance._isActiveself; }
    #endregion
     
    #region MonoBehaviour
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;

        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
       
        foreach (Button button in nextButtons)
        {
            button.onClick.AddListener(Advance);
        }
        foreach (Button button in previousButtons)
        {
            button.onClick.AddListener(GoBack);
        }
       
    }

    private void FixedUpdate()
    {
        Vector3 camerapPosition = Camera.main.transform.position;
        transform.position = new Vector3(camerapPosition.x, camerapPosition.y, transform.position.z);
        transform.rotation = Camera.main.transform.rotation;
    }
    #endregion

    #region methods
    // go to the next popup
    private void Advance()
    {
        popUps[_currentPopupIndex].UnactivateSelf();
        _currentPopupIndex++;
        popUps[_currentPopupIndex].ActivateSelf();
    }

    // go to the previous popup
    private void GoBack()
    {
        popUps[_currentPopupIndex].UnactivateSelf();
        _currentPopupIndex--;
        popUps[_currentPopupIndex].ActivateSelf();
    }

    // starts the tutorial
    public static void Activate()
    {
        _instance.background.SetActive(true);
        _instance._isActiveself = true;
        _instance._currentPopupIndex = 0;
        _instance.popUps[_instance._currentPopupIndex].ActivateSelf();
    }

    // ends the tutorial
    public static void Unactivate()
    {
        _instance.background.SetActive(false);
        _instance._isActiveself = false;
        _instance.popUps[_instance._currentPopupIndex].UnactivateSelf();
    }
    #endregion
}
