using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCast : MonoBehaviour
{
    private LayerMask mask;
    private Ray[] rays = new Ray[13];
    // Start is called before the first frame update
    void Start(){
        mask = 1 << 3 | 1 << 6 | 1 << 7;
    }

    // Update is called once per frame
    void Update(){
        MakeRays();
        //DisplayRay();
    }

    public void MakeRays(){
        transform.Rotate(new Vector3(0,-90,0));
        for(int i = 0;i < rays.Length;i++){
            rays[i] = new Ray(transform.position,transform.forward);
            transform.Rotate(new Vector3(0,15,0));
        }
        transform.Rotate(new Vector3(0,-105,0));
    }

    public List<Vector3> GetTarget(){
        List<Vector3> targetsVec = new List<Vector3>();
        for(int i = 0;i < rays.Length;i++){
            RaycastHit hit;
            /*if(Physics.Raycast(rays[i],out hit,20,mask) && (hit.collider.CompareTag("predator") || hit.collider.CompareTag("Untagged"))){
                continue;
            }*/
            if(Physics.Raycast(rays[i],out hit,20,mask) && hit.collider.CompareTag("target")){
                targetsVec.Add(hit.transform.position);
            }
        }
        if(targetsVec.Count == 0){
            targetsVec.Add(Vector3.zero);
        }
        return targetsVec;
    }

    private void DisplayRay(){
        for(int i = 0;i < rays.Length;i++){
            RaycastHit hit;
            if(Physics.Raycast(rays[i],out hit,20,mask)){
                if(hit.collider.CompareTag("predator")){
                    Debug.DrawRay(rays[i].origin,hit.point-rays[i].origin,Color.red,0.0f);
                }else if(hit.collider.CompareTag("target")){
                    Debug.DrawRay(rays[i].origin,hit.point-rays[i].origin,Color.cyan,0.0f);
                }else{
                    Debug.DrawRay(rays[i].origin,hit.point-rays[i].origin,Color.green,0.0f);
                }
            }else{
                Debug.DrawRay(rays[i].origin,rays[i].direction*20,Color.white,0.0f);
            }
        }
    }
}
