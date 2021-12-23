using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

public class NPCController : Agent,IPunObservable
{
    [SerializeField]GameObject[] targets = new GameObject[6];
    NavMeshAgent myAgent;
    int r;
    private RayCast rayCast;
    private bool isRunaway;

    protected override void Awake() {
        base.Awake();
        for(int i = 0;i < targets.Length;i++){
            targets[i] = GameObject.Find("Target" + (i+1));
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        isNPC = true;
        myAgent = GetComponent<NavMeshAgent>();
        r = Random.Range(0, 6);
        myAgent.SetDestination(targets[r].transform.position);
        myAgent.speed = 5.0f;
        rayCast = GetComponent<RayCast>();
        isRunaway = false;
    }

    protected override void Move()
    {
        base.Move();
        animator.SetBool("isRun", true);
        if (myAgent.remainingDistance < 1.0f)
        {
            r = Random.Range(0, 6);
            myAgent.SetDestination(targets[r].transform.position);
        }
        Vector3 nextPoint = myAgent.steeringTarget;
        Vector3 targetDir = nextPoint - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(targetDir);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 360f * Time.deltaTime);
    }

    protected override void Chase(Collider other)
    {
        if(killCT != 0) return;
        myAgent.SetDestination(other.transform.position);
        Vector3 pos1 = this.transform.position;
        Vector3 pos2 = other.transform.position;
        float dist = Vector3.Distance(pos1,pos2);
        if(dist < 3.0f && killCT == 0){
            Kill(other);
        }
    }

    protected override void Runaway(Collider other){
        if(!isRunaway || myAgent.remainingDistance < 1.0f){
            myAgent.velocity = Vector3.zero;
            Vector3 dir = this.transform.position - other.transform.position;
            this.transform.localRotation = Quaternion.LookRotation(dir);
            rayCast.MakeRays();
            List<Vector3> targetsVec = rayCast.GetTarget();
            myAgent.SetDestination(SelectMax(targetsVec));
            isRunaway = true;
        }
    }

    private Vector3 SelectMax(List<Vector3> targetsVec){
        int index = 0;
        float dist = Vector3.Distance(transform.position,targetsVec[index]);
        for(int i = 0;i < targetsVec.Count;i++){
            float tmp = Vector3.Distance(transform.position,targetsVec[i]);
            if(tmp > dist){
                index = i;
            }
        }
        return targetsVec[index];
    }

    public void NPCChange()
    {
        GameObject animal;
        Vector3 pos = transform.position;
        pos.y += 1;
        animal = PhotonNetwork.Instantiate(animalList.NPC_preyer[Random.Range(0,4)].name,pos,Quaternion.identity);
        Extend(animal);
    }

    protected override void Sparked(Collider other){
        if(other.GetComponent<AntiSharkField>().isActive()){
            myAgent.speed = 3.0f;
        }else{
            myAgent.speed = 5.0f;
        }
    }

    protected override void SparkExit(Collider other){
        myAgent.speed = 5.0f;
    }

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info) {
    // 処理対象が自身のオブジェクトである場合
        if (stream.IsWriting) {
        // オブジェクトの状態を管理するデータをストリームに送る
            //stream.SendNext(同期対象のメンバ1);
            //stream.SendNext(同期対象のメンバ2);
        } 
    // 処理対象が自身のオブジェクトでない場合　
        else {
        // 1. オブジェクトの状態を管理するデータをストリームから受け取る
            //同期対象のメンバ1 = (同期対象のメンバ1のデータ型)stream.ReceiveNext();
            //同期対象のメンバ2 = (同期対象のメンバ1のデータ型)stream.ReceiveNext();

        // 2. オブジェクトの状態を管理するメンバからオブジェクトの状態を実際に変化させる
        }
  }
}
