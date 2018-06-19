using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  private int level;
  private bool levelComplete, newLevel;
  private string levels;
  private string levelObjs;
  private Manager manager;
  private LevelManager LM;
  private MoleculeManager MM;
  private ShelfManager SM;
  private InformationManager IM;


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
    levelComplete = false;
    newLevel = true;
    correctMolLoaded = false;
    level = 1;
    levels = HandleTextFile.ReadLevels("levels_default");
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

    /*
    switch (level)
    {
      case 1:
        objs = "build-H2O";
        break;
      case 2:
        objs = "build-CO2_place-CO2";
        break;
      case 3:
        objs = "build-CH4_save-CH4";
        break;
      case 4:
        objs = "load-CH4";
        break;
      case 5:
        objs = "build-C2H4O2";
        break;

    }*/
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
        //updateBoard
        SM.LevelChecking(false);
        break;
      case "load":
        SM.LevelChecking(true);
        break;
      case "save":
        //updateBoard
        SM.LevelChecking(false);
        break;
      case "place":
        manager.LoadMolecule(objSplit[1] + "_place", false);
        SM.LevelChecking(false);
        break;
    }
  }

  public int GetCurrentLevel()
  {
    return level;
  }

  public void SetLoadedMolecule(GameObject mol)
  {
    StartCoroutine(CheckLoadedMolecule(mol, 0.5f));
  }

  //for after effects
  public bool CheckLevelCompletion()
  {
    levelComplete = LM.LevelCompleted();
    Debug.Log("success: " + levelComplete);
    if (!levelComplete)
      CheckNextObjectiveSetup(LM.GetNextObjective());
    //BBManager.UpdateDisplay(LM.GetLevel(), LM.GetSublevel());
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
    }
  }

}
