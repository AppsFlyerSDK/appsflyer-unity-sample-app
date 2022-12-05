using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TrailGenerator : MonoBehaviour
{
    #region Inspector
    [SerializeField] protected TrailPartBehaviour trailPartStart;
    [SerializeField] protected Vector3 trailPartStartPosition;
    [SerializeField] protected List<TrailPartBehaviour> trailPartsList;
    [SerializeField] protected PlayerInteractions player;
    #endregion

    #region Fields
    protected Vector3 _lastTrailEndPosition;
    protected Vector3 _lastTrailAngle;
    protected int _currentPartIndex;
    protected int _trailsCount = 1;
    private bool _canSpawnTrials = true;
    #endregion

    #region Properties
    public int TrailsCount
    {
        get => _trailsCount;
    }

    public Vector3 LastTrailEndPosition
    {
        get => _lastTrailEndPosition;
    }

    public Vector3 LastTrailAngle
    {
        get => _lastTrailAngle;
    }

    public bool CanSpawnTrails
    {
        set => _canSpawnTrials = value;
    }
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        Restart();
    }

    private void Update()
    {
        if(Vector3.Distance(player.transform.position, _lastTrailEndPosition) < 15 &&
            _canSpawnTrials)
        {
            SpawnTrailPart();
        }
    }
    #endregion

    #region Methods
    protected abstract float chooseRandomTrailRotation();

    protected abstract TrailPartBehaviour chooseNextTrailPart();

    protected void SpawnTrailPart()
    {
        TrailPartBehaviour chosenLevelPart = chooseNextTrailPart();
        chosenLevelPart.gameObject.SetActive(true);
        chosenLevelPart.InIt(!player.HasFlower);
      


        // set the "new" part rotation
        float angle = chooseRandomTrailRotation();
        _lastTrailAngle.z += angle;
        chosenLevelPart.SetRotation(Quaternion.Euler(_lastTrailAngle));


        // set the "new" part position
        chosenLevelPart.SetPosition(_lastTrailEndPosition);
        _lastTrailEndPosition = chosenLevelPart.GetEndPointPosition();

        while (_lastTrailEndPosition.x > 85 || _lastTrailEndPosition.y > 85 || _lastTrailEndPosition.x < -85 || _lastTrailEndPosition.y < -85)
        {
            _lastTrailAngle.z -= angle;
            angle = chooseRandomTrailRotation();
            _lastTrailAngle.z += angle;
            chosenLevelPart.SetRotation(Quaternion.Euler(_lastTrailAngle));
            _lastTrailEndPosition = chosenLevelPart.GetEndPointPosition();
        }

        // substract the rotation of the last part of the trail to the angle
        // because rotation is counterclockwise and local rotation is clockwise
        _lastTrailAngle -= chosenLevelPart.GetEndPartRotation().eulerAngles;
        _trailsCount++;


    }

    protected void UnactivateAllpartsBeside(int firstIndex)
    {
        for (int i = 0; i < trailPartsList.Count; i++)
        {
            if (i == firstIndex)
                continue;

            trailPartsList[i].gameObject.SetActive(false);
        }
    }

    public void Restart()
    {
        foreach (TrailPartBehaviour trailPart in trailPartsList)
        {
            if (trailPart.gameObject.activeSelf)
            {
                trailPart.gameObject.SetActive(false);
            }
            trailPart.InIt(true);
        }
        trailPartStart.gameObject.SetActive(true);
        trailPartStart.SetPosition(trailPartStartPosition);
        _lastTrailEndPosition = trailPartStart.GetEndPointPosition();
        _lastTrailAngle = Vector3.zero;
        _currentPartIndex = 0;
        _trailsCount = 1;
        _canSpawnTrials = true;
    }

    #endregion
}
