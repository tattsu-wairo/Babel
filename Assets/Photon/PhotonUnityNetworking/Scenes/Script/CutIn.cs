using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CutIn : MonoBehaviourPun
{
    public GameObject panel;
    private bool IsLock=false;
    AudioSource audioSource;
    bool played = false;
    private void Start()
    {
        panel.SetActive(false);
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        if((bool)hashtable["isChanged"]){
            panel.SetActive(true);
            if(!played){
                audioSource.PlayOneShot(audioSource.clip);
                played=true;
            }
            IsLock=true;
            Invoke("Wait",1);
        }
        if(!IsLock){
            panel.gameObject.SetActive(false);
        }
    }

    private void Wait(){
        IsLock=false;
        played=false;
        return;
    }
}
