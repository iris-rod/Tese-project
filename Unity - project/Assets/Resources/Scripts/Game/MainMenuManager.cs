using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

  private GameObject GameManager;

	// Use this for initialization
	void Start () {
    GameManager = transform.GetChild(0).gameObject;
    GameManager.SetActive(false);
    DontDestroyOnLoad(GameManager);
    DontDestroyOnLoad(transform.gameObject);
    GameObject.Find("LeapHandController").GetComponent<HandController>().SetScene("MainMenu");
	}

  public void ChangeScene(string scene, string levels)
  {
    SceneManager.LoadScene(scene);
    GameManager.SetActive(true);
    GameManager.GetComponent<GameManager>().SetLevels(levels);
    GameObject.Find("LeapHandController").GetComponent<HandController>().SetScene(scene);
  }
}
