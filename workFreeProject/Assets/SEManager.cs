using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour {

    [SerializeField]
    AudioClip jump;
    [SerializeField]
    AudioClip coin;
    [SerializeField]
    AudioClip wall;

    AudioSource AudioSorce;

    // Use this for initialization
    void Start () {
        AudioSorce = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.J))
        {
            SePlay("jump");
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            SePlay("coin");
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            SePlay("wall");
        }
    }

    public void SePlay( string seName )
    {
        switch( seName )
        {
            case "jump":
            {
                AudioSorce.PlayOneShot(jump);
                break;
            }
            case "coin":
            {
                AudioSorce.PlayOneShot(coin);
                break;
            }
            case "wall":
            {
                AudioSorce.PlayOneShot(wall);
                break;
            }
            default:
            {
                Debug.Log("エラー：引数に"+ seName+"が渡されましたが、SEManagerの変数名に" + seName +"が用意されていないか、このメソッド中のcaseに引数名のcaseが含まれておりません。");
                break;
            }
        }

    }
}
