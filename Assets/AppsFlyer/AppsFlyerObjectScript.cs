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


    #region Game Mechanics Fields

        private int _startLevel = 1;
        private int _extraButterfliesBonus;
        private int _useInviteBextraButterfliesBonus;
        private int _extraPointsBonus;
        private bool _didReceivedDeepLink;
        private string _userInviteLink;
        private Dictionary<string, object> _conversionDataDictionary;
        private Dictionary<string, object> _deepLinkParamsDictionary;

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
        AppsFlyeriOS.waitForATTUserAuthorizationWithTimeoutInterval(60);
         /** if you are using v6.6.0+ use:
         *   AppsFlyer.waitForATTUserAuthorizationWithTimeoutInterval(60);
         *   instead.
         */
#endif

        // start the SDK
        AppsFlyer.startSDK();
   
       
    }


#region Convertion Data

        public void onConversionDataSuccess(string conversionData)
        {
            AppsFlyer.AFLog("didReceiveConversionData", conversionData);
            Dictionary<string, object> conversionDataDictionary = AppsFlyer.CallbackStringToDictionary(conversionData);
            _conversionDataDictionary = conversionDataDictionary;
           
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

        void OnDeepLink(object sender, EventArgs args)
        {
            var deepLinkEventArgs = args as DeepLinkEventsArgs;
            AppsFlyer.AFLog("DeepLink Status", deepLinkEventArgs.status.ToString());
            switch (deepLinkEventArgs.status)
                {
                    case DeepLinkStatus.FOUND:

                        _didReceivedDeepLink = true;

                        if (deepLinkEventArgs.isDeferred())
                        {
                            AppsFlyer.AFLog("OnDeepLink", "This is a deferred deep link");
                        }
                        else
                        {
                            AppsFlyer.AFLog("OnDeepLink", "This is a direct deep link");
                        }

                        Dictionary<string, object> deepLinkParamsDictionary = null;

                        #if UNITY_IOS && !UNITY_EDITOR
                        if (deepLinkEventArgs.deepLink.ContainsKey("click_event") && deepLinkEventArgs.deepLink["click_event"] != null)
                        {
                            deepLinkParamsDictionary = deepLinkEventArgs.deepLink["click_event"] as Dictionary<string, object>;
                        }
                        #elif UNITY_ANDROID && !UNITY_EDITOR
                        deepLinkParamsDictionary = deepLinkEventArgs.deepLink;
                        #endif

                        // the campaign manager and the developer have to agree on the meaning for each key of the Deep Link parameters
                        // In this app:
                        // deep_link_value is the start level
                        // deep_link_sub1 is the quantity of the extra buterflies
                        // deep_link_sub2 is the extra points
                        // deep_link_sub3 is the referrer name if the link is a user invite link

                        if (deepLinkParamsDictionary != null)
                        {
                                _deepLinkParamsDictionary = new Dictionary<string, object>();
                            if (deepLinkParamsDictionary.ContainsKey("deep_link_value"))
                            {
                                    _startLevel = int.Parse(deepLinkParamsDictionary["deep_link_value"].ToString());
                                    _deepLinkParamsDictionary.Add("deep_link_value", _startLevel);
                            }
                            if (deepLinkParamsDictionary.ContainsKey("deep_link_sub1"))
                            {
                                _extraButterfliesBonus = int.Parse(deepLinkParamsDictionary["deep_link_sub1"].ToString());
                                _deepLinkParamsDictionary.Add("deep_link_sub1", _extraButterfliesBonus);
                            }
                            if (deepLinkParamsDictionary.ContainsKey("deep_link_sub2"))
                            {
                                _extraPointsBonus = int.Parse(deepLinkParamsDictionary["deep_link_sub2"].ToString());
                                _deepLinkParamsDictionary.Add("deep_link_sub2", _extraPointsBonus);
                            }
                            if (deepLinkParamsDictionary.ContainsKey("deep_link_sub3"))
                            {
                                var referrerName = deepLinkParamsDictionary["deep_link_sub3"].ToString();
                                _deepLinkParamsDictionary.Add("referrerName", referrerName);
                            }
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

#endregion


#region In App Events
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
