using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class resultText : MonoBehaviour {

    [SerializeField]
    public Text Runk;
    [SerializeField]
    public Text Score;

    ResultManagerScript resultManager;

    // Use this for initialization
    void Start () {
        resultManager = GameObject.Find("resultManager").GetComponent<ResultManagerScript>();

        int runk = resultManager.GetRunk();
        int score = resultManager.GetScore();


        Score.text = score.ToString() + "点";
        Runk.text = runk.ToString() + "位";
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
