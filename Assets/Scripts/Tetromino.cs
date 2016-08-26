using UnityEngine;
using System.Collections;

public class Tetromino : MonoBehaviour {

	float fall = 0f;
	private float fallSpeed;
	public bool allowRotation = true;
	public bool limitRotation = false;

	public AudioClip moveSound;
	public AudioClip rotateSound;
	public AudioClip landSound;

	private float continuousVerticalSpeed = 0.05f;		//- The speed at with the tetromint moves when the down button is held down
	private float continuousHorizontalSpeed = 0.01f;	//- The speed at with the tetromint moves when the left or right button is held down
	private float buttonDownWaitMax = 0.2f;				//- How long to wait before the tetromino recognises that a button is being held down

	private float verticalTimer = 0f;
	private float horizontalTimer = 0f;
	private float buttonDownWaitTimerHorizontal = 0;
	private float buttonDownWaitTimerVertical = 0;

	private bool movedImmediateVertical = false;
	private bool movedImmediateHorizontal = false;

	public int individualScore = 100;

	private AudioSource audioSource;

	private float individualScoreTime;


	//- Touch movement variables
	private int touchSensitivityHorizontal = 8;
	private int touchSensitivityVertical = 4;

	Vector2 previousUnitPosition = Vector2.zero;
	Vector2 direction = Vector2.zero;

	bool moved = false;


	void Start () {
	
		audioSource = GetComponent<AudioSource> ();
	}
		
	void Update () {

		if (!Game.isPaused) {

			CheckUserInput ();
			UpdateIndividualScore ();
			UpdateFallSpeed ();
		}
	}

	void UpdateFallSpeed () {

		fallSpeed = Game.fallSpeed;
	}

	void UpdateIndividualScore () {

		if (individualScoreTime < 1) {
		
			individualScoreTime += Time.deltaTime;

		} else {
		
			individualScoreTime = 0;
			individualScore = Mathf.Max (individualScore - 10, 0);

		}
	}

