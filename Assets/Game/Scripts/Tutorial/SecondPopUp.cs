using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondPopUp : PopUp
{
    #region Fields
    private bool _turnRight, _turnLeft;
    private Vector2 _touchStart;
    private Vector2 _touchEnd;
    private Vector2 _touchDelta;
    private Rigidbody2D _rb;
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    private void Update()
    {
        _turnLeft = _turnRight = false;
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Input.touches[0].position;
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    _touchStart = touchPos;
                    break;


                case TouchPhase.Ended:
                    _touchEnd = touchPos;
                    // get the screen units delta
                    _touchDelta = _touchStart - _touchEnd;

                    if (_touchDelta.x < -GameManager.ScreenUnitForSwip)
                    {
                        _turnLeft = true;
                    }
                    else if (_touchDelta.x > GameManager.ScreenUnitForSwip)
                    {
                        _turnRight = true;
                    }
                    break;

            }
        }
        if (_turnRight)
        {
            transform.Rotate(new Vector3(0f, 0f, 90f));
        }
        else if (_turnLeft)
        {
            transform.Rotate(new Vector3(0f, 0f, -90f));
        }
    }

    private void OnEnable()
    {
        transform.localRotation = Quaternion.Euler(Vector3.zero);
    }
    #endregion
}

