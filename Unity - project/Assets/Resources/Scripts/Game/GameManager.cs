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
  
  private string path = "";
  
  public BlackBoardManager BBManager;

  // Use this for initialization
  void Start()
  {
    MM = GetComponent<MoleculeManager>();
    LM = GetComponent<LevelManager>();
    SM = GameObject.FindGameObjectWithTag("shelves").GetComponent<ShelfManager>();
    manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Manager>();
    levelComplete = false;
    newLevel = true;
    level = 1;
  }

  // Update is called once per frame
  void Update()
  {
  Debug.Log("level: " + level);
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
    string objs = "";
    switch (level)
    {
      case 1:
        //manager.LoadMolecule("level_1", false);
        //BBManager.SetTexture("level_1");
        objs = "build-H2O";
        break;
      case 2:
        //manager.LoadMolecule("level_2", false);
        //BBManager.SetTexture("level_2");
        objs = "build-CO2_place";
        break;
      case 3:
        //manager.LoadMolecule("level_3", false);
        //BBManager.SetTexture("level_3");
        objs = "build-CH4_save";
        break;
      case 4:
        //manager.LoadMolecule("level_4", false);
        //BBManager.SetTexture("level_4");
        objs = "load-CH4";
        break;
      case 5:
        //manager.LoadMolecule("level_5", false);
        //BBManager.SetTexture("level_5");
        objs = "build-C2H4O2";
        break;

    }
    LM.SetLevelId(level);
    LM.SetObjective(objs);
    newLevel = false;
  }

  private bool CheckObjectiveComplete(string nextObj)
  {
    //Debug.Log(nextObj);
    string[] objSplit = nextObj.Split('-');
    string type = objSplit[0];
    bool completed = false;
    switch (type)
    {
      case "build":
        string text = HandleTextFile.ReadString(objSplit[1]+".txt");
        //Debug.Log(objSplit[1]);
        if(MM.CompareMoleculesString(text))
          completed = true;
        break;
      case "load":

        break;
      case "save":
        //check shelf
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

  public int GetCurrentLevel()
  {
    return level;
  }

  //for after effects
  public bool CheckLevelCompletion()
  {
    levelComplete = LM.LevelCompleted();
    Debug.Log("success: " + levelComplete);
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
