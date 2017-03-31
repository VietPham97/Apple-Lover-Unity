﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour 
{
	public static GameManager Instance {set; get;}

	private const float REQUIRED_SLICEFORCE = 400.0f;

	public GameObject fruitPrefab;
	public Transform trail;

	private bool isPaused;
	private List<Fruit> fruit = new List<Fruit>();
	private float lastSpawn;
	private float deltaSpawn = 1.0f;
	private Vector3 lastMousePos;
	private Collider2D[] fruitCols = new Collider2D[0];

	// UI 
	private int score;
	private int highscore;
	private int lifepoint;
	public Text scoreText;
	public Text highscoreText;
	public Image[] lifepoints;
	public GameObject pauseMenu;

	private void Awake()
	{
		Instance = this;
	}	

	private void Start()
	{
		NewGame();
	}
	
	private void NewGame()
	{
		lifepoint = 3;
		score = 0;
		scoreText.text = score.ToString();
		pauseMenu.SetActive(false);
		Time.timeScale = 1;
		isPaused = false;
	}

	private Fruit GetFruit()
	{
		Fruit f = fruit.Find(x => !x.IsActive); // find the false IsActive fruit

		if (f == null)
		{
			f = Instantiate(fruitPrefab).GetComponent<Fruit>();
			fruit.Add(f);
		}

		return f;
	}
	
	private void Update () 
	{
		if (isPaused) return;
		
		if (Time.time - lastSpawn > deltaSpawn)
		{
			Fruit f = GetFruit();
			float randomX = Random.Range(-1.65f, 1.65f);

			f.LaunchFruit(Random.Range(1.85f, 2.75f), randomX, -randomX);
			// A fruit is randomly spawn on the left or right or the middle of the screen
			// if left throw to the right, right throw to the left, and middle straight up  

			lastSpawn = Time.time;
		}	

		if (Input.GetMouseButton(0))
		{
			Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //return a vector3
			pos.z = -1;
			trail.position = pos;

			Collider2D[] thisFramesFruit = Physics2D.OverlapPointAll(new Vector2(pos.x, pos.y), LayerMask.GetMask("Fruit"));
			
			if((Input.mousePosition - lastMousePos).sqrMagnitude > REQUIRED_SLICEFORCE)
			{
				foreach (Collider2D c2 in thisFramesFruit)
				{
					for (int i = 0; i < fruitCols.Length; i++)
					{
						if (c2 == fruitCols[i])
						{
							c2.GetComponent<Fruit>().Slice();
						}
					}
				}
			}
			lastMousePos = Input.mousePosition;
			fruitCols = thisFramesFruit;
		}
	}

	public void IncrementScore(int scoreAmount)
	{
		score += scoreAmount; 
		scoreText.text = score.ToString();

		if(score > highscore)
		{
			highscore = score;
			highscoreText.text = "BEST: " + highscore.ToString();
		}
	}

	public void LoseLifepoint()
	{
		if (lifepoint <= 0)
		{
			Death();
			return;
		}

		lifepoint--;
		lifepoints[lifepoint].enabled = false;
	}

	public void Death()
    {
        Debug.LogWarning("You are death");
    }

	public void PauseGame()
	{
		pauseMenu.SetActive(!pauseMenu.activeSelf);
		isPaused = pauseMenu.activeSelf;
		Time.timeScale = (Time.timeScale == 0) ? 1 : 0;
	}
}
