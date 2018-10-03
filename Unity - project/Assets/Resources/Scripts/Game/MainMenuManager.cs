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
    DontDestroyOnLoad(GameObject.FindGameObjectWithTag("headset"));
    DontDestroyOnLoad(GameManager);
    DontDestroyOnLoad(FrontBoard);
    DontDestroyOnLoad(GameObject.Find("MenuBoard_simple"));
    DontDestroyOnLoad(transform.gameObject);
    GameObject.Find("LeapHandController").GetComponent<HandController>().SetScene("MainMenu");
	}

  public void ChangeScene(string scene, string levels)
  {
    SceneManager.LoadScene(scene);
    GameManager.SetActive(true);
    GameManager.GetComponent<GameManager>().SetLevels(levels);
    GameObject.Find("LeapHandController").GetComponent<HandController>().SetScene(scene);
    if (scene.ToLower() == "tutorial")
    {
      TutorialManager.Start();
    }
    FrontBoard.GetComponent<BlackBoardManager>().SetScene(scene);
    nextScene = scene;
    Invoke("SetUpScene", .5f);
  }

  public void ChangeToMainMenu()
  {
    SceneManager.LoadScene("MainMenu");
    Destroy(transform.gameObject);
    Destroy(GameObject.FindGameObjectWithTag("headset"));
    Destroy(GameManager);
    Destroy(FrontBoard);
    Destroy(GameObject.Find("MenuBoard_simple"));
    Destroy(transform.gameObject);
    //GameObject.Find("LeapHandController").GetComponent<HandController>().SetScene("MainMenu");
    //FrontBoard.GetComponent<BlackBoardManager>().SetScene("MainMenu");
  }

  void SetUpScene()
  {
    GameManager.GetComponent<Settings>().SetUp();
    GameManager.GetComponent<BoxAtomsManager>().SetUp();
    GameObject.Find("ControlPanelAnswer").transform.GetChild(0).GetChild(0).GetComponent<CompleteLevelButton>().SetScene(nextScene);
  }

}
