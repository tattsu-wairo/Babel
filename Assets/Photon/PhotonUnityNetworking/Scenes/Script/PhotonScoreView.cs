using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonScoreView : MonoBehaviourPun, IPunObservable
{
    private TextMesh _text;
    private int id;
    private int score;
    public int Score{
        get{return score;}
        set{score = value; RequestOwner();}
    }
    public int ID{
        get{return id;}
        set{id = value; RequestOwner();}
    }

    public static bool operator <(PhotonScoreView a, PhotonScoreView b)
        => a.Score < b.Score;

    public static bool operator >(PhotonScoreView a, PhotonScoreView b)
        => a.Score > b.Score;

    private void Awake() {
        _text = GetComponent<TextMesh>();
        score = 0;
        id = 0;
    }

    private void Update() {
        _text.text = "Player" + id + " : " + score;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(this.score);
            stream.SendNext(this.id);
        }else{
            this.score = (int)stream.ReceiveNext();
            this.id = (int)stream.ReceiveNext();
        }
    }

    public void RequestOwner(){
        if(!photonView.IsMine){
            if(photonView.OwnershipTransfer != OwnershipOption.Request){
                Debug.LogError("OwnershipTransferをRequestに変更してください");
            }else{
                photonView.RequestOwnership();
            }
        }
    }
}
