using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterfliesTrailBehaviour : MonoBehaviour
{
    #region Inspector
    [SerializeField] private GameObject butterfliesRotationPoint;
    [SerializeField] private GameObject butterfliesPositions;
    [SerializeField] private GameObject offScreenButterfliesPositions;
    [SerializeField] private GameObject butterfliesFlyingPositions;
    [SerializeField] private GameObject butterflies;
    [SerializeField] private ButterflyBehaviour butterfly;
    [SerializeField] private float helpFlyDuration;
    [SerializeField] private float flyFromOffScreenDuration;
    #endregion

    #region Fields
    private List<GameObject> _butterfliesPositionList;
    private List<GameObject> _offScreenButterfliesPositionList;
    private List<GameObject> _butterfliesFlyingPositionList;
    private List<ButterflyBehaviour> _butterfliesList;
    private PlayerInteractions _playerInteractions;
    private int _lastButterflyPosIndex;
    #endregion

    #region Properties
    public bool CanAddRegularButterflies
    {
        get => _butterfliesList.Count < _butterfliesPositionList.Count && _playerInteractions.HasFlower;
    }

    public bool CanAddExtraButterflies
    {
        get => _butterfliesList.Count < _butterfliesPositionList.Count;
    }


    public int MissingButterfliesCount
    {
        get => _butterfliesPositionList.Count - _butterfliesList.Count;
    }

    public bool CanFly
    {
        get => _butterfliesList.Count == 10;
    }
    #endregion

    #region MonoBehaviour

    private void Start()
    {
        _butterfliesPositionList = new List<GameObject>();
        _offScreenButterfliesPositionList = new List<GameObject>();
        _butterfliesFlyingPositionList = new List<GameObject>();
        _butterfliesList = new List<ButterflyBehaviour>();
        _playerInteractions = transform.parent.GetComponent<PlayerInteractions>();
        foreach (Transform position in butterfliesPositions.transform)
        {
            _butterfliesPositionList.Add(position.gameObject);
        }

        foreach (Transform position in offScreenButterfliesPositions.transform)
        {
            _offScreenButterfliesPositionList.Add(position.gameObject);
        }

        foreach (Transform position in butterfliesFlyingPositions.transform)
        {
            _butterfliesFlyingPositionList.Add(position.gameObject);
        }
    }

    #endregion


    #region Methods

    // returns the avalible position for a new butterfly on the butterflies trail
    public Vector3 GetAvailablePosition()
    {

        int cueIndex = _lastButterflyPosIndex;
        if (_lastButterflyPosIndex < _butterfliesPositionList.Count - 1)
            _lastButterflyPosIndex++;
        return _butterfliesPositionList[cueIndex].transform.localPosition;
    }

    // adds one butterfly to the trail's butterflies list
    public void AddButterfly(ButterflyBehaviour butterfly)
    {
        butterfly.transform.parent = butterflies.transform;
        _butterfliesList.Add(butterfly);
    }

    // starts the buterflies movement to help the player fly
    public void StartFlying()
    {
        _lastButterflyPosIndex = 0;
        for (int i = 0; i < _butterfliesList.Count; i++)
        {
            Vector3 targetFlyPos = _butterfliesFlyingPositionList[i].transform.localPosition;
            _butterfliesList[i].MoveLocalyTo(_butterfliesList[i].transform.localPosition, targetFlyPos, getRotatinPoint(), helpFlyDuration);
        }
    }

    // ends the help the butterflies give to the player, and realses the butterflies outside of the screen
    public void EndFlying()
    {
        for (int i = 0; i < _butterfliesList.Count; i++)
        {
            Vector3 targetFlyPos = _offScreenButterfliesPositionList[i].transform.localPosition;
            _butterfliesList[i].MoveLocalyTo(_butterfliesList[i].transform.localPosition, targetFlyPos, _offScreenButterfliesPositionList[i], flyFromOffScreenDuration);
        }

        _butterfliesList.Clear();

    }

    // add extra -- quantity -- buterflies to the buterflies trail 
    public void AddExtraButterflies(int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            ButterflyBehaviour newButterfly = Instantiate(butterfly,
                _offScreenButterfliesPositionList[i].transform.position,
                Quaternion.identity).GetComponent<ButterflyBehaviour>();
            newButterfly.ResetAnimation();
            newButterfly.transform.parent = butterflies.transform;
            _butterfliesList.Add(newButterfly);
            newButterfly.WasCatched = true;
            Vector3 targetFlyPos = GetAvailablePosition();
            newButterfly.MoveLocalyTo( newButterfly.transform.localPosition, targetFlyPos, getRotatinPoint(), flyFromOffScreenDuration);
        }

    }

    // restart the butterflies trail to a quantity of 0 butterflies
    public void Restart()
    {
        foreach (ButterflyBehaviour butterfly in _butterfliesList)
        {
            Destroy(butterfly.gameObject);
        }
        _butterfliesList.Clear();
       _lastButterflyPosIndex = 0;
    }


    // returns the rotation point of the player - indecate witch direction the player is currently looking at
    public GameObject getRotatinPoint()
    {
        return butterfliesRotationPoint;
    }

    #endregion
}
