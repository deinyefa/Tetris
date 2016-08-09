using UnityEngine;
using System.Collections;

public class Tetromino : MonoBehaviour {

	private float fall = 0f;
	private float individualScoreTime;

	private AudioSource audioSource;

	public int individualScore = 100;

	public float fallSpeed = 1f;

	public bool allowRotation = true;
	public bool limitRotation = false;

	public AudioClip moveSound;
	public AudioClip rotateSound;
	public AudioClip landSound;

	void Start () {
	
		audioSource = GetComponent<AudioSource> ();
	}
		
	void Update () {
		CheckUserInput ();
		UpdateIndividualScore ();
	}

	void UpdateIndividualScore () {

		if (individualScoreTime < 1) {
		
			individualScoreTime += Time.deltaTime;

		} else {
		
			individualScoreTime = 0;
			individualScore = Mathf.Max (individualScore - 10, 0);

		}
	}

	void CheckUserInput() {
		
		if (Input.GetKeyDown (KeyCode.RightArrow)) {

			transform.position += new Vector3 (1f, 0f, 0f);

			if (CheckIsValidPosition ()) {

				FindObjectOfType<Game> ().UpdateGrid (this);
				PlayMoveAudio ();
			} else {
			
				transform.position += new Vector3 (-1f, 0f, 0f);
			}

		} else if (Input.GetKeyDown (KeyCode.LeftArrow)) {

			transform.position += new Vector3 (-1f, 0f, 0f);

			if (CheckIsValidPosition ()) {

				FindObjectOfType<Game> ().UpdateGrid (this);
				PlayMoveAudio ();
			} else {

				transform.position += new Vector3 (1f, 0f, 0f);
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

					//- if the position is valid, we update the grid
					FindObjectOfType<Game> ().UpdateGrid (this);

					PlayRotateAudio ();
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

			transform.position += new Vector3 (0f, -1f, 0f);

			if (CheckIsValidPosition ()) {

				FindObjectOfType<Game> ().UpdateGrid (this);

				if (Input.GetKeyDown (KeyCode.DownArrow)) {

					PlayMoveAudio ();
				}

			} else {

				transform.position += new Vector3 (0f, 1f, 0f);

				FindObjectOfType<Game> ().DeleteRow ();

				//- Check if there are any minos above the grid
				if (FindObjectOfType<Game> ().CheckIsAboveGrid (this)) {
				
					FindObjectOfType<Game> ().GameOver ();
				}

				//- Play land audio, then Spawn the next piece
				PlayLandAudio();
				FindObjectOfType<Game> ().SpawnNextTetromino ();

				Game.currentScore += individualScore;

				enabled = false;
			
			}

			fall = Time.time;
		}
	}

	/// <summary>
	/// Plays the move sound when tetromino moves
	/// </summary>
	void PlayMoveAudio () {

		audioSource.PlayOneShot (moveSound);
	}

	/// <summary>
	/// Plays the rotate sound when the tetormino rotates
	/// </summary>
	void PlayRotateAudio() {

		audioSource.PlayOneShot (rotateSound);
	}

	/// <summary>
	/// Plays land sound when tetromino lands
	/// </summary>
	void PlayLandAudio () {

		audioSource.PlayOneShot (landSound);
	}

	bool CheckIsValidPosition () {

		foreach (Transform mino in transform) {
		
			Vector2 pos = FindObjectOfType<Game> ().Round (mino.position);

			if (FindObjectOfType<Game> ().CheckIsInsideGrid (pos) == false) {
			
				return false;
			}

			if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform) {

				return false;
			}
		}

		return true;
	}
}
