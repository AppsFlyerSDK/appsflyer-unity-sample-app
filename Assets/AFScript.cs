using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AppsFlyerSDK;


public class AFScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
         Debug.Log("start");
        // These fields are set from the editor so do not modify!
        //******************************//
        AppsFlyer.setIsDebug(true);
#if UNITY_WSA_10_0 && !UNITY_EDITOR
        AppsFlyer.initSDK("wdBuNGumMR6AwkhQYcJX7e", "", null);
#else
        AppsFlyer.initSDK("wdBuNGumMR6AwkhQYcJX7e", "1545848775", null);
#endif
        //******************************/
 
        AppsFlyer.startSDK();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
