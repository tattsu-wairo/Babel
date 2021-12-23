using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CharacterChange : MonoBehaviourPunCallbacks
{
    [SerializeField] private AnimalList animalList;

    /*public void Change(string name){
        //サメになる！！
        Transform player = GameObject.Find(name).transform;
        DestroyAnimal(player);
        GameObject animal = Instantiate(animalList.predator,player.position,player.rotation);
        animal.transform.parent = player;
        Agent agent = player.gameObject.GetComponent<Agent>();
        agent.animator = animal.GetComponent<Animator>();
        player.tag = "predator";
        //
        for(int j = 0;j < 5;j++){
            if("Player"+j == name) continue;
            player = GameObject.Find("Player"+j).transform;
            DestroyAnimal(player);
            animal = Instantiate(animalList.preyer[Random.Range(0,4)],player.position,player.rotation);
            animal.transform.parent = player;
            agent = player.gameObject.GetComponent<Agent>();
            agent.animator = animal.GetComponent<Animator>();
            if(player.CompareTag("predator")){
                player.tag = "preyer";
            }
        }
    }*/

    private void DestroyAnimal(Transform player){
        for(int i = 0;i < player.childCount;i++){
            if(player.GetChild(i).name != "Main Camera(Clone)"){
                Destroy(player.GetChild(i).gameObject);
            }
        }
    }
}
