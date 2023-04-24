using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;

// Add System namespace to use EventArgs
using System;


public class AppsFlyerObjectScript : MonoBehaviour , IAppsFlyerConversionData, IAppsFlyerUserInvite
{

    // These fields are set from the editor so do not modify!
    //******************************//
    public string devKey;
    public string appID;
    public string UWPAppID;
    public bool isDebug;
    public bool getConversionData;
    //******************************//

    #region AppsFlyer Related Fields

    private bool _didReceivedDeepLink; // marks if we got a DL and processed it
    private bool _deferred_deep_link_processed_flag; // only for Legacy Links users - marks if the Deffered DL was processed by UDL or not
    private string _userInviteLink;
    private Dictionary<string, object> _conversionDataDictionary;
    private Dictionary<string, object> _deepLinkParamsDictionary;

    #endregion

    #region AppsFlyer Related Properties
    
    public string UserInviteLink
    {
        get => _userInviteLink;
    }

    public Dictionary<string, object> ConversionData
    {
        get => _conversionDataDictionary;
    }

    public Dictionary<string, object> DeepLinkParams
    {
        get => _deepLinkParamsDictionary;
    }

    #endregion

    #region Game Mechanics Fields

    private int _startLevel = 1;
    private int _extraButterfliesBonus;
    private int _useInviteBextraButterfliesBonus;
    private int _extraPointsBonus;

    #endregion

    #region Game Mechanics Properties

    public int ExtraButterflies
        {
            get => _extraButterfliesBonus;
            set => _extraButterfliesBonus = value;
        }

        public int ExtraPoints
        {
            get => _extraPointsBonus;
            set => _extraPointsBonus = value;
        }

        public bool DidReceivedDeepLink
        {
            get => _didReceivedDeepLink;
            set => _didReceivedDeepLink = value;
        }

        public int StartLevel
        {
            get => _startLevel;
        }

    #endregion

    void Start()
    {

        Debug.Log("start");
        AppsFlyer.setIsDebug(isDebug);
        AppsFlyer.initSDK(devKey, appID, getConversionData ? this : null);

        //******************************/

        // set a custom method to handle deep link received - only on deep linking implementation
        AppsFlyer.OnDeepLinkReceived += OnDeepLink;

        // set up the one link ID for the user invite - only on user invite implementation
        AppsFlyer.setAppInviteOneLinkID("45rv");

        // App Tracking Transparency for iOS
#if UNITY_IOS && !UNITY_EDITOR
        AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
         /** if you are using v6.6.0+ use this call instead:
         *   AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
         */
#endif

        // start the SDK
        AppsFlyer.startSDK();
    }


    #region Conversion Data

    public void onConversionDataSuccess(string conversionData)
    {
        AppsFlyer.AFLog("didReceiveConversionData", conversionData);
        Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
        _conversionDataDictionary = conversionDataDictionary;


        /** This section is for Legacy Links Users Only. if you don't use it you can skip this implementation **/

        // af_status and is_first_launch always received with onConversionDataSuccess, no need to check if the keys exist
        string afStatus = _conversionDataDictionary["af_status"].ToString();
        bool isFirstLaunch = (bool)_conversionDataDictionary["is_first_launch"];

        // check if we got Deferred Deep Link
        if (afStatus == "Non-organic" && isFirstLaunch)
        {
            // check if the Deferred Deep Link was processed by the onDeepLink(UDL)
            if (!_deferred_deep_link_processed_flag)
            {
                // if not, process the custom param("start_level") that replaced the deep_link_value
                // additional params can be processed the same way
                _deepLinkParamsDictionary = new Dictionary<string, object>(); // reset the DL params from the previous DL processing

                if (_conversionDataDictionary.TryGetValue("start_level", out var startLevelObj))
                {
                    if (int.TryParse(startLevelObj?.ToString(), out var startLevel))
                    {
                        _startLevel = startLevel;
                        _deepLinkParamsDictionary.Add("Start Level", _startLevel);
                        _didReceivedDeepLink = true;
                    }
                }
            }
            else
            {
                // if it was, no need for further processing
                _deferred_deep_link_processed_flag = false;
            }
        }
    }

