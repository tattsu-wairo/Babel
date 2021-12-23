using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Counter : MonoBehaviour
{
	public GameObject scoreCounter;
	public GameObject timeCounter;
	public GameObject player;

	Agent agent;

	private float time = 10.0f;
	private float flame = 0f;
	private int score = 0;
	void Start()
	{
		scoreCounter = GameObject.Find("Score");
		timeCounter = GameObject.Find("TimeCounter");
		player=GameObject.Find("Player(Clone)");
		agent = player.GetComponent<Agent>();
	}

	// Update is called once per frame
	void Update()
	{
		if (time >= 0)
		{
			time -= Time.deltaTime;
			flame += Time.deltaTime;
			if (flame >= 0.99)
			{
				score += 10;
				flame = 0f;
			}
			Debug.Log(Time.deltaTime);
			SetCounterValues();
		}
		else if (time < 0)
		{
			SceneManager.LoadScene("GameOver");
			Debug.Log("Time UP");
			Debug.Log(agent.GetPoint());
            Debug.Log(score);
		}
	}

	void SetCounterValues()
	{
		scoreCounter.GetComponent<Text>().text = (score+agent.GetPoint()).ToString();
		timeCounter.GetComponent<Text>().text = time.ToString("F1");
	}
}
