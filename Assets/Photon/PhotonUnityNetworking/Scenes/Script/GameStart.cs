using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class GameStart : MonoBehaviourPun
{
      [SerializeField] private GameObject CMaker;
    private CharacterMaker player;
    private  int NpcNum=0;
    private int number;
    private bool readyflag=false;
    private bool IsLock=true;
 
    private void Start()
    {
        Cursor.visible=false;
        Cursor.lockState=CursorLockMode.Locked;
        Invoke("wait",3);
    }

    private void Update()
    {
        Cursor.lockState=CursorLockMode.Locked;
        if(!readyflag)return;
        if(!IsLock)return;
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        number=(int)hashtable["EggPlayerId"];
        if(PhotonNetwork.LocalPlayer.ActorNumber.Equals(number)){
            IsLock=false;
            GameReady();
            number+=1;
            hashtable["EggPlayerId"]=number;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            IsLock=true;
        }
        if(PhotonNetwork.CurrentRoom.Players.Count.Equals(number-1)&&PhotonNetwork.IsMasterClient){
            NpcReady();
            number+=1;
            hashtable["EggPlayerId"]=number;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        }
    }
    // Invokeで呼ばれる
    private void wait(){
        readyflag=true;
    }

    private void GameReady(){
        player=CMaker.GetComponent<CharacterMaker> ();
        player.PlayerMaker();
       
    }

    private void NpcReady(){
         while(NpcNum+PhotonNetwork.CurrentRoom.Players.Count<5)
        {
          player.NpcMaker();
          NpcNum+=1;
        } 
    }
}
