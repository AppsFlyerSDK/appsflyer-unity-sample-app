using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPopUp :PopUp
{
    #region Fields
    private bool _jump, _isJumping;
    private Vector2 _touchStart;
    private Vector2 _touchEnd;
    private Vector2 _touchDelta;
    private Vector3 _startScaleSize;
    private Animator _animator;
    #endregion

    #region MonoBehaviour
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _startScaleSize = transform.localScale;
    }

    private void Update()
    {
        _jump = false;

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

                    if (_touchDelta.y < -(GameManager.ScreenUnitForSwip - 200))
                    {
                        _jump = true;
                    }
                    break;

            }
        }
        if (_jump && !_isJumping)
        {
            _isJumping = true;
            StartCoroutine(JumpingAnimation());
        }
    }

    private void OnEnable()
    {
        _startScaleSize = transform.localScale;
    }
    #endregion

    #region Methods

    public IEnumerator JumpingAnimation()
    {
        int animation = Animator.StringToHash("Should Jump"); ;
        
        _animator.SetBool(animation, true);
        float halfJumpDuration = 0.5f;
        float timer = 0;

        while (timer < halfJumpDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_startScaleSize, _startScaleSize * 1.5f, timer / halfJumpDuration);
            yield return null;
        }
        timer = 0;
        while (timer < halfJumpDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(_startScaleSize * 1.5f, _startScaleSize, timer / halfJumpDuration);
            yield return null;
        }
        _animator.SetBool(animation, false);
        _isJumping = false;
    }
    #endregion
}
