using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

  private int level;
  private bool levelComplete, newLevel;
  private Manager manager;
  private LevelManager LM;
  private MoleculeManager MM;
  private ShelfManager SM;
  
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
    levelComplete = false;
    newLevel = true;
    correctMolLoaded = false;
    level = 1;
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

    }
    LM.SetLevelId(level);
    LM.SetObjective(objs);
    Invoke("NextLevelDisplay",2f);
    newLevel = false;
  }

  void NextLevelDisplay()
  {
    BBManager.UpdateDisplay(LM.GetLevel(), LM.GetSublevel());
  }

  private bool CheckObjectiveComplete(string nextObj)
  {
    string[] objSplit = nextObj.Split('-');
    string type = objSplit[0];
    bool completed = false;
    switch (type)
    {
      case "build":
        string text = HandleTextFile.ReadString(objSplit[1]+".txt");
        if(MM.CompareMoleculesString(text,false))
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
        manager.LoadMolecule(objSplit[1]+"_place", false);
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
    string nextObj = LM.GetNextObjective();
    string[] objSplit = nextObj.Split('-');
    string text = HandleTextFile.ReadString(objSplit[1] + ".txt");
    if (MM.CompareMoleculeString(text, mol))
      correctMolLoaded = true;
  }

  //for after effects
  public bool CheckLevelCompletion()
  {
    levelComplete = LM.LevelCompleted();
    Debug.Log("success: " + levelComplete);
    if (!levelComplete)
      CheckNextObjectiveSetup(LM.GetNextObjective());
    BBManager.UpdateDisplay(LM.GetLevel(), LM.GetSublevel());
    return levelComplete;
  }

  public void UpdateLevel ()
  {
    string nextObj = LM.GetNextObjective ();

    if (CheckObjectiveComplete (nextObj)) {
      LM.UpdateObjective (nextObj);
      CheckLevelCompletion ();
    }
  }

}