    public void onConversionDataFail(string error)
    {
        AppsFlyer.AFLog("didReceiveConversionDataWithError", error);
        // If we dont get the convertion data set the field to null
        _conversionDataDictionary = new Dictionary<string, object>()
        {
            ["convertion_data_error"] = true
        };
    }

    public void onAppOpenAttribution(string attributionData)
    {
        AppsFlyer.AFLog("onAppOpenAttribution", attributionData);
        // add direct deeplink logic here
    }

    public void onAppOpenAttributionFailure(string error)
    {
        AppsFlyer.AFLog("onAppOpenAttributionFailure", error);
    }

    #endregion


    #region Deep Link

    /** All the DeepLink handling has to be done under the onDeepLink handler **/
    void OnDeepLink(object sender, EventArgs args)
    {
        if (!(args is DeepLinkEventsArgs deepLinkEventArgs)) return;

        AppsFlyer.AFLog("DeepLink Status", deepLinkEventArgs.status.ToString());

        switch (deepLinkEventArgs.status)
        {
            case DeepLinkStatus.FOUND:

                _didReceivedDeepLink = true;

                if (deepLinkEventArgs.isDeferred())
                {
                    AppsFlyer.AFLog("OnDeepLink", "This is a deferred deep link");
                    // **only for Legacy Links users**
                    // lets the onConversionDataSuccess know we got the Deferred Deep Link and assume we can process it
                    // this can be changed later on if we got an Extended Deferred DeepLinking that can not be processed by UDL
                    // we will know the type of the DDL only on the ParseDeepLinkParams() method
                    _deferred_deep_link_processed_flag = true;
                }
                else
                {
                    AppsFlyer.AFLog("OnDeepLink", "This is a direct deep link");
                }

                var deepLinkParamsDictionary = GetDeepLinkParamsDictionary(deepLinkEventArgs);
                if (deepLinkParamsDictionary != null)
                {
                    ParseDeepLinkParams(deepLinkParamsDictionary);
                }
                break;

            case DeepLinkStatus.NOT_FOUND:
                AppsFlyer.AFLog("OnDeepLink", "Deep link not found");

                _deepLinkParamsDictionary = new Dictionary<string, object>()
                {
                    ["deep_link_not_found"] = true
                };

                break;

            default:
                AppsFlyer.AFLog("OnDeepLink", "Deep link error");

                _deepLinkParamsDictionary = new Dictionary<string, object>()
                {
                    ["deep_link_error"] = true
                };

                break;
        }
    }


    /** Get the DeepLink params depending on the device OS **/
    private Dictionary<string, object> GetDeepLinkParamsDictionary(DeepLinkEventsArgs deepLinkEventArgs)
    {
#if UNITY_IOS && !UNITY_EDITOR
    if (deepLinkEventArgs.deepLink.ContainsKey("click_event") && deepLinkEventArgs.deepLink["click_event"] != null)
    {
        return deepLinkEventArgs.deepLink["click_event"] as Dictionary<string, object>;
    }
#elif UNITY_ANDROID && !UNITY_EDITOR
    return deepLinkEventArgs.deepLink;
#endif

        return null;
    }


