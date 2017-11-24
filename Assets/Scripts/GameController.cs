using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{

	public bool isPaused;
	public bool isGameOver;
	public Transform pauseCanvas;
	public Transform player;
	public Transform gameOverUI;
	public Transform winUI;
	public bool isGameWon;

	void Start ()
	{
		isPaused = false;
		isGameOver = false;
		isGameWon = false;
	}

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape)) {
			isPaused = !isPaused;
		}
		if (isGameWon) {
			Time.timeScale = 0;
			winUI.gameObject.SetActive (true);
			GameChanged (false);
		}
		if (isPaused && !isGameOver) {
			Time.timeScale = 0;
			gameOverUI.gameObject.SetActive (false);
			pauseCanvas.gameObject.SetActive (true);
			GameChanged (false);
		} else if (!isPaused && isGameOver) {
			pauseCanvas.gameObject.SetActive (false);
			gameOverUI.gameObject.SetActive (true);
			GameChanged (false);
		} else if (!isPaused && !isGameOver && !isGameWon) {
			gameOverUI.gameObject.SetActive (false);
			pauseCanvas.gameObject.SetActive (false);
			GameChanged (true);
		}
	}

	void GameChanged (bool started)
	{
		if (started) {
			Time.timeScale = 1;
			player.GetComponent<FirstPersonController> ().enabled = true;
			player.GetComponent<CharacterController> ().enabled = true;
			AudioListener.volume = 1;
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

		} else if (!started) {
			Time.timeScale = 0;
			player.GetComponent<FirstPersonController> ().enabled = false;
			player.GetComponent<CharacterController> ().enabled = false;
			AudioListener.volume = 0;
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;

		}
	}

	public void Restart ()
	{
		SceneManager.LoadScene ("GamePlay");
		gameOverUI.gameObject.SetActive (false);
	}

	void gameChange (string state)
	{
		if (state == "Paused") {

		} else if (state == "Game Over") {

		} else {
			Debug.Log ("No such game event" + state);
		}
	}

	public void Win ()
	{
		isGameWon = true;
	}

	public void Resume ()
	{
		Time.timeScale = 1;
		isPaused = false;
	}

	public void GameOver ()
	{
		isGameOver = true;
	}

	public void Quit ()
	{
		Application.Quit ();
	}

	public void QuitToMainMenu ()
	{
		SceneManager.LoadScene ("MainMenu");
	}
}
