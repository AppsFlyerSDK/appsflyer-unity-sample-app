using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    #region Fields
    private Rigidbody2D _rb;
    private Vector2 _touchStart;
    private Vector2 _touchEnd;
    private Vector2 _touchDelta;

    private bool _fingerHadMoved;
    private float _turnRightBorder;
    private float _turnLeftBorder;
    private PlayerInteractions _playerInteractions;


    private bool _turnRight, _turnLeft, _jump, _fly;
    private bool _moveRight, _moveLeft;

    private bool _canTurnAndJump = true;

    private bool _isJumping;
    private bool _isFlying;
    #endregion

    #region Properties
    public bool IsJumping
    {
        get => _isJumping;
        set => _isJumping = value;
    }

    public bool IsFlying
    {
        get => _isFlying;
        set => _isFlying = value;
    }
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        transform.position = GameManager.GameStartPoint;
        _rb = GetComponent<Rigidbody2D>();
        _turnRightBorder = Screen.width / 2  + 100;
        _turnLeftBorder = Screen.width / 2 - 100;
        _playerInteractions = GetComponent<PlayerInteractions>();
    }

    private void Update()
    {
        _touchDelta = Vector2.zero;
        // movment inputs
        _turnRight = _turnLeft = _jump = _fly = false;
        _moveRight = _moveLeft = false;

        if (GameManager.GameOnPuase || TutorialManager.IsActiveSelf)
        {
            return;
        }

        // checks what is the current movement input - turn/move right, turn/move left, jump, fly
        CheckCurrentMovementInput();

        if (_turnRight)
        {
            transform.Rotate(new Vector3(0f, 0f, 90f));
        }
        else if (_turnLeft)
        {
            transform.Rotate(new Vector3(0f, 0f, -90f));
        }
        else if (_jump && !_isJumping && !GameManager.PlayerHasLost)
        {
            _isJumping = true;
        }
        else if (_fly && !_isFlying && !GameManager.PlayerHasLost)
        {
            if (_playerInteractions.CanFly)
            {

            _isFlying = true;

            }
        }
    }

    private void FixedUpdate()
    {
        if (_moveRight)
        {
            _rb.velocity = transform.up * GameManager.PlayerForwardSpeed + transform.right * GameManager.PlayerSidesSpeed;
        }

        else if (_moveLeft)
        {
            _rb.velocity = transform.up * GameManager.PlayerForwardSpeed + transform.right * -GameManager.PlayerSidesSpeed;
        }
        else
        {
            _rb.velocity = transform.up * GameManager.PlayerForwardSpeed;
        }
    
    }
    #endregion

    #region Methods
    public void Restart()
    {
        transform.position = GameManager.GameStartPoint;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        _isJumping = _isFlying = false;
    }

    private void CheckCurrentMovementInput()
    {
        if (Input.touchCount > 0 || Input.touches.Length > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Input.touches[0].position;
            switch (touch.phase)
            {
                case TouchPhase.Stationary:
                    //We transform the touch position into word space from screen space and store it.
                    Vector2 touchPosWorld = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);

                    Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);

                    //We now raycast with this information. If we have hit something we can process it.
                    RaycastHit2D hitInformation = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

                    if (hitInformation.collider != null)
                    {
                        //We should have hit something with a 2D Physics collider!
                        GameObject touchedObject = hitInformation.transform.gameObject;
                        if (touchedObject.CompareTag("Player Fly Trigger"))
                        {
                            _fly = true;
                            break;
                        }

                    }
                    if (touchPos.x >= _turnRightBorder)
                    {
                        _moveRight = true;
                    }
                    else if (touchPos.x <= _turnLeftBorder)
                    {
                        _moveLeft = true;
                    }
                    break;


                // detect if screen touches are 
                case TouchPhase.Began:
                    _touchStart = touchPos;
                    break;


                case TouchPhase.Ended:
                    if (_isFlying || !_canTurnAndJump || !_fingerHadMoved)
                    {
                        _fingerHadMoved = false;
                        break;
                    }
                    _touchEnd = touchPos;
                    _fingerHadMoved = false;

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
                    else if (_touchDelta.y < -(GameManager.ScreenUnitForSwip - 200))
                    {
                        _jump = true;
                    }

                    break;


                case TouchPhase.Moved:
                    _fingerHadMoved = true;
                    break;


                // detect if there is a long prees to tern right or left
                case TouchPhase.Canceled:
                    break;
            }
        }
    }
    #endregion
}