	/// <summary>
	/// Checks the user input.
	/// </summary>
	void CheckUserInput() {

		//- This method checks the keys that the player can press to manipulate the position of the tetromino
		//- Options are left, right, down and up
		//- Left and right moves the tetromino horizontally
		//- Up rotates it
		#if UNITY_ANDROID

		if (Input.touchCount > 0) {
		
			Touch t = Input.GetTouch (0);

			if (t.phase == TouchPhase.Began) {
			
				previousUnitPosition = new Vector2 (t.position.x, t.position.y);

			} else if (t.phase == TouchPhase.Moved) {

				Vector2 touchDeltaPosition = t.deltaPosition;
				direction = touchDeltaPosition.normalized;

				if (Mathf.Abs (t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal && direction.x < 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10) {
				
					//- Move left
					MoveLeft ();
					previousUnitPosition = t.position;
					moved = true;

				} else if (Mathf.Abs (t.position.x - previousUnitPosition.x) >= touchSensitivityHorizontal && direction.x > 0 && t.deltaPosition.y > -10 && t.deltaPosition.y < 10) {
				
					//- Move right
					MoveRight ();
					previousUnitPosition = t.position;
					moved = true;

				} else if (Mathf.Abs (t.position.y - previousUnitPosition.y) >= touchSensitivityVertical && direction.y < 0 && t.deltaPosition.x > -10 && t.deltaPosition.x < 10) {
				
					//- Move down
					MoveDown ();
					previousUnitPosition = t.position;
					moved = true;
				}

			} else if (t.phase == TouchPhase.Ended) {
			
				if (!moved && t.position.x > Screen.width / 4) {
				
					Rotate ();
				}

				moved = false;
			}
		}

		if (Time.time - fall >= fallSpeed) {

			MoveDown();
		}

		#else

		if (Input.GetKeyUp (KeyCode.RightArrow) || Input.GetKeyUp (KeyCode.LeftArrow)) {
		
			movedImmediateHorizontal = false;
			horizontalTimer = 0;
			buttonDownWaitTimerHorizontal = 0;
		}

		if (Input.GetKeyUp(KeyCode.DownArrow)) {
		
			movedImmediateVertical = false;
			verticalTimer = 0;
			buttonDownWaitTimerVertical = 0;
		}
		
		if (Input.GetKey(KeyCode.RightArrow)) {

			MoveRight ();
		}

		if (Input.GetKey (KeyCode.LeftArrow)) {
			
			MoveLeft ();
		}

		if (Input.GetKeyDown (KeyCode.UpArrow)) {

			Rotate ();
		} 

		if (Input.GetKey (KeyCode.DownArrow) || (Time.time - fall >= fallSpeed)) {

			MoveDown ();
		}

		#endif
	}

	/// <summary>
	/// Moves to the left.
	/// </summary>
	void MoveLeft() {

		if (movedImmediateHorizontal) {
		
			if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {
			
				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}
			if (horizontalTimer < continuousHorizontalSpeed) {
			
				horizontalTimer += Time.deltaTime;
				return;
			}
		}

		if (!movedImmediateHorizontal) {
		
			movedImmediateHorizontal = true;
		}
		horizontalTimer = 0;

		//- First we try to move the tetromino to the left
		transform.position += new Vector3 (-1f, 0f, 0f);

		if (CheckIsValidPosition ()) {

			FindObjectOfType<Game> ().UpdateGrid (this);
			PlayMoveAudio ();
		} else {

			transform.position += new Vector3 (1f, 0f, 0f);
		}

	}

	/// <summary>
	/// Moves to the right.
	/// </summary>
	void MoveRight() {

		if (movedImmediateHorizontal) {

			if (buttonDownWaitTimerHorizontal < buttonDownWaitMax) {

				buttonDownWaitTimerHorizontal += Time.deltaTime;
				return;
			}
			if (horizontalTimer < continuousHorizontalSpeed) {

				horizontalTimer += Time.deltaTime;
				return;
			}
		}

		if (!movedImmediateHorizontal) {

			movedImmediateHorizontal = true;
		}
		horizontalTimer = 0;

		//- First we attempt to move the tetromino to the right
		transform.position += new Vector3 (1f, 0f, 0f);

		//- We then check if the tetromino is a valid position
		if (CheckIsValidPosition ()) {

			//- If it is, we then call the UpdateGrid method which records this tetrominos new position
			FindObjectOfType<Game> ().UpdateGrid (this);
			PlayMoveAudio ();

		} else {

			//- If it isn't we move the tetromino back to the left 
			transform.position += new Vector3 (-1f, 0f, 0f);
		}
	}

	/// <summary>
	/// Moves down.
	/// </summary>
	void MoveDown() {
		
		if (movedImmediateVertical) {

			if (buttonDownWaitTimerVertical < buttonDownWaitMax) {

				buttonDownWaitTimerVertical += Time.deltaTime;
				return;
			}

			if (verticalTimer < continuousVerticalSpeed) {

				verticalTimer += Time.deltaTime;
				return;
			}
		}

		if (!movedImmediateVertical) {

			movedImmediateVertical = true;
		}

		verticalTimer = 0;

		transform.position += new Vector3 (0f, -1f, 0f);

		if (CheckIsValidPosition ()) {

			FindObjectOfType<Game> ().UpdateGrid (this);

			if (Input.GetKey (KeyCode.DownArrow)) {

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

	/// <summary>
	/// Rotates.
	/// </summary>
	void Rotate() {

		//- The up arrow was pressed, let's first check if the tetromino is allowed to rotate
		if (allowRotation) {

			//- If it is, we need to check if the tetromino is limited to just back and forth
			if (limitRotation) {

				//- If it is, we need to check what the current position is
				if (transform.rotation.eulerAngles.z >= 90) {

					//- If it is at 90 then we know it was already rotated, so we rotate it back by -90
					transform.Rotate (0, 0, -90);
				} else {

					//- If it isn't, then we rotate it to 90
					transform.Rotate (0, 0, 90);
				}
			} else {

				//- If it isn't limited, we rotate it to 90
				transform.Rotate (0f, 0f, 90);
			}

			//- Now we check if the tetromino is at a valid position after attempting a arotation
			if (CheckIsValidPosition ()) {

				//- if the position is valid, we update the grid
				FindObjectOfType<Game> ().UpdateGrid (this);
				PlayRotateAudio ();

			} else {

				//- If it isn't at a valid position, we rotate it back -90
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
