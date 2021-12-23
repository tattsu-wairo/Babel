using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ReturnTitle : MonoBehaviour
{
    // Start is called before the first frame update
public GameObject rule;
public GameObject explain;
public void Return(){
    SceneManager.LoadScene("Title");
}
public void Explain(){
    rule.gameObject.SetActive(false);
    explain.gameObject.SetActive(true);    
}
public void Rule(){
    explain.gameObject.SetActive(false);
    rule.gameObject.SetActive(true);
}
}
