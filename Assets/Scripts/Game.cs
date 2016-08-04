using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {

	public static int gridWidth = 5;
	public static int gridHeight = 20;

	// Use this for initialization
	void Start () {
		
		SpawnNextTetromino ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void SpawnNextTetromino () {

		GameObject nextTetromino = (GameObject)Instantiate (Resources.Load (GetRandomTetromino (), typeof(GameObject)), new Vector2 (2f, 10f), Quaternion.identity);
	}

	public bool CheckIsInsideGrid (Vector2 pos) {

		return (pos.x >= 0 && pos.x < gridWidth && pos.y >= 0);
	}

	public Vector2 Round (Vector2 pos) {
	
		return new Vector2 ((pos.x), (pos.y));
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
}
