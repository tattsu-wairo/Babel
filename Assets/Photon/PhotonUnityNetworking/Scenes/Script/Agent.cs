using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Text.RegularExpressions;

public class Agent : MonoBehaviourPunCallbacks
{
    [SerializeField] protected float KILLCOOLTIME = 10f;
    public int killPoint = 10;
    public int deadPoint = 10;
    public Animator animator;
    private int startTime;
    protected int point;
    protected float killCT = 0f;
    protected GameObject[] respawnPlaces = new GameObject[5];
    protected AnimalList animalList;
    protected bool gameFinished=false;
    protected int id;
    [SerializeField] protected GameObject text;
    protected GameObject textArea;
    protected PhotonTextView textView;
    private int number = 1;
    List<Agent> npcs;
    protected bool isNPC;
    protected bool isChanged;
    [SerializeField] protected GameObject killBall;
    protected PhotonScoreView scoreView;

    protected virtual void Awake() {
        if(!photonView.IsMine) return;
        Vector3 pos = gameObject.transform.position;
        pos.y += 1.5f;
        textArea = PhotonNetwork.Instantiate(text.name,pos,Quaternion.identity);
        textView = textArea.GetComponent<PhotonTextView>();
    }

    protected virtual void Start()
    {
        if(!photonView.IsMine) return;
        GameObject characterMaker = GameObject.Find("CharacterMaker");
        for(int i = 0;i < respawnPlaces.Length;i++){
            respawnPlaces[i] = GameObject.Find("Target" + (i+1));
        }
        animalList = characterMaker.GetComponent<AnimalList>();
        animator = GetComponentInChildren<Animator>();
        startTime=PhotonNetwork.ServerTimestamp;
        id = int.Parse(Regex.Replace(this.name, @"[^0-9]", ""));
        isChanged = false;
        textView.Text = this.name;
        GameObject scoreText = GameObject.Find("Score"+id);
        if(scoreText == null){
            scoreText = PhotonNetwork.Instantiate("Score0",new Vector3(-10,20+id*-3,50),Quaternion.identity);
            scoreText.name = "Score"+id;
        }
        scoreView = scoreText.GetComponent<PhotonScoreView>();
        point = scoreView.Score;
        scoreView.ID = id;
        Time.timeScale = 1;
    }

    protected virtual void Update()
    {
        if (!photonView.IsMine) return;
        if (killCT > 0){
            killCT -= Time.deltaTime;
        }
        else if (killCT < 0){
            killCT = 0;
        }
        Move();
        Entertarget();
        scoreView.Score = point;
        SetChange();
        if(isNPC) return;
        if(PhotonNetwork.LocalPlayer.ActorNumber.Equals(number)){
            CheckChange();
        }
        if(number > PhotonNetwork.CurrentRoom.Players.Count && PhotonNetwork.IsMasterClient){
            var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
            hashtable["isChanged"] = false;
            hashtable["CurrentNumber"] = 1;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            npcs = GetNPCs();
            foreach(NPCController npc in npcs){
                npc.NPCChange();
            }
        }
    }

    public void SetChange(){
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        isChanged = (bool)hashtable["isChanged"];
        number = (int)hashtable["CurrentNumber"];
    }

    public int GetPoint()
    {
        return point;
    }

    private List<Agent> GetNPCs(){
        List<Agent> agentList = new List<Agent>();
        GameObject[] preyers = GameObject.FindGameObjectsWithTag("preyer");
        Agent predator = GameObject.FindGameObjectWithTag("predator").GetComponent<Agent>();
        if(predator.isNPC){
            agentList.Add(predator);
        }
        foreach(GameObject preyer in preyers){
            Agent agent = preyer.GetComponent<Agent>();
            if(agent.isNPC){
                agentList.Add(agent);
            }
        }
        return agentList;
    }

    protected virtual void Move()
    {
        Vector3 pos = gameObject.transform.position;
        pos.y += 1.5f;
        textArea.transform.position = pos;
        textArea.transform.rotation = gameObject.transform.rotation;
    }

