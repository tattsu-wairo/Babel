using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomTimeManager : MonoBehaviourPun
{
   private int startTime;
   private int second;
   public Text timeText;
   [SerializeField]int timeLimit;
    void Start()
    {
        startTime=PhotonNetwork.ServerTimestamp;
    }

    // Update is called once per frame
    void Update()
    {
        if(!PhotonNetwork.IsMasterClient) return;
        second=timeLimit-(PhotonNetwork.ServerTimestamp-startTime)/1000;
        timeText.text=string.Format("{0}秒でゲームを開始します",second);
        if(second<0){
            PhotonNetwork.CurrentRoom.IsOpen=false;
            SceneManager.LoadScene("MainScene");
        }
    }
}
