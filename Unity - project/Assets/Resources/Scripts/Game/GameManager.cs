using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  private int level;
  private bool levelComplete, newLevel;
  private string levels;
  private string levelObjs;
  private string correctAnswerMC, pressedAnswer;
  private Manager manager;
  private LevelManager LM;
  private MoleculeManager MM;
  private ShelfManager SM;
  private InformationManager IM;
  //private AnswerPanel AP;

  private bool getAnswer;
  private bool correctMolLoaded;

  private string path = "";

  public BlackBoardManager BBManager;

  // Use this for initialization
  void Start()
  {
    MM = GetComponent<MoleculeManager>();
    LM = GetComponent<LevelManager>();
    SM = GameObject.FindGameObjectWithTag("shelves").GetComponent<ShelfManager>();
    manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Manager>();
    BBManager = GameObject.FindGameObjectWithTag("Board").GetComponent<BlackBoardManager>();
    IM = GameObject.Find("Info").GetComponent<InformationManager>();
    //AP = GameObject.Find("ControlPanelAnswers").GetComponent<AnswerPanel>();
    levelComplete = false;
    newLevel = true;
    getAnswer = false;
    correctMolLoaded = false;
    level = 1;
    levels = HandleTextFile.ReadLevels("levels_teste");
  }

  // Update is called once per frame
  void Update()
  {
    //Debug.Log("level: " + level);
    if (newLevel)
      SetLevel();

    if (levelComplete)
    {
      newLevel = true;
      levelComplete = false;
      level++;
    }
  }

  private void SetLevel()
  {
    Debug.Log("new Level: " + level);
    string objs = "";
    string[] levelsSplit = levels.Split(';');
    for(int i = 0; i < levelsSplit.Length; i ++)
    {
      string[] levelDescp = levelsSplit[i].Split(':');
      int levelID = int.Parse(levelDescp[0].Trim());
      if (level == levelID)
      {
        objs = levelDescp[1].Trim();
        break;
      }
    }

    LM.SetLevelId(level);
    LM.SetObjective(objs);
    CheckNextObjectiveSetup(LM.GetNextObjective());
    Invoke("NextLevelDisplay", 2f);
    newLevel = false;
  }

  void NextLevelDisplay()
  {
    //BBManager.UpdateDisplay(LM.GetLevel(), LM.GetSublevel());
    IM.UpdateLevel(LM.GetLevel());
    IM.UpdateDisplay(LM.GetNextObjective(),true);
    if(getAnswer)
      correctAnswerMC = IM.GetCorrectAnswer();
  }

  private bool CheckObjectiveComplete(string nextObj)
  {
    string[] objSplit = nextObj.Split('-');
    string type = objSplit[0];
    bool completed = false;
    switch (type)
    {
      case "build":
        string text = HandleTextFile.ReadString(objSplit[1] + ".txt");
        if (MM.CompareMoleculesString(text, false))
          completed = true;
        break;
      case "load":
        if (correctMolLoaded)
        {
          correctMolLoaded = false;
          completed = true;
        }
        break;
      case "save":
        string savedText = HandleTextFile.ReadString(objSplit[1] + ".txt");
        if (MM.CompareMoleculesString(savedText, true))
          completed = true;
        break;
      case "place":
        GameObject invi = GameObject.FindGameObjectWithTag("Invisible");
        if (invi.GetComponent<InvisibleMoleculeBehaviour>().HasOverlap())
        {
          completed = true;
          invi.GetComponent<InvisibleMoleculeBehaviour>().DestroyOverlap();
          Destroy(invi);
        }
        break;
      case "multiple choice":
        if(pressedAnswer.ToLower().Trim() == correctAnswerMC.ToLower().Trim())
        {
          completed = true;
          //AP.Disappear(); //make control panel with buttons disappear
        }
        break;
    }
    return completed;
  }

  private void CheckNextObjectiveSetup(string nextObj)
  {
    Debug.Log("new sublevel");
    string[] objSplit = nextObj.Split('-');
    string type = objSplit[0];
    switch (type)
    {
      case "build":
        SM.LevelChecking(false);
        break;
      case "load":
        SM.LevelChecking(true);
        break;
      case "save":
        SM.LevelChecking(false);
        break;
      case "place":
        manager.LoadMolecule(objSplit[1] + "_place", false);
        SM.LevelChecking(false);
        break;
      case "multiple choice":
        SM.LevelChecking(false);
        //AP.Appear(); //make control panel with buttons appear
        getAnswer = true;
        break;
    }
  }

  public int GetCurrentLevel()
  {
    return level;
  }

  public void SetPressedAnswer(string button)
  {
    pressedAnswer = button;
  }

  public void SetLoadedMolecule(GameObject mol)
  {
    StartCoroutine(CheckLoadedMolecule(mol, 0.5f));
  }

  //for after effects
  public bool CheckLevelCompletion()
  {
    levelComplete = LM.LevelCompleted();
    if (!levelComplete)
      CheckNextObjectiveSetup(LM.GetNextObjective());

    IM.UpdateDisplay(LM.GetNextObjective(),false);
    return levelComplete;
  }

  //when a molecule is loaded, there is a small interval to set the id (to not be the default 0) and then when the player presses
  //the check button, it will only check the bool to see if the mol was loaded
  IEnumerator CheckLoadedMolecule(GameObject mol, float delay)
  {
    yield return new WaitForSeconds(delay);

    string nextObj = LM.GetNextObjective();
    string[] objSplit = nextObj.Split('-');
    string text = HandleTextFile.ReadString(objSplit[1] + ".txt");
    if (MM.CompareMoleculeString(text, mol))
      correctMolLoaded = true;
  }

  public void UpdateLevel()
  {
    string nextObj = LM.GetNextObjective();

    if (CheckObjectiveComplete(nextObj))
    {
      LM.UpdateObjective(nextObj);
      CheckLevelCompletion();
      Debug.Log(levelComplete);
    }
  }

}
