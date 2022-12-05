using System.Collections;
#if UNITY_IOS
using Unity.Advertisement.IosSupport;
#endif
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    #region Inspector
        #region Start Screen
        // screens
            [SerializeField] private Image startScreen;
            [SerializeField] private Image gamePlayCanvas;
            [SerializeField] private Image TutorialScreen;
            [SerializeField] private Image conversionDataScreen;
            [SerializeField] private Image deepLinkParamsScreen;
            [SerializeField] private Image userInviteScreen;
            [SerializeField] private Image userInviteErrorScreen;
            [SerializeField] private Image flyAlertScreen;

            // buttons
            [SerializeField] private Button startPlayingButton;
            [SerializeField] private Button resumePlayingButton;
            [SerializeField] private Button showConversionDataButton;
            [SerializeField] private Button showDeepLinkParamsButton;
            [SerializeField] private Button showUserInviteButton;   
            [SerializeField] private Button hideConversionDataButton;
            [SerializeField] private Button hideDeepLinkParamsButton;
            [SerializeField] private Button hideUserInviteButton;
            [SerializeField] private Button shareUserInviteButton;
            [SerializeField] private Button hideUserInviteErrorButton;
            [SerializeField] private Button startToturialButton;
            [SerializeField] private Button endTutorialButton;

            // texts
            [SerializeField] private TMP_Text conversionDataText;
            [SerializeField] private TMP_Text DeepLinkParamsText;

            // input fields
            [SerializeField] private TMP_InputField referrerNameInput;
        #endregion

        #region Main Scene
             // buttons
            [SerializeField] private Button returnToStartScreenButton;
            [SerializeField] private Button addExtraButterfliesButton;

            // texts
            [SerializeField] private TMP_Text extraButterfliesCountText;
            [SerializeField] private TMP_Text scoreText;
            [SerializeField] private TMP_Text levelText;
        #endregion

        #region Player Lost Window
            // buttons
            [SerializeField] private Button playerLostButton;

            // texts
            [SerializeField] private TMP_Text currentScoreLostText;
            [SerializeField] private TMP_Text highestScoreLostText;

            // windows
            [SerializeField] private Image playerLostWindow;
        #endregion
    #endregion

    #region Fields
    private static UIManager _instance;
    #endregion


    #region MonoBehaviour
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
    
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        // request ATT for iOS
#if UNITY_IOS
         if (ATTrackingStatusBinding.GetAuthorizationTrackingStatus()
             == ATTrackingStatusBinding.AuthorizationTrackingStatus.NOT_DETERMINED)
                {
                    ATTrackingStatusBinding.RequestAuthorizationTracking();
                }
