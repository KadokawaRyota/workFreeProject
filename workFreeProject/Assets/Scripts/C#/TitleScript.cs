using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

//ipを取得する用。
using System.Net;
using System.Net.NetworkInformation;
using System.Linq;

#if NETFX_CORE
using Windows.Networking;
using Windows.Networking.Connectivity;
#endif

public class TitleScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OfflineSceneChange()
    {
        SceneManager.LoadScene("Offline");
    }

    public void OnlineSceneChangeHost()
    {
        GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().NetSettingData( "host" , GameObject.Find("Canvas/InputFieldIp").GetComponent<InputField>().text ,"7777" );

        //シーン移行寸前にinputFieldにあるipアドレスを保持する。
        GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().SetUseNetworkIp( GameObject.Find("Canvas/InputFieldIp").GetComponent<InputField>().text );
        SceneManager.LoadScene("Online");
    }

    public void OnlineSceneChangeClient()
    {
        GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().NetSettingData( "client" , GameObject.Find("Canvas/InputFieldIp").GetComponent<InputField>().text ,"7777" );
        //シーン移行寸前にinputFieldにあるipアドレスを保持する。
        GameObject.Find("NetworkSetter").GetComponent<NetworkSetter>().SetUseNetworkIp(GameObject.Find("Canvas/InputFieldIp").GetComponent<InputField>().text);
        SceneManager.LoadScene("Online");
    }

    public string GetIp()
    {
        string ipaddress = "";
        IPHostEntry ipentry = Dns.GetHostEntry(Dns.GetHostName());

        foreach (IPAddress ip in ipentry.AddressList)
        {
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ipaddress = ip.ToString();
                break;
            }
        }
        return ipaddress;
    }
}
