using System.Collections.Generic;
using System.Collections;
using System.IO;
using UnityEngine;



public class GameManager : MonoBehaviour
{
    #region Inspector
    // AppsFlyer object - the only communication with AppsFlyer
    [SerializeField] private AppsFlyerObjectScript appsFlyerObj;
    // the player
    [SerializeField] private GameObject player;
    // the generator of the first level
    [SerializeField] private FirstLevelTrailGenerator firstLevelGenerator;
    // the generator of the second level
    [SerializeField] private SecondLevelTrailGenerator secondLevelGenerator;
    // the max unites of the screen to detect a screen long press
    [SerializeField] private Vector3 gameStartPoint = Vector3.zero;
    // the number of trails to unlock for level achieved
    [SerializeField] private int trailsNumberForLevelAchieved;
    // the speed the player is moving forwards
    [SerializeField] private float initailForwardSpeed;
    // the speed the player is moving forwards
    [SerializeField] private float maxForwardSpeed;
    // the speed the player is moving forwards
    [SerializeField] private float sidesSpeed;
    // the time gap to for points to be added
    [SerializeField] private float pointsTimeGap;
    // time gap to get points
    [SerializeField] private float forwordSpeedAddition;
    // the min unites of the screen to detect a screen swip(right, left, up)
    [SerializeField] private float minScreenUnitsForSwip;
    // the initail distance the player needs to fly
    [SerializeField] private float initailFlyDistance;
    // the distance the player needs to jump
    [SerializeField] private float jumpDistance;
    #endregion

    #region Fields
    private static GameManager _instance;
    private ButterfliesTrailBehaviour _butterfliesTail;
    private PlayerMovement _playerMovement;
    private PlayerInteractions _playerInteractions;
    private TrailGenerator _trailGenerator;
    private Dictionary<PointsLevels, int> _points;
    private PlayerData _playerData;
    private PointsLevels _pointsLevel;
    private string _persistentpath = "";
    private bool _playerHasLost;
    private bool _isFirstLaunch = true;
    private bool _playerIsOnPause = true;
    private bool _displayedTutorialForFirstLaunch;
    private int _currentLevel = 1;
    private int _currentScore;
    private int _extraButterfliesCount;
    private float _midForwardSpeed;
    private float _playerForwardSpeed;
    private float _playerForwardSpeedBackup;
    private float _playerSidesSpeed;
    private float _pointsTimeGapCounter;
    private float _flyDistance;
    #endregion

    #region Properties
    public static Vector3 GameStartPoint { get => _instance.gameStartPoint; }
    public static ButterfliesTrailBehaviour ButterfliesTrail { get => _instance._butterfliesTail; }
    public static Dictionary<string, object> ConvertionData { get => _instance.appsFlyerObj.ConversionData; }
    public static Dictionary<string, object> DeepLinkParams { get => _instance.appsFlyerObj.DeepLinkParams; }
    public static bool PlayerHasLost { get => _instance._playerHasLost; }
    public static bool PlayerHasFlower { get => _instance._playerInteractions.HasFlower; }
    public static bool GameOnPuase { get => _instance._playerIsOnPause; }
    public static int CurrentLevel { get => _instance._currentLevel; set => _instance._currentLevel = value; }
    public static int ExtrabutterfliesCount { get => _instance._extraButterfliesCount; }
    public static float PlayerForwardSpeed { get => _instance._playerForwardSpeed; }
    public static float PlayerSidesSpeed { get => _instance._playerSidesSpeed; }
    public static float ScreenUnitForSwip { get => _instance.minScreenUnitsForSwip; }
    public static float FlyDuration { get => _instance._flyDistance / _instance._playerForwardSpeed; }
    public static float JumpDuration { get => _instance.jumpDistance / _instance._playerForwardSpeed; }
    #endregion

    #region MonoBehaviour
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            InIt();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        // check if the extra butterflies number of AppsFlyer object has changed
        if (appsFlyerObj.ExtraButterflies > 0)
        {
            _extraButterfliesCount += appsFlyerObj.ExtraButterflies;
            appsFlyerObj.ExtraButterflies = 0;
            UIManager.SetExstraButterfliesText(_extraButterfliesCount.ToString());
        }


        // check if the AppsFlyer object had received a deep link
        if (appsFlyerObj.DidReceivedDeepLink)
        {
            appsFlyerObj.DidReceivedDeepLink = false;
            if (!_playerIsOnPause)
            {
                PausePlaying();
            }

            UIManager.ShowDeepLinkParamsScreen();
        }

        // no need to update anything else if player lost or on puase
        if (_playerHasLost || _playerIsOnPause)
        {
            return;
        }

