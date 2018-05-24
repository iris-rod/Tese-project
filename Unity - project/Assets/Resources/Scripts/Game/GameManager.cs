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
  public BlackBoardManager BBManager;

  // Use this for initialization
  void Start()
  {
    MM = GetComponent<MoleculeManager>();
    LM = GetComponent<LevelManager>();
    manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Manager>();
    levelComplete = false;
    newLevel = true;
    level = 0;
  }

  // Update is called once per frame
  void Update()
  {
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
        manager.LoadMolecule("level_1", false);
        BBManager.SetTexture("level_1");
        objs = "build-H20";
        break;
      case 2:
        manager.LoadMolecule("level_2", false);
        BBManager.SetTexture("level_2");
        objs = "build-CO2_place";
        break;
      case 3:
        manager.LoadMolecule("level_3", false);
        BBManager.SetTexture("level_3");
        objs = "build-CH4_save";
        break;
      case 4:
        manager.LoadMolecule("level_4", false);
        BBManager.SetTexture("level_4");
        objs = "load";
        break;
      case 5:
        manager.LoadMolecule("level_5", false);
        BBManager.SetTexture("level_5");
        objs = "build";
        break;

    }
    LM.SetLevelId(level);
    LM.SetObjective(objs);
    newLevel = false;
  }

  private void CheckObjeciveComplete(string nextObj)
  {
    string[] objSplit = nextObj.Split('-');
    string type = objSplit[0];
    switch (type)
    {
      case "build":
        //use compare from mm
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
          invi.GetComponent<InvisibleMoleculeBehaviour>().DestroyOverlap();
          Destroy(invi);
        }
        break;
    }
  }

  public int GetCurrentLevel()
  {
    return level;
  }

  public bool CheckLevelCompletion()
  {
    levelComplete = LM.LevelCompleted();
    return levelComplete;
  }

  public void UpdateLevel()
  {
    string nextObj = LM.GetNextObjective();

    //CheckObjectiveComplete();

    LM.UpdateObjective(nextObj);
    CheckLevelCompletion();
  }

}
