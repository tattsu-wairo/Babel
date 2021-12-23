using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MoveRule : MonoBehaviour
{
   public void Move(){
       SceneManager.LoadScene("Rule");
   }

   public void MoveCredit(){
       SceneManager.LoadScene("Credits");
   }
}
