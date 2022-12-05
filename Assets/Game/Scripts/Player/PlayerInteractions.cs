using UnityEngine;

public class PlayerInteractions : MonoBehaviour
{
    #region Inspector
    [SerializeField] private ButterfliesTrailBehaviour butterfliesTrail;
    [SerializeField] private GameObject flower;
    [SerializeField] private Vector3 flowerPosition;
    #endregion

    #region Fields
    private PlayerMovement _playerMovement;
    private PlayerAnimations _playerAnimations;
    private bool _isOnPlatform;
    #endregion

    #region Properties
    public bool HasFlower
    {
        get => flower.activeSelf;
    }

    public bool CanFly
    {
        get => butterfliesTrail.CanFly;
    }
    #endregion

    #region MonoBehaviour 
    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _playerAnimations = transform.Find("Animator").GetComponent<PlayerAnimations>();
        flower.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Off Trail Side"))
        {
            _isOnPlatform = false;

            if (!_playerMovement.IsFlying && !_playerMovement.IsJumping)
            {
                GameManager.PlayerLost();
            }
        }

        if (collision.gameObject.CompareTag("Flower") && !_playerMovement.IsFlying && !_playerMovement.IsJumping && !HasFlower)
        {
            flower.SetActive(true);
            _playerAnimations.StartRunningWithFlowerAnimation();

        }
        else if (collision.gameObject.CompareTag("Platform"))
        {
            _isOnPlatform = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Platform"))
        {
            _isOnPlatform = true;
        }

        if (collision.gameObject.CompareTag("Fly Alert Trigger"))
        {
            UIManager.ShowFlyAlert();
        }

        if (collision.gameObject.CompareTag("Should Jump"))
        {
            if (!_playerMovement.IsJumping && !_playerMovement.IsFlying && !_isOnPlatform)
            {
                GameManager.PlayerLost();
            }
        }
        else if (collision.gameObject.CompareTag("Should Fly"))
        {
            if (!_playerMovement.IsFlying && !_isOnPlatform)
            {
                GameManager.PlayerLost();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Platform"))
        {
            _isOnPlatform = false;
        }
    }
    #endregion

    #region Methods
    public ButterfliesTrailBehaviour GetButterfliesTrail()
    {
        return butterfliesTrail;
;
    }

    public void StartFlying()
    {
        UIManager.HideFlyAlert();
        butterfliesTrail.StartFlying();
        print("i started flying");
    }

    public void EndFlying()
    {
        butterfliesTrail.EndFlying();
        flower.SetActive(false);
        if (!_isOnPlatform)
        {
            GameManager.PlayerLost();
        }
    }

    public void EndJumping()
    {
        if (!_isOnPlatform)
        {
            GameManager.PlayerLost();
        }
    }

    public void Restart()
    {
        butterfliesTrail.Restart();
        flower.SetActive(false);


    }
    #endregion
}