        // update points count
        _pointsTimeGapCounter += Time.deltaTime;
        if (_pointsTimeGapCounter >= pointsTimeGap)
        {
            UpdateScore();

            // update the forward speed each point gap
            if (_playerForwardSpeed < maxForwardSpeed)
            {
                _playerForwardSpeed += forwordSpeedAddition;
            }
        }

        // check if we the start level from AppsFlyer object has changed
        if (appsFlyerObj.StartLevel > CurrentLevel)
        {

            MoveToNextLevel(false);
        }

        // check if the player can move to the next level organically
        if (_trailGenerator.TrailsCount == trailsNumberForLevelAchieved && _currentLevel == 1)
        {
            MoveToNextLevel(true);
        }

    }

    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
        {
            // update the heighets score
            if (_instance._currentScore > _instance._playerData.highestScore)
            {
                _instance._playerData.highestScore = _instance._currentScore;
            }

            _instance._playerData.currentLevel = _instance._currentLevel;

            // save the current player data localy to the phone
            using (StreamWriter writer = new StreamWriter(_persistentpath))
            {
   
                string writeJson = JsonUtility.ToJson(_playerData);
                writer.Write(writeJson);
            }
        }
        else if (_playerData != null)
        {
            // check what level is higher, AppsFlyer object or the player data saved localy to the phone
            if (_playerData.currentLevel > appsFlyerObj.StartLevel)
            {
                _currentLevel = _playerData.currentLevel;
            }
            else
            {
                _currentLevel = appsFlyerObj.StartLevel;
            }
        }

        // if we upgarted a level, update the current level
        if (_currentLevel == 2)
        {
            _trailGenerator = secondLevelGenerator;
        }
    }

    #endregion

    #region Methods

        #region Static Methods
        // called when the player lost the current game
        public static void PlayerLost()
        {
            _instance._playerHasLost = true;
            _instance._playerForwardSpeed = 0f;
            _instance._playerSidesSpeed = 0f;


            if (_instance._currentScore > _instance._playerData.highestScore)
            {
                _instance._playerData.highestScore = _instance._currentScore;
            }

            UIManager.ShowLoseWindow(_instance._currentScore.ToString(), _instance._playerData.highestScore.ToString());
        }

        // called when the player starts playing a new game
        public static void StartPlaying()
        {
            _instance._playerMovement.Restart();
            _instance._playerInteractions.GetComponent<PlayerInteractions>().Restart();
            _instance.player.transform.Find("Animator").GetComponent<PlayerAnimations>().Restart();
            _instance._trailGenerator.Restart();
            _instance._pointsLevel = PointsLevels.Start;
            _instance._currentScore = 0;
            _instance._playerForwardSpeed = _instance.initailForwardSpeed;
            _instance._playerSidesSpeed = _instance.sidesSpeed;
            _instance._playerHasLost = false;
            _instance._playerIsOnPause = false;

            // display tutorail only for the first launch of the game
            if (_instance._playerData.isFirstLaunch)
            {
                UIManager.StartTutorial();
                _instance._playerData.isFirstLaunch = false;
            }
            else
            {
                // check if the extra points of AppsFlyer object has changed
                if (_instance.appsFlyerObj.ExtraPoints > 0)
                {
                _instance._currentScore += _instance.appsFlyerObj.ExtraPoints;
                _instance.appsFlyerObj.ExtraPoints = 0;
                }
            }
        }

        // called when player presses resume button
        public static void ResumePlaying()
        {
            _instance._playerForwardSpeed = _instance._playerForwardSpeedBackup;
            _instance._playerIsOnPause = false;

        }


        // called when player presses puase game button
        public static void PausePlaying()
        {

            _instance._playerForwardSpeedBackup = _instance._playerForwardSpeed;
            _instance._playerForwardSpeed = 0;
            _instance._playerIsOnPause = true;

        }

        // adding extra butterflies to the player according to _extraButterfliesCount
        public static void AddExtraButterflies()
            {
            // if player has the max amount of butterflies, return
            if (!_instance._butterfliesTail.CanAddExtraButterflies)
            {
                return;
            }

            // check how many available slots the butterflies trail has, and add extra butterflies accordingly
            int avialbleButterfliesCount = _instance._butterfliesTail.MissingButterfliesCount;
            if (_instance._extraButterfliesCount < avialbleButterfliesCount)
            {
                _instance._butterfliesTail.AddExtraButterflies(_instance._extraButterfliesCount);
                _instance._extraButterfliesCount = 0;
            }
            else
            {
                _instance._butterfliesTail.AddExtraButterflies(avialbleButterfliesCount);
                _instance._extraButterfliesCount -= avialbleButterfliesCount;
            }

        }

        // called when user presses 
        public static void ShareUserIniviteLink(string referrerName)
        {
            // start at level 1, with five extra butterflies, with no extra point, and give five extra buterflies to the inviter
            _instance.appsFlyerObj.generateAppsFlyerLink(referrerName, "1", "5", "0", 5);
            _instance.StartCoroutine(_instance.ShareLink(referrerName));

        }
    #endregion

        #region Private Methods
        // initializes the Game Manger Object
        private void InIt()
            {
                initializeConstantFields();

                initializePlayerData();

                InitializeCurrentLevel();
            }

            private  void initializeConstantFields()
            {
                _butterfliesTail = player.transform.Find("Butterflies Trail").GetComponent<ButterfliesTrailBehaviour>();
                _playerMovement = player.GetComponent<PlayerMovement>();
                _playerInteractions = player.GetComponent<PlayerInteractions>();
                _playerForwardSpeed = 0;
                _pointsLevel = PointsLevels.Start;
                _playerSidesSpeed = sidesSpeed;
                _midForwardSpeed = initailForwardSpeed + (maxForwardSpeed - initailForwardSpeed) / 2;
                _flyDistance = initailFlyDistance;
                _persistentpath = Application.persistentDataPath + Path.AltDirectorySeparatorChar + "SaveData.json";
                _points = new Dictionary<PointsLevels, int>()
                {
                    [PointsLevels.Start] = (int)PointsLevels.Start,
                    [PointsLevels.Mid] = (int)PointsLevels.Mid,
                    [PointsLevels.Extreme] = (int)PointsLevels.Extreme

                };
            }

            private void initializePlayerData()
            {
                try
                {
                    using (StreamReader reader = new StreamReader(_persistentpath))
                    {
                        string readJson = reader.ReadToEnd();
                        if (readJson == null || readJson == "")
                        {
                            using (StreamWriter writer = new StreamWriter(_persistentpath))
                            {
                                string writeJson = JsonUtility.ToJson(_playerData);
                                writer.Write(writeJson);
                            }
                        }
                        else
                        {
                            _playerData = JsonUtility.FromJson<PlayerData>(readJson);
                        }

                    }
                }
                catch (FileNotFoundException e)
                {
                    _playerData = new PlayerData(true, 0);
                }
            }

            private void InitializeCurrentLevel()
            {

                if (_playerData.currentLevel > appsFlyerObj.StartLevel)
                {
                    _currentLevel = _playerData.currentLevel;
                }
                else
                {
                    _currentLevel = appsFlyerObj.StartLevel;
                }

                if (_currentLevel == 1)
                {
                    _trailGenerator = firstLevelGenerator;
                    firstLevelGenerator.gameObject.SetActive(true);
                    secondLevelGenerator.gameObject.SetActive(false);
                }
                else
                {
                    _trailGenerator = secondLevelGenerator;
                    firstLevelGenerator.gameObject.SetActive(false);
                    secondLevelGenerator.gameObject.SetActive(true);
                }
            }

            // called every frame to update the score of the game
            private void UpdateScore()
            {
                _pointsTimeGapCounter = 0;
                _currentScore += _points[_pointsLevel];
                UIManager.UpdateScore(_currentScore.ToString());

                // check if we need to update the points level
                if (_midForwardSpeed <= _playerForwardSpeed && _playerForwardSpeed < maxForwardSpeed)
                {
                    _pointsLevel = PointsLevels.Mid;
                }
                else if (maxForwardSpeed <= _playerForwardSpeed)
                {
                    _pointsLevel = PointsLevels.Extreme;
                }
            }

            // called when player can move to the next level(level 2)
            private void MoveToNextLevel(bool organic)
            {

                // if the player has moved to the next level organically (with no AppsFlyer invovlment), set an In-App event
                if (organic)
                {
                    appsFlyerObj.SendLevelAchievedEvent(_currentLevel.ToString(), _currentScore.ToString());
                }

                // set game manger to level 2
                _playerData.currentLevel = _currentLevel = 2;
                secondLevelGenerator.gameObject.SetActive(true);
                firstLevelGenerator.gameObject.SetActive(false);
                _trailGenerator = secondLevelGenerator;
                UIManager.UpdateLevel(_currentLevel.ToString());

                // restarts
                _instance._playerMovement.Restart();
                _instance._playerInteractions.GetComponent<PlayerInteractions>().Restart();
                _instance.player.transform.Find("Animator").GetComponent<PlayerAnimations>().Restart();
      
            }

            // generates link for share
            private IEnumerator ShareLink(string referrerName)
            {
                yield return new WaitForSeconds(2);

                if (appsFlyerObj.UserInviteLink != null)
                {
                    new NativeShare()
                       .SetUrl(appsFlyerObj.UserInviteLink)
                       .SetText("Hello! " + referrerName + " has invited you to join ButterFlyer and get a spacial gift!\nClick on the link to download/open the game:")
                       .Share();
                }
                else
                {
                    UIManager.ShowUserInviteShareErrorScreen();
                }
        }
        #endregion

    #endregion
}
