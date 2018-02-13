using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour {

    public Text scoreText; //Text用変数
    private int score = 0; //スコア計算用変数
    private int scoreMax = 0;   //現在のスコア

    // Use this for initialization
    void Start () {
        scoreText.text = "Score: 0";
    }

    // Update is called once per frame
    void Update () {

        if( scoreMax > score )
        {
            int scoreDiff = scoreMax - score;
            if (scoreDiff > 10 && scoreDiff < 30 )
            {
                score += 1;
            }
            else if (scoreMax - score > 30 && scoreDiff < 600)
            {
                score += 5;
            }
            else if (scoreMax - score > 600)
            {
                score += 10;
            }
            else
            {
                score++;
            }

            scoreText.text = "Score: " + score.ToString();
        }
	}

    public void ScoreAdd( int value)
    {
        scoreMax += value;
    }
}
