using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerController : Agent, IPunObservable
{
    public float speed = 10f;
    private float RoateSpeed = 100f; // 視点移動の速度
    public GameObject prefab;
    private GameObject myCamera;
    private int PlayerNumber;
    Transform canvas, slider; //Babelで使ってる
    Vector3 targetPos;//プレイヤー自身の座標(カメラの注視点)
    //private
    Rigidbody rb;
    Vector3 aim;//カメラの向く方向
    float inputHorizontal;//縦入力
    float inputVertical;//横入力
    [SerializeField] private float moveSpeed = 5.0f;        // 移動速度
    private Vector3 moveDirection = Vector3.zero;
    AudioSource audioSource;
    // Start is called before the first frame update

    protected override void Awake()
    {
        base.Awake();
        if (!photonView.IsMine) return;
        GameObject camera = GameObject.Find("BaseCamera");
        Destroy(camera);
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z - 8);
        myCamera = Instantiate(prefab, pos, Quaternion.identity);
        //myCamera.transform.parent = transform;
    }
    protected override void Start()
    {
        if (!photonView.IsMine) return;
        base.Start();
        isNPC = false;
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        targetPos = transform.position;
        aim = transform.position - myCamera.transform.position;//カメラの方向を計算
        myCamera.transform.localRotation = Quaternion.LookRotation(aim);//カメラをプレイヤーに向かせる
        audioSource = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        if (!photonView.IsMine) return;
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(myCamera.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        Vector3 moveForward = cameraForward * inputVertical + myCamera.transform.right * inputHorizontal;
        Debug.Log("moveForward : " + moveForward);
        Debug.Log("moveForward * moveSpeed : " + moveForward * moveSpeed);

        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        rb.velocity = moveForward.normalized * moveSpeed + new Vector3(0, rb.velocity.y, 0);

        // キャラクターの向きを進行方向に
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(moveForward);
        }
    }
    protected override void Move()
    {
        //移動処理　NPCもプレイヤーも　オーバーライドして
        //キャラクターの移動
        base.Move();
        inputHorizontal = Input.GetAxis("Horizontal");
        inputVertical = Input.GetAxis("Vertical");
        if (inputHorizontal != 0 || inputVertical != 0)
        {
            animator.SetBool("isRun", true);
        }
        else
        {
            animator.SetBool("isRun", false);
        }
        myCamera.transform.position += transform.position - targetPos;//プレイヤーの移動分だけカメラも移動させる
        targetPos = transform.position;
        //視点の移動
        float mouseInputX = Input.GetAxis("Mouse X");
        float mouseInputY = Input.GetAxis("Mouse Y");
        if (mouseInputX != 0 || mouseInputY != 0)
        {
            // targetの位置のY軸を中心に、回転（公転）する
            myCamera.transform.RotateAround(targetPos, Vector3.up, mouseInputX * Time.deltaTime * RoateSpeed);
        }
    }

    protected override void Chase(Collider other)
    {
        if (killCT == 0 && Input.GetMouseButtonDown(0))
        {
            Kill(other);
        }
    }

    protected override void Kill(Collider other)
    {
        audioSource.PlayOneShot(audioSource.clip);
        base.Kill(other);
        GameObject timeManeger = GameObject.FindGameObjectWithTag("timeManeger");
        timeManeger.GetComponent<TimeManeger>().setKilledTime();
    }

    protected override void Babel(Collider other)
    {
        if (!photonView.IsMine) return;
        int objectID = int.Parse(Regex.Replace(other.gameObject.name, @"[^0-9]", ""));
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        int[] BabelTouch = (int[])hashtable["BabelTouch"];
        if ((BabelTouch[objectID] == -1 || BabelTouch[objectID] == PhotonNetwork.LocalPlayer.ActorNumber) && !isChanged)
        {
            BabelTouch[objectID] = PhotonNetwork.LocalPlayer.ActorNumber;
            hashtable["BabelTouch"] = BabelTouch;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            canvas = other.transform.Find("Canvas");
            slider = canvas.transform.Find("Slider");
            if (Input.GetKey("space") && !(bool) hashtable["ChangeFlag"+id])
            {
                slider.GetComponent<ProgressBar>().Activate();
            }else
            {
                slider.GetComponent<ProgressBar>().Deactivate();
            }
        }
    }

    protected override void BabelExit(Collider other)
    {
        if (!photonView.IsMine) return;
        int objectID = int.Parse(Regex.Replace(other.gameObject.name, @"[^0-9]", ""));
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        int[] BabelTouch = (int[])hashtable["BabelTouch"];
        if (BabelTouch[objectID] == PhotonNetwork.LocalPlayer.ActorNumber)
        {
            BabelTouch[objectID] = -1;
            hashtable["BabelTouch"] = BabelTouch;
            PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);
            slider.GetComponent<ProgressBar>().Deactivate();
            slider = null;
        }
    }

    protected override void Change(int number)
    {
        GameObject animal;
        Vector3 pos = transform.position;
        pos.y += 1;
        if (PhotonNetwork.LocalPlayer.ActorNumber.Equals(number))
        {
            animal = PhotonNetwork.Instantiate(animalList.Player_predator.name, pos, Quaternion.identity);
        }
        else
        {
            animal = PhotonNetwork.Instantiate(animalList.Player_preyer[Random.Range(0, 4)].name, pos, Quaternion.identity);
        }
        if(slider != null){
            slider.GetComponent<ProgressBar>().Deactivate();
        }
        Destroy(myCamera.gameObject);
        Extend(animal);
    }

    protected override void Spark(Collider other){
        if (Input.GetKey("space")){
            other.GetComponent<AntiSharkField>().TurnOn();
        }
    }

    protected override void Sparked(Collider other){
        if(other.GetComponent<AntiSharkField>().isActive()){
            moveSpeed = 3.0f;
        }else{
            moveSpeed = 5.0f;
        }
    }

    protected override void SparkExit(Collider other){
        moveSpeed = 5.0f;
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