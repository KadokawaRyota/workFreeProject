using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class HostTimer : NetworkBehaviour {

    float timer = 0.0f;

    [SyncVar]
    float syncTimer;

    // Use this for initialization

    [Client]
    void Start () {
        timer = 0.0f;
    }

    // Update is called once per frame
    [Client]
    void Update () {
        timer += Time.deltaTime;

        CmdTimer();
    }

    [Command]
    void CmdTimer()
    {
        syncTimer = timer;
    }

    public float GetTime()
    {
        return syncTimer;
    }
}
