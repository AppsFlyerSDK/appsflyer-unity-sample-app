using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : MonoBehaviour
{
    #region Inspector
    [SerializeField] private float ascendingDuration;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private int flyingAndJumpingSortingLayer;
    [SerializeField] private float flyingScale;
    [SerializeField] private float jumpingScale;
    #endregion

    #region Fields
    private Animator _animator;
    private PlayerMovement _playerMovement;
    private PlayerInteractions _playerInteractions;
    private bool _isJumping;
    private bool _isFlying;
    private Vector3 _initialScale;
    #endregion

    #region Animation Tags
    public static int ShouldJump = Animator.StringToHash("Should Jump");
    public static int ShouldFly = Animator.StringToHash("Should Fly");
    public static int ShouldJumpFlower = Animator.StringToHash("Should Jump Flower");
    public static int ShouldRunFlower = Animator.StringToHash("Should Run Flower");
    #endregion

    #region MonoBehavior
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _playerMovement = transform.parent.GetComponent<PlayerMovement>();
        _playerInteractions = transform.parent.GetComponent<PlayerInteractions>();
    
        _initialScale = transform.localScale;
    }

    public void Update()
    {
        if (_playerMovement.IsFlying && !_isFlying)
        {
            _isFlying = true;
            _playerInteractions.StartFlying();
            StartCoroutine(FlyingAnimation());
            //_animator.SetTrigger(Fly);
        }

        else if (_playerMovement.IsJumping && !_isJumping)
        {
            _isJumping = true;
            StartCoroutine(JumpingAnimation());
            //_animator.SetTrigger(Jump);
        }
    }
    #endregion

    #region Methods
    public IEnumerator JumpingAnimation()
    {
        int tempSortingOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = flyingAndJumpingSortingLayer;
        int animation = ShouldJump;
        if (_playerInteractions.HasFlower)
        {
            animation = ShouldJumpFlower;
        }
        _animator.SetBool(animation, true);
        float halfJumpDuration = GameManager.JumpDuration / 2f;
        Vector3 startScaleSize = transform.localScale;
        float timer = 0;

        while (timer < halfJumpDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScaleSize, startScaleSize * jumpingScale, timer / halfJumpDuration);
            yield return null;
        }
        timer = 0;
        while (timer < halfJumpDuration)
        {
            timer += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScaleSize * jumpingScale, startScaleSize, timer / halfJumpDuration);
            yield return null;
        }
        transform.localScale = Vector3.one;
        _isJumping = false;
        _playerMovement.IsJumping = false;
        _animator.SetBool(animation, false);
        _playerInteractions.EndJumping();
        spriteRenderer.sortingOrder = tempSortingOrder;

    }

    public IEnumerator FlyingAnimation()
    {
        int tempSortingOrder = spriteRenderer.sortingOrder;
        spriteRenderer.sortingOrder = flyingAndJumpingSortingLayer;
        _animator.SetBool(ShouldFly, true);
        _animator.SetBool(ShouldRunFlower, false);
        float flyDuration = GameManager.FlyDuration;
        float timer = 0f;
        Vector3 initialScale = transform.localScale;
        while (timer <= ascendingDuration)
        {

            transform.localScale = Vector3.Lerp(initialScale, initialScale * flyingScale, timer / ascendingDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(flyDuration - ascendingDuration * 2);
        timer = 0f;
        while (timer <= ascendingDuration)
        {
            transform.localScale = Vector3.Lerp(initialScale * flyingScale, initialScale, timer / ascendingDuration);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.one;
        _isFlying = false;
        _playerMovement.IsFlying = false;
        _animator.SetBool(ShouldFly, false);
        _playerInteractions.EndFlying();
        spriteRenderer.sortingOrder = tempSortingOrder;

    }

    public void StartRunningWithFlowerAnimation()
    {
        _animator.SetBool(ShouldRunFlower, true);
    }
    
    public void Restart()
    {
        _isJumping = _isFlying = false;
        _animator.SetBool(ShouldFly, false);
        _animator.SetBool(ShouldJump, false);
        _animator.SetBool(ShouldJumpFlower, false);
        _animator.SetBool(ShouldRunFlower, false);
        transform.localScale = _initialScale;
    }
    #endregion
}
