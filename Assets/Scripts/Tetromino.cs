using UnityEngine;
using System.Collections;

public class Tetromino : MonoBehaviour {

	private float fall = 0f;

	public float fallSpeed = 0.5f;
	public bool allowRotation = true;
	public bool limitRotation = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		CheckUserInput ();
	}

	void CheckUserInput() {
		
		if (Input.GetKeyDown (KeyCode.RightArrow)) {

			transform.position += new Vector3 (0.5f, 0f, 0f);

			if (CheckIsValidPosition ()) {

			} else {
			
				transform.position += new Vector3 (-0.5f, 0f, 0f);
			}

		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {

			transform.position += new Vector3 (-0.5f, 0f, 0f);

			if (CheckIsValidPosition ()) {
				
			} else {

				transform.position += new Vector3 (0.5f, 0f, 0f);
			}

		} else if (Input.GetKeyDown (KeyCode.UpArrow)) {

			if (allowRotation) {

				if (limitRotation) {

					if (transform.rotation.eulerAngles.z >= 90) {
						
						transform.Rotate (0, 0, -90);
					} else {
						
						transform.Rotate (0, 0, 90);
					}
				} else {
					
					transform.Rotate (0f, 0f, 90);
				}
				if (CheckIsValidPosition ()) {
					// ...
				} else {
			
					if (limitRotation) {

						if (transform.rotation.eulerAngles.z >= 90) {
						
							transform.Rotate (0, 0, -90);
						} else {
						
							transform.Rotate (0f, 0f, 90);
						}
					} else {
						
						transform.Rotate (0, 0, -90);
					}
				}
			}

		} else if (Input.GetKeyDown (KeyCode.DownArrow) || (Time.time - fall >= fallSpeed)) {

			transform.position += new Vector3 (0f, -0.5f, 0f);

			if (CheckIsValidPosition ()) {

			} else {

				transform.position += new Vector3 (0f, 0.5f, 0f);
			}

			fall = Time.time;
		}
	}

	bool CheckIsValidPosition () {

		foreach (Transform mino in transform) {
		
			Vector2 pos = FindObjectOfType<Game> ().Round (mino.position);

			if (FindObjectOfType<Game> ().CheckIsInsideGrid (pos) == false) {
			
				return false;
			}
		}

		return true;
	}
}
