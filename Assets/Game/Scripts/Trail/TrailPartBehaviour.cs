using System.Collections.Generic;
using UnityEngine;

public class TrailPartBehaviour : MonoBehaviour
{
    #region Inspector
    [SerializeField] private GameObject butterflies;
    [SerializeField] private GameObject flowers;
    [SerializeField] private Transform endPart;
    [SerializeField] private Transform endPoint;
    #endregion


    #region Fields
    private List<OffTrailSideBehaviour> _unactivedSideTriggers;
    private List<GameObject> _butterflyList;
    private List<GameObject> _flowerList;
    private Quaternion _initailRotation;
    #endregion


    #region MonoBehaviour
    private void Start()
    {
        _initailRotation = transform.localRotation;
        InIt(true);
    }
    #endregion

    #region Methods
    public void InIt(bool canPlaceFlowers)
    {
        foreach (Transform butterfly in butterflies.transform)
        {
            float rand = Random.value;
            if (rand < .5f)
            {
                butterfly.gameObject.SetActive(true);
                butterfly.GetComponent<ButterflyBehaviour>().ResetAnimation();
            }
            else
                butterfly.gameObject.SetActive(false);
        }
        foreach (Transform flower in flowers.transform)
        {
     
            float rand = Random.value;
            if (rand < .333f)
            {
                flower.gameObject.SetActive(true);
            }
            else
                flower.gameObject.SetActive(false);
         
        }
        if (_unactivedSideTriggers != null)
        {
            foreach (OffTrailSideBehaviour sideTrigger in _unactivedSideTriggers)
            {
                sideTrigger.gameObject.SetActive(true);
            }
        }
        _unactivedSideTriggers = new List<OffTrailSideBehaviour>();
        transform.localRotation = _initailRotation; 
    }

    public void SetPosition(Vector3 newPos)
    {
        transform.position = newPos;
    }

    public void SetRotation(Quaternion newRot)
    {
        transform.rotation = newRot;
    }

    public Vector3 GetEndPointPosition()
    {
        return endPoint.position;
    }


    public Quaternion GetEndPartRotation()
    {
        return endPart.localRotation;
    }

    public void AddUnactivatedSideTrigger(OffTrailSideBehaviour sideTrigger)
    {
        _unactivedSideTriggers.Add(sideTrigger);
    }
    #endregion
}
