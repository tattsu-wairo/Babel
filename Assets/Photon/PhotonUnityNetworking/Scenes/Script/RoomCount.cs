using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class RoomCount : MonoBehaviourPunCallbacks
{
   
    //ルームに動きがあった場合
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("人数変動");
        foreach(RoomInfo r in roomList)
        {
            if(r.Name.Equals(this.transform.name)){
                if(r.PlayerCount>=2){
                    this.gameObject.SetActive(false);
                }
                if(r.PlayerCount<=2){
                    this.gameObject.SetActive(true);
                }
            }
        }
    }
   
}
