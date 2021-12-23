using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KillBall : MonoBehaviourPun
{
    private void DestroyBall(){
        if(!photonView.IsMine) return;
        PhotonNetwork.Destroy(this.gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("preyer")){
            DestroyBall();
        }
    }
}
