using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class ProgressBar : MonoBehaviourPunCallbacks, IPunObservable
{
    Slider slider;
    GameObject Babel, canvas, background, fillarea;
    Animator animator;
    int objectID;
    bool isChanged;
    [SerializeField]

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        background = slider.transform.Find("Background").gameObject;
        fillarea = slider.transform.Find("Fill Area").gameObject;
        background.SetActive(false);
        fillarea.SetActive(false);
        canvas = transform.parent.gameObject;
        Babel = canvas.transform.parent.gameObject;
        objectID = int.Parse(Regex.Replace(Babel.gameObject.name, @"[^0-9]", ""));
        animator = Babel.GetComponent<Animator>();
        isChanged = false;
    }

    void Update(){
        animator.SetFloat("progressbar", slider.value);
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        float[] babelProgres = (float[]) hashtable["BabelProgress"];
        slider.value = babelProgres[objectID];
    }

    public void Activate()
    {
        if(isChanged) return;
        background.SetActive(true);
        fillarea.SetActive(true);
        slider.value += 0.001f;
        animator.SetFloat("progressbar", slider.value);
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        float[] BabelProgress = (float[]) hashtable["BabelProgress"];
        if(slider.value == 1.0f){
            Built(hashtable);
            slider.value = 0f;
        }
        BabelProgress[objectID] = slider.value;
        hashtable["BabelProgress"] = BabelProgress;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
    }

    public void Deactivate(){
        background.SetActive(false);
        fillarea.SetActive(false);
    }

    private void Built(ExitGames.Client.Photon.Hashtable hashtable){
        int[] babelTouch = (int[]) hashtable["BabelTouch"];
        int number = babelTouch[objectID];
        for(int i = 0;i < babelTouch.Length;i++){
            babelTouch[i] = -1;
        }
        hashtable["MaxBabelID"] = number;
        hashtable["isChanged"] = true;
        hashtable["BabelTouch"] = babelTouch;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
        animator.SetFloat("progressbar",slider.value);
    }

    /*public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if(!photonView.IsMine) return;
        object value = null;
        if(propertiesThatChanged.TryGetValue("BabelProgress",out value)){
            float[] BabelProgress = (float[]) value;
            slider.value = BabelProgress[objectID];
        }
        if(propertiesThatChanged.TryGetValue("isChanged",out value)){
            isChanged = (bool) value;
        }
    }*/

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