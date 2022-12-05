using UnityEngine;

public class OffTrailSideBehaviour : MonoBehaviour
{

    #region Fields

    private TrailPartBehaviour _trailPart;
    #endregion


    #region MonoBehaviour

    private void Awake()
    {
        _trailPart = transform.parent.parent.parent.parent.GetComponent<TrailPartBehaviour>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            _trailPart.AddUnactivatedSideTrigger(this);
            gameObject.SetActive(false);
        }

    }


    #endregion
}
