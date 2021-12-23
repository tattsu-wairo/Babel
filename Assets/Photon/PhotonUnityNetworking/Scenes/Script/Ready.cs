using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Ready : MonoBehaviourPunCallbacks
{
    public GameObject CharacterMaker;
    CharacterMaker maker;
    Button Btn;
    [SerializeField]Button btn;
    int Count=0;
    void Awake() {
        PhotonNetwork.AutomaticallySyncScene =true;
        if(!PhotonNetwork.IsMasterClient) return;
        var hashtable = new ExitGames.Client.Photon.Hashtable();
        hashtable.Add("role",new int[]{0,0,0,0,1});
        hashtable.Add("index",5);
        hashtable.Add("playerID",0);
        hashtable.Add("Player0",0);
        hashtable.Add("Player1",0);
        hashtable.Add("Player2",0);
        hashtable.Add("Player3",0);
        hashtable.Add("Player4",0);
        hashtable.Add("BabelTouch",new int[]{-1,-1,-1,-1});
        hashtable.Add("BabelProgress",new float[]{0,0,0,0});
        hashtable.Add("ChangeFlag0",false);
        hashtable.Add("ChangeFlag1",false);
        hashtable.Add("ChangeFlag2",false);
        hashtable.Add("ChangeFlag3",false);
        hashtable.Add("ChangeFlag4",false);
        hashtable.Add("isChanged",false);
        hashtable.Add("CurrentNumber",1);
        hashtable.Add("MaxBabelID",-1);
        hashtable.Add("EggPlayerId",1);
        hashtable.Add("ASFstate",new int[]{0,0});
        hashtable.Add("ASFbools",new bool[][]{new bool[]{false,false},new bool[]{false,false}});
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);   
    }

    void Start()
    {
        Count=PhotonNetwork.CurrentRoom.PlayerCount;
        CharacterMaker=GameObject.Find("CharacterMaker");
        maker=CharacterMaker.GetComponent<CharacterMaker>();
        maker.LobbyPlayerMaker();
    }

    // Update is called once per frame
    void Update()
    {
        if(Count!=PhotonNetwork.CurrentRoom.PlayerCount){
            Count=PhotonNetwork.CurrentRoom.PlayerCount;
            SceneManager.LoadScene("Room");
           
        }
    }
    //ゲームスタートが押されると呼ばれる
    public void StartGame(){
        if(!PhotonNetwork.IsMasterClient) return;
            PhotonNetwork.CurrentRoom.IsOpen=false;
            SceneManager.LoadScene("MainScene");
    } 
}
