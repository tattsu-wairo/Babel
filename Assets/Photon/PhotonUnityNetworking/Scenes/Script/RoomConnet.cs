using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomConnet :MonoBehaviourPunCallbacks
{
    private RoomOptions roomOptions = new RoomOptions();
    private bool flag=false;
    public GameObject noEnter;
    
    private void Update()
    {
        if(PhotonNetwork.InRoom&&SceneManager.GetActiveScene().name=="RoomSelect"){
            Debug.Log("遷移");     
            SceneManager.LoadScene("Room");
        }if(flag){
            if(SceneManager.GetActiveScene().name=="RoomSelect"){
                Invoke("wait",1);
                
            }
        }
    }
    
    private void wait(){
        noEnter.SetActive(true);
    }
    public void JoinRoom1()
    {
        roomOptions.MaxPlayers=5;
        
        Debug.Log($"ルーム1に参加します。");
     
        PhotonNetwork.JoinOrCreateRoom("ルーム1",roomOptions, TypedLobby.Default);
        flag=true;
    }

    public void JoinRoom2()
    {
        roomOptions.MaxPlayers=5;
        
        Debug.Log($"ルーム2に参加します。");

        PhotonNetwork.JoinOrCreateRoom("ルーム2",roomOptions, TypedLobby.Default);
        flag=true;
    }

    public void JoinRoom3()
    {
        roomOptions.MaxPlayers=5;
        
        Debug.Log($"ルーム3に参加します。");
     
        PhotonNetwork.JoinOrCreateRoom("ルーム3",roomOptions, TypedLobby.Default); 
        flag=true;
    } 
}