#endif

        // set the level tittle to cerrunt level
        _instance.levelText.text = GameManager.CurrentLevel.ToString();

        // When the game is loaded for the first time, show the start screen
        startScreen.gameObject.SetActive(true);

        // when the game is loaded, no need a resume button
        resumePlayingButton.gameObject.SetActive(false);

        // start/resume playing on click listener
        startPlayingButton.onClick.AddListener(StartPlaying);
        resumePlayingButton.onClick.AddListener(ResumePlaying);

        // return to start screen onclick listener
        returnToStartScreenButton.onClick.AddListener(PausePlaying);

        // Tutorial onClick listener
        startToturialButton.onClick.AddListener(StartTutorial);
        endTutorialButton.onClick.AddListener(EndTutorial);

        // conversion data onclick listeners
        showConversionDataButton.onClick.AddListener(ShowConversionDataScreen);
        hideConversionDataButton.onClick.AddListener(HideConversionDataScreen);

        // DeepLink parameters onclick listeners
        showDeepLinkParamsButton.onClick.AddListener(ShowDeepLinkParamsScreen);
        hideDeepLinkParamsButton.onClick.AddListener(HideDeepLinkParamsScreen);

        // user invite onclick listeners
        showUserInviteButton.onClick.AddListener(ShowUserInviteScreen);
        hideUserInviteButton.onClick.AddListener(HideUserInviteScreen);
        hideUserInviteErrorButton.onClick.AddListener(HideUserInviteShareErrorScreen);
        shareUserInviteButton.onClick.AddListener(ShareUserInviteLink);


        // onclick player lost button
        playerLostButton.onClick.AddListener(StartPlaying);

        // onclick extra butterflies
        addExtraButterfliesButton.onClick.AddListener(AddExtraButterflies);


    }
    #endregion

    #region Methods
    private void StartPlaying()
    {
        startScreen.gameObject.SetActive(false);
        playerLostWindow.gameObject.SetActive(false);
        GameManager.StartPlaying();
    }

    private void ResumePlaying()
    {
        startScreen.gameObject.SetActive(false);
        resumePlayingButton.gameObject.SetActive(false);
        GameManager.ResumePlaying();
    }

    private void PausePlaying()
    {
        startScreen.gameObject.SetActive(true);
        resumePlayingButton.gameObject.SetActive(true);
        GameManager.PausePlaying();
    }

    public static void ShowLoseWindow(string currentScore, string highestScore)
    {
        _instance.playerLostWindow.gameObject.SetActive(true);
        _instance.currentScoreLostText.text = currentScore;
        _instance.highestScoreLostText.text = highestScore;
        HideFlyAlert();
    }

    private void ShowConversionDataScreen()
    {
        conversionDataScreen.gameObject.SetActive(true);
        Dictionary<string, object> convertionData = GameManager.ConvertionData;
        string text = "Conversion Data Loading...";
        if (convertionData != null)
        {
            if (convertionData.ContainsKey("convertion_data_error"))
            {
                text = "Conversion Data Error.";
            }
            else
            {
                text = "";
                foreach (KeyValuePair<string, object> entry in convertionData)
                {
                    text += entry.Key + ": ";
                    if (entry.Value != null)
                    {
                        text += entry.Value.ToString() + '\n';
                    }
                    else
                    {
                        text += "null\n";
                    }
                }
            }
        }
        conversionDataText.text = text;
    }

    private void HideConversionDataScreen()
    {
        conversionDataScreen.gameObject.SetActive(false);
    }

    public static void ShowDeepLinkParamsScreen()
    {
        _instance.deepLinkParamsScreen.gameObject.SetActive(true);

        Dictionary<string, object> deepLinkParams = GameManager.DeepLinkParams;
        string text = "No bonuses have received yet. You can check again in a bit.";
        if (deepLinkParams != null)
        {
            string[] headlines = { "Start level", "Extra butterflies", "Extra points", "Referrer name"};
            if (deepLinkParams.ContainsKey("deep_link_error"))
            {
                text = "Bonuses Loading Error.";
            }
            else if (deepLinkParams.ContainsKey("deep_link_not_found"))
            {
                text = "Can Not Find Bonuses.";
            }
            else
            {
                int i = 0;
                text = "";
                foreach (KeyValuePair<string, object> entry in deepLinkParams)
                {
                    if (i < deepLinkParams.Count)
                    {
                        text += headlines[i] + ": ";
                        if (entry.Value != null)
                        {
                            text += entry.Value.ToString() + '\n';
                        }
                        else
                        {
                            text += "null\n";
                        }
                        i++;
                    }
                }
                if (i == 0)
                {
                    text = "Deep Link Received With No Bonuses.";
                }
            }
        }
        _instance.DeepLinkParamsText.text = text;
    }

    private void HideDeepLinkParamsScreen()
    {
        deepLinkParamsScreen.gameObject.SetActive(false);
        if (GameManager.GameOnPuase && !TutorialManager.IsActiveSelf)
        {
            GameManager.ResumePlaying();
        }
    }


    private void ShowUserInviteScreen()
    {
        userInviteScreen.gameObject.SetActive(true);
    }

    private void HideUserInviteScreen()
    {
        userInviteErrorScreen.gameObject.SetActive(false);
        userInviteScreen.gameObject.SetActive(false);
    }

    public void HideUserInviteShareErrorScreen()
    {
        userInviteErrorScreen.gameObject.SetActive(false);
    }

    private void ShareUserInviteLink()
    {
        string referrerName = referrerNameInput.text;
        GameManager.ShareUserIniviteLink(referrerName);
    }

    public static void SetExstraButterfliesText(string text)
    {
        _instance.extraButterfliesCountText.text = text;
    }

    private void AddExtraButterflies()
    {
        GameManager.AddExtraButterflies();
        extraButterfliesCountText.text = GameManager.ExtrabutterfliesCount.ToString();
    }

    public static void UpdateScore(string score) 
    {
        _instance.scoreText.text = score;
    }


    public static void UpdateLevel(string level)
    {
        _instance.levelText.text = level;
    }

    public static void ShowUserInviteShareErrorScreen()
    {
        _instance.userInviteErrorScreen.gameObject.SetActive(true);
    }

    public static void ShowFlyAlert()
    {
        _instance.flyAlertScreen.gameObject.SetActive(true);
    }

    public static void HideFlyAlert()
    {
        _instance.flyAlertScreen.gameObject.SetActive(false);
    }

    public static void StartTutorial()
    {
        _instance.gamePlayCanvas.gameObject.SetActive(false);
        _instance.TutorialScreen.gameObject.SetActive(true);
        _instance.playerLostWindow.gameObject.SetActive(false);
        TutorialManager.Activate();
        GameManager.PausePlaying();
    }

    public static void EndTutorial()
    {
        _instance.gamePlayCanvas.gameObject.SetActive(true);
        _instance.TutorialScreen.gameObject.SetActive(false);
        TutorialManager.Unactivate();
        GameManager.StartPlaying();
    }
    #endregion
}
