using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuSystem : MonoBehaviour {

	public Text levelText;
	public Text highScoreText;
	public Text highScoreText2;
	public Text highScoreText3;

	void Start () {


		levelText.text = "Level: 0";

		highScoreText.text = PlayerPrefs.GetInt ("highscore").ToString();
		highScoreText2.text = PlayerPrefs.GetInt ("highscore2").ToString();
		highScoreText3.text = PlayerPrefs.GetInt ("highscore3").ToString ();
	}

	public void PlayAgain () {

		SceneManager.LoadScene ("Start");
	}

	public void GameLevel () {
		if (Game.startingLevel == 0) {
			
			Game.startingAtLevelZero = true;
		} else {
			
			Game.startingAtLevelZero = false;
		}

		SceneManager.LoadScene ("Level");
	}

	public void HowToPlay () {

		SceneManager.LoadScene ("How To Play");
	}

	public void ChangedValue (float value) {

		Game.startingLevel = (int)value;
		levelText.text = "Level: " + value.ToString ();
	}

}
