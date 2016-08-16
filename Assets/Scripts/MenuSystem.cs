using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Advertisements;

public class MenuSystem : MonoBehaviour {

	public Text levelText;
	public Text highScoreText;
	public Text highScoreText2;
	public Text highScoreText3;

	public Text lastScore;

	void Start () {

		//- ad testmode is true
		Advertisement.Initialize ("1115962", true);

		if (levelText != null)
			levelText.text = "Level: 0";
		
		if (highScoreText != null)
			highScoreText.text = PlayerPrefs.GetInt ("highscore").ToString();

		if (highScoreText2 != null)
			highScoreText2.text = PlayerPrefs.GetInt ("highscore2").ToString();

		if (highScoreText3 != null)
			highScoreText3.text = PlayerPrefs.GetInt ("highscore3").ToString ();

		if (lastScore != null)
			lastScore.text = PlayerPrefs.GetInt ("lastscore").ToString ();
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


	public void PlayAgain () {

		ShowAd ();
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
