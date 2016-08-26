﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.Advertisements;

public class Game : MonoBehaviour {

	public static int gridWidth = 10;
	public static int gridHeight = 20;
	public static int currentScore = 0;

	public static Transform [,] grid = new Transform[gridWidth, gridHeight];

	public static bool startingAtLevelZero;
	public static int startingLevel;

	public Canvas hud_canvas;
	public Canvas pause_canvas;

	public int scoreOneLine = 40;
	public int scoreTwoLine = 100;
	public int scoreThreeLine = 300;
	public int scoreFourLine = 1200;

	public int currentLevel = 0;
	private int numLinesCleared = 0;

	public static float fallSpeed = 1f;
	public static bool isPaused = false;

	public Text hud_score;
	public Text hud_level;
	public Text hud_lines;

	public AudioClip clearedLineSound;

	private int numberOfRowsThisTurn = 0;

	private AudioSource audioSource;

	private GameObject previewTetromino, nextTetromino;

	private bool gameStarted = false;
	private int startingHighScore;
	private int startingHighScore2;
	private int startingHighScore3;

	private Vector2 previewTetrominoPosition = new Vector2 (12f, 15f);


	void Start () {

		pause_canvas.enabled = false;

		currentScore = 0;
		hud_score.text = "0";

		currentLevel = startingLevel;
		hud_level.text = currentLevel.ToString ();
		hud_lines.text = "0";

		SpawnNextTetromino ();

		audioSource = GetComponent<AudioSource> ();

		startingHighScore = PlayerPrefs.GetInt ("highscore");
		startingHighScore2 = PlayerPrefs.GetInt ("highscore2");
		startingHighScore3 = PlayerPrefs.GetInt ("highscore3");
	}

	/// <summary>
	/// Shows the ad.
	/// </summary>
	public void ShowAd () {
		if (Advertisement.IsReady())
		{
			Advertisement.Show();
		}
	}

	void Update () {

		UpdateScore ();
		UpdateUI ();
		UpdateLevel ();
		UpdateSpeed ();
		CheckUserInput ();
	}

	void CheckUserInput() {

		if (CrossPlatformInputManager.GetButtonDown ("MoveTouchpad") || Input.GetKeyDown (KeyCode.P)) {
		
			if (Time.timeScale == 1)
				PauseGame ();
			else
				ResumeGame ();
		}
	}

	/// <summary>
	/// Pauses the game.
	/// </summary>
	public void PauseGame () {

		Time.timeScale = 0;
		audioSource.Pause ();
		isPaused = true;
//		hud_canvas.enabled = false;
		pause_canvas.enabled = true;
		Camera.main.GetComponent<Blur>().enabled = true;

		ShowAd ();
	}

	/// <summary>
	/// Resumes the game.
	/// </summary>
	public void ResumeGame () {

		Time.timeScale = 1;
		isPaused = false;
		audioSource.Play ();
		hud_canvas.enabled = true;
		pause_canvas.enabled = false;
		Camera.main.GetComponent<Blur>().enabled = false;
	}

	/// <summary>
	/// Updates the level.
	/// </summary>
	void UpdateLevel() {

		if ((startingAtLevelZero == true) || (startingAtLevelZero == false && numLinesCleared / 10 > startingLevel)) {
			currentLevel = numLinesCleared / 10;
		}
	}

	/// <summary>
	/// Updates the speed.
	/// </summary>
	void UpdateSpeed() {

		fallSpeed = 1f - ((float)currentLevel * 0.1f);
	}

	/// <summary>
	/// Updates the UI.
	/// </summary>
	public void UpdateUI () {
	
		hud_score.text = currentScore.ToString ();
		hud_level.text = currentLevel.ToString ();
		hud_lines.text = numLinesCleared.ToString ();
	}

	/// <summary>
	/// Updates the score.
	/// </summary>
	public void UpdateScore() {
	
		if (numberOfRowsThisTurn > 0) {
		
			if (numberOfRowsThisTurn == 1) {

				ClearedOneLine ();

			} else if (numberOfRowsThisTurn == 2) {

				ClearedTwoLines ();

			} else if (numberOfRowsThisTurn == 3) {

				ClearedThreeLines ();

			} else if (numberOfRowsThisTurn == 4) {

				ClearedFourLines ();

			}

			numberOfRowsThisTurn = 0;

			PlayLineCleardSound ();
		}
	}

	public void ClearedOneLine() {
	
		currentScore += scoreOneLine + (currentLevel * 20);
		numLinesCleared++;
	}

	public void ClearedTwoLines () {
	
		currentScore += scoreTwoLine + (currentLevel * 25);
		numLinesCleared += 2;
	}

	public void ClearedThreeLines () {
	
		currentScore += scoreThreeLine + (currentLevel * 30);
		numLinesCleared += 3;
	}

	public void ClearedFourLines () {
	
		currentScore += scoreFourLine + (currentLevel * 40);
		numLinesCleared += 4;
	}

	public void PlayLineCleardSound () {

		audioSource.PlayOneShot (clearedLineSound);
	}

