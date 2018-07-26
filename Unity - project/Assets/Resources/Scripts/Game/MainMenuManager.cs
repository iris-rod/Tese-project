using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour {

  private GameObject GameManager;
  private GameObject FrontBoard;
  private string nextScene;
  private string nextLevels;

	// Use this for initialization
	void Start () {
    GameManager = transform.GetChild(0).gameObject;
    FrontBoard = GameObject.Find("FrontBoard");
    GameManager.SetActive(false);
    DontDestroyOnLoad(GameManager);
    DontDestroyOnLoad(FrontBoard);
    DontDestroyOnLoad(transform.gameObject);
    GameObject.Find("LeapHandController").GetComponent<HandController>().SetScene("MainMenu");
	}

  public void ChangeScene(string scene, string levels)
  {
    SceneManager.LoadScene(scene);
    GameManager.SetActive(true);
    GameManager.GetComponent<GameManager>().SetLevels(levels);
    GameObject.Find("LeapHandController").GetComponent<HandController>().SetScene(scene);
    if (scene == "Tutorial")
    {
      TutorialManager.Start();
      FrontBoard.GetComponent<BlackBoardManager>().SetScene(scene);
    }
    Invoke("SetUpScene", .5f);
  }

  void SetUpScene()
  {
    GameManager.GetComponent<Settings>().SetUp();
    GameManager.GetComponent<BoxAtomsManager>().SetUp();
  }

}
