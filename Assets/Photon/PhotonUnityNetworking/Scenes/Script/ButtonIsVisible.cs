using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ButtonIsVisible : MonoBehaviourPun
{
   private void Update()
    {
         if(!PhotonNetwork.IsMasterClient){
            this.gameObject.SetActive(false);
        }else{
            this.gameObject.SetActive(true);
        }
    }
}
