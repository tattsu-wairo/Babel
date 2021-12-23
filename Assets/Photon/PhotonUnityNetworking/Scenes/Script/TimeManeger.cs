using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class TimeManeger : MonoBehaviour
{
    public int timeLimit=10;
    public Text timeText;
    public Text scoreText;
    public Text winnerText;
    public Text resurtScoreText;

    public Text exitText;


    public Slider slider;
    public GameObject finishPanel;
    public GameObject resultPanel;

    private int time;
    private int startTime;
    private int exitTime;
    private int exitSecond;
    private int killedTime;
    private const int killCT=10;
    PhotonScoreView[] scorelist=new PhotonScoreView[5];

    public void setKilledTime(){
        killedTime=PhotonNetwork.ServerTimestamp;
        slider.gameObject.SetActive(true);
    }

    // Start is called before the first frame update
    void Start()
    {
        startTime=PhotonNetwork.ServerTimestamp;
        time=0;
        exitSecond=0;
        killedTime=PhotonNetwork.ServerTimestamp;
        finishPanel.SetActive(false);
        resultPanel.SetActive(false);
        slider.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        time=PhotonNetwork.ServerTimestamp;
        float remainKillCT=killCT-(time-killedTime)/1000.0f;
        if(remainKillCT<0){
            slider.value=0.0f;
            slider.gameObject.SetActive(false);
        }else{
            slider.value=remainKillCT;
        }
        int second=timeLimit-(time-startTime)/1000;//残り時間(秒)
        //タイムアップしてから3秒後
        if(second<=-3){
            finishPanel.SetActive(false);
            resultPanel.SetActive(true);
            GameObject[] scoreObject = GameObject.FindGameObjectsWithTag("score");
            for(int i = 0;i < scoreObject.Length;i++){
                scorelist[i] = scoreObject[i].GetComponent<PhotonScoreView>();
            }
            PhotonScoreView topScore = scorelist[0];
            for(int i=1;i<scorelist.Length;i++){
                topScore = (topScore > scorelist[i])? topScore:scorelist[i];
            }
            winnerText.text=string.Format("Winner：Player{0}",topScore.ID);
            string allScore="Score：\n";
            for(int i=0;i<scorelist.Length;i++){
                allScore+=string.Format("Player{0}：{1}\n",i,scorelist[i].Score);
            }
            resurtScoreText.text=allScore;
            exitSecond=30-(PhotonNetwork.ServerTimestamp-exitTime)/1000;//退室まで残り時間(秒)
            exitText.text=string.Format("{0}で退室します",exitSecond);
        }
        //タイムアップした時
        else if(second<=0){
            timeText.text="";
            Time.timeScale=0.0f;//時間停止
            Cursor.visible=true;
            Cursor.lockState = CursorLockMode.None;
            slider.gameObject.SetActive(false);
            finishPanel.SetActive(true);
            GameObject predator=GameObject.FindGameObjectWithTag("predator");
            predator.GetComponent<Agent>().setGameFinish();
            GameObject[] preyers=GameObject.FindGameObjectsWithTag("preyer");
            foreach(GameObject preyer in preyers){
                preyer.GetComponent<Agent>().setGameFinish();
            }
            exitTime=PhotonNetwork.ServerTimestamp;
        }
        //時間内なら
        else{
            int minute=second/60;
            second=second%60;
            timeText.text=string.Format("{0}:{1:D2}",minute,second);
        }
        if(exitSecond<0){
            Application.Quit();
        }
    }
}