    protected virtual void Chase(Collider other)
    {
        //追いかけ処理　NPCもプレイヤーも　オーバーライドして
    }

    protected virtual void Runaway(Collider other)
    {
        //逃げる処理　NPCだけ　オーバーライドして
    }

    protected virtual void Babel(Collider other)
    {
        //バベル接触処理
    }

    protected virtual void BabelExit(Collider other)
    {
        //バベル離脱判定
    }

    protected virtual void Spark(Collider other)
    {
        //フィールド起動
    }

    protected virtual void Sparked(Collider other)
    {
        //フィールド踏むよ
    }

    protected virtual void SparkExit(Collider other)
    {
        //フィールド出るよ
    }
    
    protected virtual void Kill(Collider other)
    {
        killCT = KILLCOOLTIME;
        point += killPoint;
        //animator.SetTrigger("Attack");
        PhotonNetwork.Instantiate(killBall.name,this.transform.position,this.transform.rotation);
    }

    public virtual void Died()
    {
        if(!photonView.IsMine) return;
        point -= deadPoint;
        GameObject[] chasers = GameObject.FindGameObjectsWithTag("predator");
        float distanceMax = 0;
        float distanceMin = Mathf.Infinity;
        GameObject respawnPlace = respawnPlaces[0];
        for (int i = 0; i < respawnPlaces.Length; i++)
        {
            distanceMin = Mathf.Infinity;
            foreach (GameObject chaser in chasers)
            {
                float distance = Vector3.Distance(respawnPlaces[i].transform.position, chaser.transform.position);
                if (distanceMin > distance)
                {
                    distanceMin = distance;
                }
            }
            if (distanceMax < distanceMin)
            {
                distanceMax = distanceMin;
                respawnPlace = respawnPlaces[i];
            }
        }
        this.transform.position = respawnPlace.transform.position;
    }
    
    public void setGameFinish(){
        gameFinished=true;
    }

    public void Entertarget()
    {
        if(gameFinished)return;
        GameObject enemy=GameObject.FindGameObjectWithTag("predator");
        if(enemy == null) return;
        if(gameObject==enemy)return;
        float distance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
        int time=PhotonNetwork.ServerTimestamp;
        if(distance<10 && (time-startTime)/1000>=1)
        {
            point++;
            startTime=time;
        }
    }

    private void CheckChange(){
        if(isChanged){
            Time.timeScale = 0;
            isChanged = false;
            number++;
            var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
            hashtable["CurrentNumber"] = number;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            int num = (int) hashtable["MaxBabelID"];
            Change(num);
        }
    }

    protected virtual void Change(int viewID){

    }

    protected void Extend(GameObject obj){
        obj.name = this.name;
        PhotonNetwork.Destroy(textArea);
        PhotonNetwork.Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("bullet") && this.CompareTag("preyer")){
            Died();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (this.CompareTag("predator") && other.CompareTag("preyer") && !other.isTrigger)
        {
            Chase(other);
        }
        if (this.CompareTag("preyer") && other.CompareTag("predator") && !other.isTrigger)
        {
            Runaway(other);
        }
        if (this.CompareTag("preyer") && other.CompareTag("Babel"))
        {
            Babel(other);
        }
        if (this.CompareTag("preyer") && other.CompareTag("AntiShark"))
        {
            Spark(other);
        }
        if (this.CompareTag("predator") && other.CompareTag("AntiShark"))
        {
            Sparked(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (this.CompareTag("preyer") && other.CompareTag("Babel"))
        {
            BabelExit(other);
        }
        if (this.CompareTag("predator") && other.CompareTag("AntiShark"))
        {
            SparkExit(other);
        }
    }

    /*public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        object value = null;
        if(propertiesThatChanged.TryGetValue("CurrentNumber",out value)){
            number = (int) value;
        }
        if(propertiesThatChanged.TryGetValue("isChanged",out value) && number == 1){
            isChanged = (bool) value;
        }
    }*/
}