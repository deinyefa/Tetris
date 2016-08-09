using UnityEngine;
using System.Collections;

public class MenuSystem : MonoBehaviour {

	public void PlayAgain () {

		Application.LoadLevel ("Start");
	}

	public void GameLevel () {

		Application.LoadLevel ("Level");
	}
}
