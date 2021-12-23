using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class NonHostExplanation : MonoBehaviour
{
    private void Start()
    {
        if(!PhotonNetwork.IsMasterClient){
            this.gameObject.SetActive(true);
        }else{
            this.gameObject.SetActive(false);
        }
    }

    
}
