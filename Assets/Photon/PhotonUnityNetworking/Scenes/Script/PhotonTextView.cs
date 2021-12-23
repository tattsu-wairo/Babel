using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class PhotonTextView : MonoBehaviourPun, IPunObservable
{
    private TextMesh textMesh;
    private string _text;
    public string Text{
        get{return _text;}
        set{
            _text = value;
            RequestOwner();
        }
    }

    private void Awake() {
        textMesh = GetComponent<TextMesh>();
        _text = "";
    }

    private void Update() {
        textMesh.text = _text;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if(stream.IsWriting){
            stream.SendNext(this._text);
        }else{
            this._text = (string)stream.ReceiveNext();
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
