using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
public class CharacterMaker : MonoBehaviourPun
{
    [SerializeField] private GameObject Player;
    private GameObject prefab;
    [SerializeField] private GameObject[] panels = new GameObject[5];
    private int PlayerNumber = 0;
    private  int NpcNumber = 0;
    [SerializeField] private AnimalList animalList;
    [SerializeField] private GameObject[] respawnPlaces = new GameObject[5];
    private  float x;
    private  float y;
    private  float z;
    
    public void LobbyPlayerMaker()
    {
        PlayerNumber += 1;
        for(int i=0;i<PhotonNetwork.CurrentRoom.Players.Count;i++){
            x=panels[i].transform.position.x;
            y=panels[i].transform.position.y;
            z=panels[i].transform.position.z;
            GameObject egg=Instantiate(Player,new Vector3(x, y-10, z-10),Quaternion.Euler(0f, -100f, 0f));
            egg.transform.parent = panels[i].transform;
        }    
    }

    public void PlayerMaker()
    {
        MakeAgent(true);
    }

    public void NpcMaker()
    {
        NpcNumber++;
        MakeAgent(false);
    }

    private void MakeAgent(bool isPlayer){
        var hashtable = PhotonNetwork.CurrentRoom.CustomProperties;
        int id = (int) hashtable["playerID"];
        int[] role = (int[]) hashtable["role"];
        int index = (int) hashtable["index"];
        int r = Random.Range(0,index);
        index--;
        int tmp = role[r];
        role[r] = role[index];
        role[index] = tmp;
        //hashtable更新
        hashtable["playerID"] = id+1;
        hashtable["role"] = role;
        hashtable["index"] = index;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hashtable);

        Vector3 pos = respawnPlaces[id].transform.position;
        if(isPlayer){
            if(tmp == 0){
                prefab = PhotonNetwork.Instantiate(animalList.Player_preyer[Random.Range(0,4)].name,pos,Quaternion.identity);
            }else{
                prefab = PhotonNetwork.Instantiate(animalList.Player_predator.name,pos,Quaternion.identity);
            }
        }else{
            if(tmp == 0){
                prefab = PhotonNetwork.Instantiate(animalList.NPC_preyer[Random.Range(0,4)].name,pos,Quaternion.identity);
            }else{
                prefab = PhotonNetwork.Instantiate(animalList.NPC_predator.name,pos,Quaternion.identity);
            }
        }
        prefab.name = "Player"+id;
    }
}