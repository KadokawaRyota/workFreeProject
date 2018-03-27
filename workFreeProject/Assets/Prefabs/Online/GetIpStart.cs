using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetIpStart : MonoBehaviour {

    GameObject TitleScript;

	// Use this for initialization
	void Start () {
        TitleScript = GameObject.Find("TitleScript");
        GetComponent<InputField>().text = TitleScript.GetComponent<TitleScript>().GetIp();
        if( GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().GetUseNetworkIp() != null )
        {
            GetComponent<InputField>().text = GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().GetUseNetworkIp();
        }
    }
}