	/// <summary>
	/// Updates the high score.
	/// </summary>
	public void UpdateHighScore () {
		
		if (currentScore > startingHighScore) {

			PlayerPrefs.SetInt ("highscore3", startingHighScore2);
			PlayerPrefs.SetInt ("highscore2", startingHighScore);
			PlayerPrefs.SetInt ("highscore", currentScore);

		} else if (currentScore > startingHighScore2) {
		
			PlayerPrefs.SetInt ("highscore3", startingHighScore2);
			PlayerPrefs.SetInt ("highscore2", currentScore);

		} else if (currentScore > startingHighScore3) {
		
			PlayerPrefs.SetInt ("highscore3", currentScore);
		}

		PlayerPrefs.SetInt ("lastscore", currentScore);
	}

	public bool CheckIsAboveGrid (Tetromino tetromino) {

		for (int x = 0; x < gridWidth; ++x) {
		
			foreach (Transform mino in tetromino.transform) {
			
				Vector2 pos = Round (mino.position);
				if (pos.y > gridHeight - 1) {
				
					return true;
				}
			}
		}
		return false;
	}

	public bool IsFullRowAt (int y) {
	
		for (int x = 0; x < gridWidth; ++x) {
		
			if (grid [x, y] == null) {
			
				return false;
			}
		}
		// since found a full row, increment ful row variable
		numberOfRowsThisTurn ++;

		return true;
	}

	public void DeleteMinoAt (int y) {
	
		for (int x = 0; x < gridWidth; ++x) {
		
			Destroy (grid [x, y].gameObject);
			grid [x, y] = null;
		}
	}

	public void MoveRowDown (int y) {
	
		for (int x = 0; x < gridWidth; ++x) {
		
			if (grid [x, y] != null && y>0) {
			
				grid [x, y - 1] = grid [x, y];
				grid [x, y] = null;
				grid [x, y - 1].position += new Vector3 (0, -1, 0);
			}
		}
	}

	public void DeleteRow () {

		for (int y = 0; y < gridHeight; ++y) {
		
			if (IsFullRowAt (y)) {
			
				DeleteMinoAt (y);
				MoveAllRowsDown (y + 1);
				--y;
			}
		}
	}

	public void MoveAllRowsDown (int y) {

		for (int i = y; i < gridHeight; ++i) {
			MoveRowDown (i);
		}
	}

	public void UpdateGrid (Tetromino tetromino) {

		for (int y = 0; y < gridHeight; ++y) {

			for (int x = 0; x < gridWidth; ++x) {

				if (grid [x, y] != null) {

					if (grid [x, y].parent == tetromino.transform) {

						grid [x, y] = null;
					}
				}
			}
		}
	
		foreach (Transform mino in tetromino.transform) {

			Vector2 pos = Round (mino.position);

			if (pos.y < gridHeight) {

				grid[(int)pos.x, (int)pos.y] = mino;
			}
		}
	}

	public Transform GetTransformAtGridPosition (Vector2 pos) {

		if (pos.y > gridHeight - 1) {

			return null;
		} else {

			return grid [(int)pos.x, (int)pos.y];
		}
	}

	public void SpawnNextTetromino () {

		if (!gameStarted) {

			gameStarted = true;

			//- Spawn another tetromino from the resourses assets folder using the GetRandomTetromino method to select a random prefab
			nextTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), new Vector2 (5f, 20f), Quaternion.identity);
			previewTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino> ().enabled = false;

		} else {

			previewTetromino.transform.localPosition = new Vector2 (5f, 20f);
			nextTetromino = previewTetromino;
			nextTetromino.GetComponent <Tetromino> ().enabled = true;

			previewTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), previewTetrominoPosition, Quaternion.identity);
			previewTetromino.GetComponent<Tetromino> ().enabled = false;
		}

	}

	public bool CheckIsInsideGrid (Vector2 pos) {

		return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
	}

	public Vector2 Round (Vector2 pos) {
	
		return new Vector2 (Mathf.Round(pos.x), Mathf.Round(pos.y));
	}

	string GetRandomTetromino () {
		int randomTetromino = Random.Range (1, 8);

		string randomTetrominoName = "Prefabs/Tetromino_T";

		switch (randomTetromino) {
			
		case 1: 
			randomTetrominoName = "Prefabs/Tetromino_T";
			break;
		case 2:
			randomTetrominoName = "Prefabs/Tetromino_Long";
			break;
		case 3:
			randomTetrominoName = "Prefabs/Tetromino_Square";
			break;
		case 4: 
			randomTetrominoName = "Prefabs/Tetromino_J";
			break;
		case 5:
			randomTetrominoName = "Prefabs/Tetromino_L";
			break;
		case 6: 
			randomTetrominoName = "Prefabs/Tetromino_S";
			break;
		case 7:
			randomTetrominoName = "Prefabs/Tetromino_Z";
			break;
		}
		return randomTetrominoName;
	}

	public void GameOver () {

		UpdateHighScore ();
		SceneManager.LoadScene ("Game Over");
	}
}
