using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
  public string Scene;
  public BlackBoardManager BBManager;

  private int level;
  private bool levelComplete, newLevel;
  private string levelsFile = "";
  private string levels;
  private string levelObjs;
  private string correctAnswerMC, pressedAnswer;
  private Manager manager;
  private LevelManager LM;
  private MoleculeManager MM;
  private ShelfManager SM;
  private InformationManager IM;
  private PointSystem PS;
  private AnswerPanel APMultiple;
  private AnswerPanel APSingle;

  private bool getAnswer;
  private bool correctMolLoaded;

  private string path = "";


  // Use this for initialization
  void Start()
  {
    MM = GetComponent<MoleculeManager>();
    LM = GetComponent<LevelManager>();
    manager = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Manager>();
    BBManager = GameObject.FindGameObjectWithTag("Board").GetComponent<BlackBoardManager>();
    IM = GameObject.Find("Info").GetComponent<InformationManager>();

    PS = GetComponent<PointSystem>();
    GetComponent<Settings>().SetUp();
    GetComponent<BoxAtomsManager>().SetUp();
    //APMultiple = GameObject.Find("ControlPanelAnswers").GetComponent<AnswerPanel>();
    //APSingle = GameObject.Find("ControlPanelAnswer").GetComponent<AnswerPanel>();
    levelComplete = false;
    newLevel = true;
    getAnswer = false;
    correctMolLoaded = false;
    level = 1;
    if (levelsFile != "")
    {
      levels = HandleTextFile.ReadLevels(levelsFile);
      SM = GameObject.FindGameObjectWithTag("shelves").GetComponent<ShelfManager>();
    }
    SoundEffectsManager.SetUp();
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

    /*string molDes = CheckMoleculeDescription("aldeido");
    if (IsMoleculeComplete(molDes))
    {
      Debug.Log("true");
    }*/
  }

  private void SetLevel()
  {

    if (levels != "" && levels != null)
    {
      string objs = "";
      string[] levelsSplit = levels.Split(';');
      for (int i = 0; i < levelsSplit.Length; i++)
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
        string molFile = CheckMoleculeRepresentation(objSplit[1]); //to get the correct name of the file in case the structure is being used
        string text = HandleTextFile.ReadString(molFile + ".txt");
        if (MM.CompareMoleculesString(text, false))
        {
          completed = true;
          //APSingle.Disappear(); //make control panel with buttons disappear
        }
        break;
      case "complete":
        string molDes = CheckMoleculeDescription(objSplit[2]);
        if (IsMoleculeComplete(molDes))
        {
          completed = true;
          //APSingle.Disappear(); //make control panel with buttons disappear
        }
        break;
      case "load":
        if (correctMolLoaded)
        {
          correctMolLoaded = false;
          completed = true;
          //APSingle.Disappear(); //make control panel with buttons disappear
        }
        break;
      case "save":
        string savedText = HandleTextFile.ReadString(objSplit[1] + ".txt");
        if (MM.CompareMoleculesString(savedText, true))
        {
          completed = true;
          //APSingle.Disappear(); //make control panel with buttons disappear
        }
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
          PS.StopTimer();
          //APMultiple.Disappear(); //make control panel with buttons disappear
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
        //APSingle.Appear(); //make control panel with buttons appear
        break;
      case "complete":
        SM.LevelChecking(false);
        //APSingle.Appear(); //make control panel with buttons appear
        break;
      case "transform":
        SM.LevelChecking(false);
        //APSingle.Appear(); //make control panel with buttons appear
        break;
      case "load":
        SM.LevelChecking(true);
        //APSingle.Appear(); //make control panel with buttons appear
        break;
      case "save":
        SM.LevelChecking(false);
        //APSingle.Appear(); //make control panel with buttons appear
        break;
      case "place":
        manager.LoadMolecule(objSplit[1] + "_place", false);
        SM.LevelChecking(false);
        break;
      case "multiple choice":
        SM.LevelChecking(false);
        PS.StartTimer();
        //APMultiple.Appear(); //make control panel with buttons appear
        getAnswer = true;
        break;
    }
  }

  private string CheckMoleculeRepresentation(string rawObj)
  {
    string result = rawObj;
    int length = rawObj.Length;
    if (length > 9)
    {
      result = rawObj.Substring(0, rawObj.Length - 9);
    }
    return result;
  }
  //rawObj can appear in different forms: CH4, CH4estrutura, alcool & etanol, etanol, haloalcano
  private string CheckMoleculeDescription(string rawObj)
  {
    if (rawObj.Contains("&") || !rawObj.Any(char.IsUpper))
    {
      return rawObj;
    }
    else if(rawObj.Length > 9)
    {
      if (rawObj.Substring(9, rawObj.Length) == "estrutura")
        return CheckMoleculeRepresentation(rawObj);
      else return rawObj;
    }
    return "";
  }
  

  private bool IsMoleculeComplete(string description)
  {

    if (description.Contains("&"))
    {
      string[] info = description.Split('&');
      string name = info[1];
      string text1 = HandleTextFile.ReadString(name + ".txt");
      return MM.CompareMoleculesString(text1, false);
    }

    if (!description.Any(char.IsUpper))
    {
      if (MM.CheckMoleculesClass(description))
        return true;
      else
      {
        string text1 = HandleTextFile.ReadString(description + ".txt");
        return MM.CompareMoleculesString(text1, false);
      }
    }

    if (description.Split(' ').Length <= 1)
    {
      string text1 = HandleTextFile.ReadString(description + ".txt");
      return MM.CompareMoleculesString(text1, false);
    }



    return false;
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
    }
  }

  public void SetLevels(string name)
  {
    levelsFile = name;
  }





}
