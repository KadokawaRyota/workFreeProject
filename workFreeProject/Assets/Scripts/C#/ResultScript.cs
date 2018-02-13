using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultScript : MonoBehaviour {

    float time;

    [SerializeField]
    float titleTime;
	// Use this for initialization
	void Start () {
        time = 0;
	}
	
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime;

        if( time > titleTime )
        {
            time = 0;
            TitleSceneChange();
        }
	}

    public void TitleSceneChange()
    {
        SceneManager.LoadScene("Title");
    }
}
