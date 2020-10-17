using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// アクションシーン中のプレイの状態を管理するクラス
/// </summary>
public class PlayState : MonoBehaviour
{

    //Instance
    static public PlayState playState;
    //アクションシーンの再再生時にオブジェを複製するかどうかのフラグ
    static public bool copyFlg = false; 
    //ゲームの状態
    public enum GameMode
    {
        StartEf,
        Play,
        Menu,
        Anime,
        Stop,
        Rain,
        RainSelect,
        Clear,
        ClearFront,
        GameOver,
        Pause,
        Thunder,
        RotationPot,
    }
    //ゲームモードの変化を検知するシーケンサー用の変数
    GameMode backGameMode;
    //現在のゲームモード
    public GameMode gameMode;
    //アメフラシ起動演出の状態からプレイモードに遷移するまでの時間
    public float rainTime = 0;

    public bool Tutorial = false;

    [SerializeField]
    TutorialUI tutorialUI;
    [SerializeField ]
    public  float ThunderTime=0;
    public float RotationPotTime = 0;
    public bool RotationPotTimech;
    //
    bool IsThunder = false;
    [SerializeField]
    CharacterMaster ChaMs;



    public GameObject stageObj=null;
    public CharacterController character;
    public GameObject WaterObj = null;
    [SerializeField]
    public GameObject WaveEf;

    [SerializeField]
    GameObject topObj;

    bool timelot = false ;
    bool rotSet = false;

    public Vector3 PayerPos = Vector3.zero;
    Vector3 afterAngle = Vector3.zero;
    Vector3 startAngle = Vector3.zero;

     GameObject WaveObj = null;

    WaterHi waterHi = null;

    Vector3 saveLot = Vector3.zero;
    Vector3 playerStartPos = Vector3.zero;

    public bool IsFallBox = false;