    /**
    Parse the DeepLink params, according to the conventions between the campaign manager and the developer. 
    Both have to agree on the meaning of each key of the Deep Link parameters.

    In this app:
    deep_link_value is the start level
    deep_link_sub1 is the quantity of the extra butterflies
    deep_link_sub2 is the extra points
    deep_link_sub3 is the referrer name if the link was generated using UserInvite

    If you don't want/can't use deep_link_value, you can add a custom param.
    the campaign manager and the developer agreed on the param 'start_level'as the deep link value param,
    instead of deep_link_value.
    **/
    private void ParseDeepLinkParams(Dictionary<string, object> deepLinkParamsDictionary)
    {
        _deepLinkParamsDictionary = new Dictionary<string, object>(); // reset the DL params from the previous DL processing

        // check if we got deep_link_value and its not null
        if (deepLinkParamsDictionary.TryGetValue("deep_link_value", out var deepLinkValueObj) && int.TryParse(deepLinkValueObj?.ToString(), out var deepLinkValue))
        {
            _startLevel = deepLinkValue;
            _deepLinkParamsDictionary.Add("Start Level", _startLevel);
        }
        // **only for Legacy Links users** if not, search for the custom param that replaces the deep_link_value.
        else if (deepLinkParamsDictionary.TryGetValue("start_level", out var startLevelObj) && int.TryParse(startLevelObj?.ToString(), out var startLevel))
        {
            _startLevel = startLevel;
            _deepLinkParamsDictionary.Add("Start Level", _startLevel);
        }
        // **only for Legacy Links users**
        else
        {
            // onDeepLink(UDL) cant handle Extended Deferred DeepLinking, mark to the onConversionDataSuccess to process it
            _deferred_deep_link_processed_flag = false;
        }
        // check for others DeepLink params
        if (deepLinkParamsDictionary.TryGetValue("deep_link_sub1", out var extraButterfliesBonusObj))
        {
            if (int.TryParse(extraButterfliesBonusObj?.ToString(), out var extraButterfliesBonus))
            {
                _extraButterfliesBonus = extraButterfliesBonus;
                _deepLinkParamsDictionary.Add("Extra Butterflies", _extraButterfliesBonus);
            }
        }

        if (deepLinkParamsDictionary.TryGetValue("deep_link_sub2", out var extraPointsBonusObj))
        {
            if (int.TryParse(extraPointsBonusObj?.ToString(), out var extraPointsBonus))
            {
                _extraPointsBonus = extraPointsBonus;
                _deepLinkParamsDictionary.Add("Extra Points", _extraPointsBonus);
            }
        }

        if (deepLinkParamsDictionary.TryGetValue("deep_link_sub3", out var referrerNameObj))
        {
            var referrerName = referrerNameObj?.ToString();
            _deepLinkParamsDictionary.Add("Referrer Name", referrerName);
        }
    }

    #endregion


    #region In-App Events

    /** custom method to generate User Invite link **/
    public void SendLevelAchievedEvent(string levelName, string score)
        {

            Dictionary<string, string> levelAchievedEvent = new
            Dictionary<string, string>();
            levelAchievedEvent.Add(AFInAppEvents.LEVEL, levelName);
            levelAchievedEvent.Add(AFInAppEvents.SCORE, score);
            AppsFlyer.sendEvent(AFInAppEvents.LEVEL_ACHIEVED, levelAchievedEvent);

        }

#endregion


#region User Invite

    /** custom method to generate User Invite link **/
    public void generateAppsFlyerLink(string referrerName, string deepLinkValue, string deepLinkSub1, string deepLinkSub2, int currentUserInviteExtraButterflies)
    {
        _userInviteLink = null;
        _useInviteBextraButterfliesBonus = currentUserInviteExtraButterflies;
        Dictionary<string, string> parameters = new Dictionary<string, string>();
        parameters.Add("channel", "Unity_Native");
        parameters.Add("campaign", "invite_and_get_five_free_butterflies");
        parameters.Add("deep_link_value", deepLinkValue);
        parameters.Add("deep_link_sub1", deepLinkSub1);
        parameters.Add("deep_link_sub2", deepLinkSub2);
        parameters.Add("deep_link_sub3", referrerName);


        // other params
        //parameters.Add("referrerImageUrl", "some_referrerImageUrl");
        //parameters.Add("customerID", "some_customerID");
        //parameters.Add("baseDeepLink", "some_baseDeepLink");
        //parameters.Add("brandDomain", "some_brandDomain");
        AppsFlyer.generateUserInviteLink(parameters, this);
    }

    public void onInviteLinkGenerated(string link)
    {
        AppsFlyer.AFLog("onInviteLinkGenerated", link);
        _userInviteLink = link;
        _extraButterfliesBonus += _useInviteBextraButterfliesBonus;

    }

    public void onInviteLinkGeneratedFailure(string error)
    {
        AppsFlyer.AFLog("onInviteLinkGeneratedFailure", error);
        _userInviteLink = null;
    }

    public void onOpenStoreLinkGenerated(string link)
    {
        AppsFlyer.AFLog("onOpenStoreLinkGenerated", link);
    }

#endregion
}
