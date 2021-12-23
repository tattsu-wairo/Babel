using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class ScoreManager : MonoBehaviourPunCallbacks
{
    PhotonScoreView[] score;
    GameObject[] scoreObject;
    // Start is called before the first frame update
    void Start()
    {
        score = new PhotonScoreView[5];
    }

    // Update is called once per frame
    void Update()
    {
        scoreObject = GameObject.FindGameObjectsWithTag("score");
        for(int i = 0;i < scoreObject.Length;i++){
            score[i] = scoreObject[i].GetComponent<PhotonScoreView>();
        }
        ScoreUpdate();
    }
    
    private void ScoreUpdate(){
        PhotonScoreView topScore = score[0];
        if(topScore == null) return;
        for(int i = 1;i < scoreObject.Length;i++){
            topScore = (topScore > score[i])? topScore:score[i];
        }
        GetComponent<TextMesh>().text = "Player"+topScore.ID;
    }
}