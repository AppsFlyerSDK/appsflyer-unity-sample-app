using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterflyBehaviour : MonoBehaviour
{
    #region Inspector
    [SerializeField] private float catchTime;
    #endregion

    #region Fields
    private bool _wasChatched;
    private Vector3 _targetLocalPosition;
    #endregion

    #region Propreties
    public bool WasCatched
    {
        set => _wasChatched = value;
    }
    #endregion

    #region MonoBehaviour
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Flower"))
        {
            //if (!_wasChatched && !GameManager.PlayerIsJumpingOrFlying())
                if (!_wasChatched ) {
                ButterfliesTrailBehaviour butterfliesTrail = GameManager.ButterfliesTrail;
                if (butterfliesTrail.CanAddRegularButterflies)
                {
                    ButterflyBehaviour replacledButterfly = Instantiate(this, transform.position, Quaternion.identity);
                    replacledButterfly.gameObject.SetActive(false);
                    butterfliesTrail.AddButterfly(this);
                    _targetLocalPosition = butterfliesTrail.GetAvailablePosition();
                    MoveLocalyTo(transform.localPosition, _targetLocalPosition, butterfliesTrail.getRotatinPoint(),catchTime);
                    _wasChatched = true;
                }
            }
        }
    }
    #endregion

    #region Methods
    // moves the butterfly from start to end point in duration time while pointing to the localRotationPoint
    public void MoveLocalyTo( Vector3 startpoint, Vector3 endPoint, GameObject localRotationPoint, float duration)
    {
        StartCoroutine(MoveLocalyCoroutine( startpoint, endPoint, localRotationPoint, duration));

    }


    // the coroutine for the MoveLocalyTo method
    private IEnumerator MoveLocalyCoroutine( Vector3 startpoint, Vector3 endPoint, GameObject localRotationPoint, float duration)
    {
        float timer = 0f;
        Vector3 velocity = Vector3.zero;
        while (timer <= duration)
        {
            timer += Time.deltaTime;
            //newDirection = Vector3.RotateTowards(transform.up, targetRotation, Time.deltaTime, 0.0f);
            //transform.localRotation = Quaternion.LookRotation(newDirection);
            Vector3 current = transform.up;
            Vector3 to = localRotationPoint.transform.position - transform.position;
            transform.up = Vector3.RotateTowards(current, to, 2 * Time.deltaTime, 0.0f);

            transform.localPosition = Vector3.Lerp(startpoint, endPoint, timer / duration);
          
            //transform.localPosition = Vector3.SmoothDamp(transform.localPosition, endPoint, ref velocity, catchTime);
            yield return null;
        }
        transform.localPosition = endPoint;

    }

    // reset the animator of the butterfly
    public void ResetAnimation()
    {
        if (transform.Find("Animator").gameObject.activeSelf)
            transform.Find("Animator").GetComponent<Animator>().Play("FlyAnimation",0, Random.value);
    }
    #endregion
}
