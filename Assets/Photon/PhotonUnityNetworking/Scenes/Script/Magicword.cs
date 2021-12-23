using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Magicword : MonoBehaviourPunCallbacks,IPunObservable
{
     void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    // 処理対象が自身のオブジェクトである場合
        if (stream.IsWriting) {
        // オブジェクトの状態を管理するデータをストリームに送る
            //stream.SendNext(同期対象のメンバ2);
        } 
    // 処理対象が自身のオブジェクトでない場合　
        else {
        // 1. オブジェクトの状態を管理するデータをストリームから受け取る
            //同期対象のメンバ2 = (同期対象のメンバ1のデータ型)stream.ReceiveNext();

        // 2. オブジェクトの状態を管理するメンバからオブジェクトの状態を実際に変化させる
        }
  }
}