    // 初期化
    void Start()
    {
        
        playState = new PlayState();
        playState.WaveEf = WaveEf;
        playState.gameMode =gameMode;
        playState.ChaMs = ChaMs;
        playState.topObj = topObj;
        playState.Tutorial = StageMake.LoadStageData.TutorialFlg;
        if (playState.Tutorial) { tutorialUI.TutorialStart(); }
        if (!PlayState.copyFlg)
        {
            DontDestroyOnLoad(gameObject);
            PlayState.copyFlg = true;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }

    private void Update()
    {

        //ゲームモードが変更されたときに呼び出す処理
        if(playState.backGameMode != playState.gameMode)
        {
            switch (playState.gameMode)
            {
                case GameMode.ClearFront:
                    Progress.progress.FrontResultSet();
                    break;
                case GameMode.Clear:
                    Progress.progress.ResultSet();
                    break;
                case GameMode.GameOver:
                    Progress.progress.GameOverSet();
                    break;
                case GameMode.Rain:
                    break;
                case GameMode.Thunder:
                    playState.ThunderTime = 5;
                    break;
            }
        }
        playState.backGameMode = playState.gameMode;

        //ゲームモードごとに毎フレーム呼び出す処理
        switch (playState.gameMode)
        {
            case GameMode.Rain:
                playState.RainUpDate();
                break;
            case GameMode.Thunder:
                playState.ThunderUpDate();
                break;
            case GameMode.RotationPot:
                playState.RotationPotUpDate();
                break;
        }
        //Debug.Log(playState.gameMode);
    }



    //アメフラシ演出中の処理
    void RainUpDate()
    {
        playState.rainTime -= Time.deltaTime;
        if (playState.rainTime <= 0)
        {
            if ((AmehurashiManager.amehurashi.amehurashiTrueCont == StageMake.LoadStageData.DoThunder && !IsThunder && StageMake.LoadStageData.IsThunder))
            {
                IsThunder = true;
                PlayState.playState.gameMode = PlayState.GameMode.Thunder;
                
               
            }
            else
            {
                playState.gameMode = GameMode.Play;

            }
        }
    }
    void ThunderUpDate()
    {
        ThunderTime = (ThunderTime - Time.deltaTime < 0) ? 0 : ThunderTime - Time.deltaTime;
        if (ThunderTime == 0)
        {
            playState.gameMode = GameMode.Play;
        }
    }
    void RotationPotUpDate()
    {
        RotationPotTime = (RotationPotTime - Time.deltaTime < 0) ? 0 : RotationPotTime - Time.deltaTime;

        if(RotationPotTime<0.75&&!RotationPotTimech)
        {
            RotationPotTimech = true;
            Camera.main.gameObject.GetComponent<CameraPos>().RainPotChangeOut(true);

        }
        


        if(rotSet&& RotationPotTime < 4)
        {
            WaterObj.SetActive(false);
            rotSet = false ;
            WaveObj.GetComponent<BoxMake>().MoveStart(waterHi.max /StageMake.LoadStageData .stageSize .x );
        }

        if (5-RotationPotTime>1&&5- RotationPotTime < 2.75f)
        {
            timelot = true;
            float lotTime = ((5-RotationPotTime) - 1f) / 1.75f;
            //Debug.Log(Mathf.Lerp(0f, -90f, lotTime));
            stageObj.transform.eulerAngles = Vector3.Lerp(startAngle,afterAngle, lotTime);

            character.gameObject .transform.eulerAngles= new Vector3(0, character.gameObject.transform.eulerAngles.y, 0);
            character.gameObject.transform.localPosition = Vector3.Lerp(PayerPos, playerStartPos, lotTime);
        }

        if(timelot && 5 - RotationPotTime > 2.75f)
        {
            timelot = false;
            stageObj.transform.eulerAngles = Vector3.Lerp(startAngle, afterAngle, 1);
            IsFallBox = true;
            character.GetComponent<PlayerType2>().IsDontCharacterMove = false;
            WaterObj.SetActive(true);
            WaterObj.transform.eulerAngles=Vector3 .zero ;
            WaterObj.transform.position = new Vector3(0.01f, 0, 0.01f);
            WaveObj.SetActive(false );
        }



        if (RotationPotTime == 0)
        {
            playState.gameMode = GameMode.Play;
            character.GetComponent<PlayerType2>().IsDontShield = false;
             
        }
    }
    public void RotationPotStart(Vector3 lotAngle ,Vector3 Weve, Vector3 pot, float goAngle=0,bool SetAngle=false)
    {
        IsFallBox = false;
        character.GetComponent<PlayerType2>().IsDontShield = true;
        character.GetComponent<PlayerType2>().IsDontCharacterMove  = true;

        PlayState.playState.gameMode = PlayState.GameMode.RotationPot;
        PlayState.playState.RotationPotTime = 5;
        PlayState.playState.RotationPotTimech = false;
        Camera.main.gameObject.GetComponent<CameraPos>().RainPotChange(goAngle, SetAngle);
        PayerPos = character.gameObject.transform.localPosition;
        afterAngle = lotAngle+ stageObj.transform.eulerAngles;
        startAngle = stageObj.transform.eulerAngles;
        waterHi = WaterObj.GetComponent<WaterHi>();

        WaveObj.SetActive(true);
        WaveObj.transform.localScale = StageMake.LoadStageData.stageSize- new Vector3(0.1f, 0.1f, 0.1f);
        WaveObj.transform.localPosition = Vector3.zero;
        WaveObj.transform.localEulerAngles  = -lotAngle;
        WaveObj.GetComponent<BoxMake>().sethi  = waterHi.max / StageMake.LoadStageData.stageSize.x;
        WaveObj.GetComponent<BoxMake>().moveTime =0;
        WaveObj.GetComponent<BoxMake>().lat =0;
        WaveObj.GetComponent<BoxMake>().BoxCheck = false;
        WaveObj.transform.eulerAngles = Weve ;

        saveLot += Weve;
        playerStartPos = pot;
        WaveObj.GetComponent<BoxMake>().planeUpdate();

        rotSet = true;
    }

    public void WaveMake()
    {
        WaveObj = Instantiate(WaveEf, stageObj.transform);
        WaveObj.GetComponent<BoxMake>().Init(topObj);
        WaveObj.SetActive(false);
    }
}
