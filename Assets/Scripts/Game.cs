using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour {

	public static int gridWidth = 10;
	public static int gridHeight = 20;
	public static int currentScore = 0;

	public static Transform [,] grid = new Transform[gridWidth, gridHeight];

	public int scoreOneLine = 40;
	public int scoreTwoLine = 100;
	public int scoreThreeLine = 300;
	public int scoreFourLine = 1200;

	public Text hud_score;
	public AudioClip clearedLineSound;

	private int numberOfRowsThisTurn = 0;

	private AudioSource audioSource;

	void Start () {
		
		SpawnNextTetromino ();

		audioSource = GetComponent<AudioSource> ();
	}

	void Update () {

		UpdateScore ();
		UpdateUI ();
	}

	public void UpdateUI () {
	
		hud_score.text = currentScore.ToString ();
	}

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
	
		currentScore += scoreOneLine;
	}

	public void ClearedTwoLines () {
	
		currentScore += scoreTwoLine;
	}

	public void ClearedThreeLines () {
	
		currentScore += scoreThreeLine;
	}

	public void ClearedFourLines () {
	
		currentScore += scoreFourLine;
	}

	public void PlayLineCleardSound () {

		audioSource.PlayOneShot (clearedLineSound);
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

		for (int i = 0; i < gridHeight; ++i) {
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

		GameObject nextTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), new Vector2 (5f, 20f), Quaternion.identity);
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

		Application.LoadLevel ("Game Over");
	}
}
