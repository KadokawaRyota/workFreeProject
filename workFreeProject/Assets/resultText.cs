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

        if( runk == -1 )
        {
            //オフライン時は見えないように。
            Runk.GetComponent<Text>().enabled = false;
            //位置を変更
            Score.GetComponent<RectTransform>().transform.position = Runk.GetComponent<RectTransform>().transform.position;
        }
        else
        {
            Runk.GetComponent<Text>().enabled = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
