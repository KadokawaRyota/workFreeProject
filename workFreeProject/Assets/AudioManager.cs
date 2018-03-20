using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Audio 管理
public class AudioManager : MonoBehaviour
{

    //現在存在しているオブジェクト実態の記憶領域
    static AudioManager instance;

    [SerializeField]
    GameObject bgm;

    [SerializeField]
    GameObject se;

    AudioSource bgmSourceMaster;
    AudioSource[] bgmSource;
    AudioSource[] seSource;
    AudioSource audioSource;

    public enum BGM
    {
        Title = 0,
        Home,
        Game001,
        Game002,
        Game003,
        Game004,
        Mission,
        MAX
    };


    public enum SE
    {
        Walk = 0,
        Run,
        SetRope,
        DisObject,
        OpenChat,
        MissionStart,
        ObjectMove,
        Result,
        Ending,
        MAX
    };

    AudioClip[] bgmClip;
    AudioClip[] seClip;
    //シングルトン処理
    void Awake()
    {

        //AudioManagerインスタンスが存在したら
        if (instance != null)
        {
            //今回インスタンス化したAudioManagerを破棄
            Destroy(this.gameObject);
            return;
            //AudioManagerインスタンスがなかったら
        }
        else if (instance == null)
        {
            //このAudioManagerをインスタンスとする
            instance = this;
        }

        //シーンを跨いでもAudioManagerインスタンスを破棄しない
        DontDestroyOnLoad(gameObject);

        bgmSource = bgm.GetComponents<AudioSource>();
        seSource = se.GetComponents<AudioSource>();

        //開始シーンごとに初期BGMを再生
        int random = 0;

        switch (SceneManager.GetActiveScene().name)
        {
            case "Title":
                Play_BGM(BGM.Title);
                break;

            case "Home":
                Play_BGM(BGM.Home);
                break;

            case "Offline":
                random = Random.Range(0, 4);
                switch (random)
                {
                    case 0:
                        Play_BGM(BGM.Game001);
                        break;

                    case 1:
                        Play_BGM(BGM.Game002);
                        break;

                    case 2:
                        Play_BGM(BGM.Game003);
                        break;

                    case 3:
                        Play_BGM(BGM.Game004);
                        break;

                    default:
                        Play_BGM(BGM.Game004);
                        break;
                }
                break;

            case "Main":
                random = Random.Range(0, 4);
                switch (random)
                {
                    case 0:
                        Play_BGM(BGM.Game001);
                        break;

                    case 1:
                        Play_BGM(BGM.Game002);
                        break;

                    case 2:
                        Play_BGM(BGM.Game003);
                        break;

                    case 3:
                        Play_BGM(BGM.Game004);
                        break;

                    default:
                        Play_BGM(BGM.Game004);
                        break;
                }
                break;

            case "Result":
                Stop_BGM();

                break;
        }

    }

    //引数のBGMをマスターソースに入れて再生
    public void Play_BGM(BGM bgm)
    {
        if (null != bgmSourceMaster)
        {
            Stop_BGM();
        }
        bgmSourceMaster = bgmSource[(int)bgm];
        bgmSourceMaster.Play();

    }

    //マスターソースを停止
    public void Stop_BGM()
    {
        if (null != bgmSourceMaster)
            bgmSourceMaster.Stop();
    }

    public void Play_SE(SE se)
    {
        seSource[(int)se].PlayOneShot(seSource[(int)se].clip);
        //audioSource.PlayOneShot(seClip[(int)se]);

    }

    public AudioSource GetBGMMasterSource()
    {
        return bgmSourceMaster;
    }

}