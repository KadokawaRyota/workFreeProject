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
    [SerializeField]
    AudioClip pi;
    [SerializeField]
    AudioClip pon;

    AudioSource AudioSorce;

    // Use this for initialization
    void Start () {
        AudioSorce = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
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
            case "pi":
                {
                    AudioSorce.PlayOneShot(pi);
                    break;
                }
            case "pon":
                {
                    AudioSorce.PlayOneShot(pon);
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
