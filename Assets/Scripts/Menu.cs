using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class Menu : MonoBehaviour
{

	public void StartGame ()
	{
		SceneManager.LoadScene ("GamePlay");
	}

	public void Quit ()
	{
		Application.Quit ();
	}
}
