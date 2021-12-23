using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class AntiSharkField : MonoBehaviour, IPunObservable
{
    bool isOn;
    bool sleep;
    public Material[] materials;
    private int[] ASFstate;
    private bool[][] ASFbools;

    private int objectID;
    private float validtime;
    private float recast;
    private bool readyflag = false;

    private void Wait(){
        readyflag = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        //isOn = false;
        //sleep = false;
        validtime = 10.0f;
        recast = 20.0f;
        objectID = int.Parse(Regex.Replace(gameObject.name,@"[^0-9]",""));
        Invoke("Wait",4);
    }

    private void Update() {
        if(!readyflag) return;
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        ASFstate = (int[]) PhotonNetwork.CurrentRoom.CustomProperties["ASFstate"];
        ASFbools = (bool[][]) PhotonNetwork.CurrentRoom.CustomProperties["ASFbools"];
        if(ASFstate[objectID] == 1){
            ASFbools[objectID][0] = true;
            validtime -= Time.deltaTime;
            if(validtime <= 0.0f){
                ASFbools[objectID][0] = false;
                ASFbools[objectID][1] = true;
                ASFstate[objectID] = 2;
                validtime = 10.0f;
                SetProp(hashtable, ASFstate);
            }
        }
        if(ASFstate[objectID] == 2){
            recast -= Time.deltaTime;
            if(recast <= 0.0f){
                ASFbools[objectID][1] = false;
                ASFstate[objectID] = 0;
                recast = 20.0f;
                SetProp(hashtable, ASFstate);
            }
        }
        this.GetComponent<Renderer>().material = materials[ASFstate[objectID]];
    }

    private void SetProp(ExitGames.Client.Photon.Hashtable hashtable, int[] ASFstate){
        hashtable["ASFstate"] = ASFstate;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    // Update is called once per frame
    public void TurnOn(){
        if(sleep) return;
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        ASFstate = (int[]) hashtable["ASFstate"];
        ASFstate[objectID] = 1;
        hashtable["ASFstate"] = ASFstate;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    public bool isActive(){
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        ASFbools = (bool[][]) PhotonNetwork.CurrentRoom.CustomProperties["ASFbools"];
        return ASFbools[objectID][0];
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // 処理対象が自身のオブジェクトである場合
        if (stream.IsWriting)
        {
            // オブジェクトの状態を管理するデータをストリームに送る
            //stream.SendNext(同期対象のメンバ2);
        }
        // 処理対象が自身のオブジェクトでない場合　
        else
        {
            // 1. オブジェクトの状態を管理するデータをストリームから受け取る
            //同期対象のメンバ2 = (同期対象のメンバ1のデータ型)stream.ReceiveNext();

            // 2. オブジェクトの状態を管理するメンバからオブジェクトの状態を実際に変化させる
        }
    }
}