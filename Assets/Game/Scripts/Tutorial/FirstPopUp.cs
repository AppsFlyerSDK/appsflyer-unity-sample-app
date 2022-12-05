using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPopUp : PopUp
{
    #region Fields
    private float _turnLeftBorder;
    private float _turnRightBorder;
    private bool _moveRight, _moveLeft;
    private Rigidbody2D _rb;
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        _turnLeftBorder = Screen.width / 2 - 100;
        _turnRightBorder = Screen.width / 2 + 100;
        _rb = GetComponent<Rigidbody2D>();
        transform.localPosition = new Vector3(0, -1, 0);
    }

    private void Update()
    {
        _moveRight = _moveLeft = false;
        if (Input.touches.Length > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Input.touches[0].position;
            switch (touch.phase)
            {
                case TouchPhase.Stationary:

                    if (touchPos.x >= _turnRightBorder)
                    {
                        _moveRight = true;
                    }
                    else if (touchPos.x <= _turnLeftBorder)
                    {
                        _moveLeft = true;
                    }
                    break;
            }
        }

    }

    private void OnEnable()
    {
        _turnLeftBorder = Screen.width / 2 - 200;
        _turnRightBorder = Screen.width / 2 + 200;
        _rb = GetComponent<Rigidbody2D>();
        transform.localPosition = new Vector3(0, -1, 0);
    }


    private void FixedUpdate()
    {
        if (_moveRight && transform.localPosition.x <= 2)
        {
            _rb.velocity = transform.up * GameManager.PlayerForwardSpeed + transform.right * GameManager.PlayerSidesSpeed;
        }
        else if (_moveLeft && transform.localPosition.x >= -2)
        {
            _rb.velocity = Vector3.zero + transform.right * -GameManager.PlayerSidesSpeed;
        }
        else
        {
            _rb.velocity = Vector3.zero;
        }
    }
}
#endregion