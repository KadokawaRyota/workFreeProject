using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public void OnlineSceneChange()
    {
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
