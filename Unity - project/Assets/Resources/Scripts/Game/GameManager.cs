using System;
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
  private float startTimerWait = 3f;

  private string path = "";

  //partial molecule
  private GameObject partialGO;
  private string partialName;
  private bool partialCreated, canCreateNew, restore;

  //teste
  private string currentTask;
  private string fileName = "teste23";

  // Use this for initialization
  void Start()
  {
    MM = GetComponent<MoleculeManager>();
    LM = GetComponent<LevelManager>();
    manager = GetComponent<Manager>();
    BBManager = GameObject.FindGameObjectWithTag("Board").GetComponent<BlackBoardManager>();
    IM = GameObject.Find("Info").GetComponent<InformationManager>();

    PS = GetComponent<PointSystem>();
    if (levelsFile != "")
    {
      levels = HandleTextFile.ReadLevels(levelsFile);

    }
    //APMultiple = GameObject.Find("ControlPanelAnswers").GetComponent<AnswerPanel>();
    //APSingle = GameObject.Find("ControlPanelAnswer").GetComponent<AnswerPanel>();
    levelComplete = false;
    newLevel = true;
    getAnswer = false;
    correctMolLoaded = false;
    partialCreated = false;
    canCreateNew = true;
    restore = false;
    level = 1;
    string[] info = new string[2] {"1","multiple choice" };
    Logs.BeginFile(fileName, info);
    SoundEffectsManager.SetUp();
  }

  public void SetShelves(GameObject obj)
  {
    SM = obj.GetComponent<ShelfManager>();
  }

  public void SetPanelAnswer(GameObject panel, bool mul)
  {
    if (mul) APMultiple = panel.GetComponent<AnswerPanel>();
    else APSingle = panel.GetComponent<AnswerPanel>();
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

    if (restore && partialGO == null)
    {
      partialGO = manager.LoadMolecule(partialName, false);
      restore = false;
    }

    

  }

  private void SetLevel()
  {
    bool hasNewLevel = false;
    if (levels != "" && levels != null)
    {
      string objs = "";
      string[] levelsSplit = levels.Split(';');
      for (int i = 0; i < levelsSplit.Length; i++)
      {
        string[] levelDescp = levelsSplit[i].Split(':');
        int levelID = 0;
        try
        {
          levelID = int.Parse(levelDescp[0].Trim());
        }
        catch (Exception e) { }
        if (level == levelID)
        {
          objs = levelDescp[1].Trim();
          hasNewLevel = true;
          break;
        }
      }

      if (hasNewLevel)
      {
        string moves = PS.GetMoves().ToString();
        string time = PS.GetTime();
        string wrongAnswers = PS.GetWrongAnswers().ToString();
        LM.SetLevelId(level);
        LM.SetObjective(objs);
        CheckNextObjectiveSetup(LM.GetNextObjective());
        Invoke("NextLevelDisplay", 3f);
        newLevel = false;
        //log
        string[] info = new string[6] { level.ToString(), currentTask, moves, time, wrongAnswers, PS.GetPoints().ToString() };
        Logs.EndLevel(fileName, info);
      }
      else
      {
        IM.SetFinalDisplay(PS.GetPoints());
        string moves = PS.GetMoves().ToString();
        string time = PS.GetTime();
        string wrongAnswers = PS.GetWrongAnswers().ToString();
        string[] info = new string[6] { level.ToString(), currentTask, moves, time, wrongAnswers, PS.GetPoints().ToString() };
        Logs.EndLevel(fileName, info);
        newLevel = false;
      }
    }

  }

  void NextLevelDisplay()
  {
    IM.UpdateLevel(LM.GetLevel());
    IM.UpdateDisplay(LM.GetNextObjective(), PS.GetPoints()); //true
    SetupNextObjective();
    //if the next task is multiple choice, the correct answer is fetched from the info to compare when the player choses an answer
    if (getAnswer)
    {
      Debug.Log("here");
      //Invoke("StartAnswersCounter", startTimerWait); //start timer after the information about points is removed
      correctAnswerMC = IM.GetCorrectAnswer();
    }
  }

  private void NewTaskDisplay()
  {
    IM.UpdateDisplay(LM.GetNextObjective(), PS.GetPoints()); //false
    if (getAnswer)
    {
      Debug.Log("here");
      //Invoke("StartAnswersCounter", startTimerWait); //start timer after the information about points is removed
      correctAnswerMC = IM.GetCorrectAnswer();
    }
  }


  private void StartTimer()
  {
    PS.StartTimer();
  }

  private void PlacePartialMolecule()
  {
    if (partialCreated && partialGO == null)
    {
      partialGO = manager.LoadMolecule(partialName, false);
    }
  }


  private bool CheckObjectiveComplete(string nextObj)
  {
    string[] objSplit = nextObj.Split('>');
    
    string type = objSplit[0].Trim();
    bool completed = false;
    switch (type)
    {
      case "build":
        string molFile = CheckMoleculeRepresentation(objSplit[1].Trim()); //to get the correct name of the file in case the structure is being used
        string text = HandleTextFile.ReadString(molFile + "Bonds.txt");
        if (MM.CompareMoleculesString(text, false))
        {
          completed = true;
          PS.StopMovesCounter();
        }
        break;
      case "complete":
        string molDes = CheckMoleculeDescription(objSplit[1].Trim());
        if (IsMoleculeComplete(molDes))
        {
          completed = true;
          PS.StopMovesCounter();
        }
        break;
      case "transform":
        string molDes1 = CheckMoleculeDescription(objSplit[1].Trim());
        if (IsMoleculeComplete(molDes1))
        {
          completed = true;
          PS.StopMovesCounter();
        }
        break;
      case "load":
        if (correctMolLoaded)
        {
          correctMolLoaded = false;
          completed = true;
        }
        break;
      case "save":
        string savedText = HandleTextFile.ReadString(objSplit[1].Trim() + ".txt");
        if (MM.CompareMoleculesString(savedText, true))
        {
          completed = true;
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
        Debug.Log(pressedAnswer);
        Debug.Log(correctAnswerMC);
        if (pressedAnswer.ToLower().Trim() == correctAnswerMC.ToLower().Trim())
        {
          completed = true;
          PS.UpdateAnswers();
          PS.StopAnswersCounter();
          RemoveAllMolecules();
        }
        PS.UpdateAnswers();
        break;
    }
    return completed;
  }

  public void RemoveAllMolecules()
  {
    GameObject[] molecules = GameObject.FindGameObjectsWithTag("Molecule");
    for(int i = 0; i < molecules.Length; i++)
    {
      MM.RemoveMolecule(molecules[i]);
      Destroy(molecules[i]);
    }
  }

  private void CheckNextObjectiveSetup(string nextObj)
  {
    string[] objSplit = nextObj.Split('>');
    string type = objSplit[0].Trim();
    currentTask = type;
    switch (type)
    {
      case "build":
        partialCreated = false;
        getAnswer = false;
        SM.LevelChecking(false);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
        PS.StartMovesCounter();
        CheckMininumMoves(objSplit[1].Trim());
        break;
      case "complete":
        partialCreated = true;
        getAnswer = false;
        SM.LevelChecking(false);
        partialName = GetPartialMolecule(objSplit[1].Trim());
        CheckMininumMoves(objSplit[1].Trim());
        //partialGO = manager.LoadMolecule(partialName, false);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
        PS.StartMovesCounter();
        break;
      case "transform":
        partialCreated = true;
        getAnswer = false;
        SM.LevelChecking(false);
        partialName = objSplit[2].Trim();
        //partialGO = manager.LoadMolecule(objSplit[2].Trim(), false);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
        CheckMininumMoves(objSplit[1].Trim());
        PS.StartMovesCounter();
        break;
      case "load":
        partialCreated = false;
        SM.LevelChecking(true);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
        break;
      case "save":
        partialCreated = false;
        SM.LevelChecking(false);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
        break;
      case "place":
        partialCreated = false;
        manager.LoadMolecule(objSplit[1].Trim() + "_place", false);
        SM.LevelChecking(false);
        break;
      case "multiple choice":
        partialCreated = true;
        partialName = objSplit[2].Trim();
        //partialGO = manager.LoadMolecule(partialName, false);
        SM.LevelChecking(false);
        APSingle.Disappear();
        APMultiple.Appear(); //make control panel with buttons appear
        PS.StartAnswersCounter();
        getAnswer = true;
        break;
    }
  }

  private void CheckMininumMoves(string molecule)
  {
    if (molecule == "CH4")
      PS.SetMinimumMoves(9);
    else if (molecule == "dimetilbutanoEstrutura")
      PS.SetMinimumMoves(24);
    else if (molecule == "CH3COOHEstrutura")
      PS.SetMinimumMoves(15);
    else if (molecule == "alcool")
      PS.SetMinimumMoves(6);
    else if (molecule == "haloalcano")
      PS.SetMinimumMoves(6);
    else if (molecule == "3-metilpentano")
    {
      PS.SetMinimumMoves(16);
    }
    
  }

  //for tasks that require a partial molecule to be displayed, that partial has to be loaded
  //if the description is name&class, we get the name and add "Partial"
  //if it has upper case it means it is a structure or formule
  private string GetPartialMolecule(string description)
  {
    string result = description;
    if (description.Contains("&"))
    {
      string[] info = description.Split('&');
      return info[1] + "Partial";
    }
    else if (MoleculesCharacteristics.CheckIfIsClass(description))
      return "partial_default";
    else if (description.Any(char.IsUpper))
    {
      if (description.Length > 9)
      {
        if (description.Substring(description.Length-9).ToLower() == "estrutura")
          return description.Substring(0, description.Length - 9) + "Partial";
      }
    }
    return result;

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
      if (rawObj.Substring(rawObj.Length-9).ToLower() == "estrutura")
        return CheckMoleculeRepresentation(rawObj);
      else return rawObj;
    }
    return "";
  }
  

  private bool IsMoleculeComplete(string description)
  {
    //with two names describing the molecule, the first has to be the name and the second the class (it only really needs the name)
    if (description.Contains("&"))
    {
      string[] info = description.Split('&');
      string name = info[1];
      string text1 = HandleTextFile.ReadString(name + "Bonds.txt");
      return MM.CompareMoleculesString(text1, false);
    }

    //with no upper case letters, it could be used just the name or the class.
    //for the class, it is called CheckMoleculesClass, and for the name just read the text file
    if (!description.Any(char.IsUpper))
    {
      if (MM.CheckMoleculesClass(description))
        return true;
      else
      {
        string text1 = HandleTextFile.ReadString(description + "Bonds.txt");
        return MM.CompareMoleculesString(text1, false);
      }
    }

    //with upper case letters and no spaces, it means it is the formule of the molecule (Ex. CH4)
    if (description.Split(' ').Length <= 1)
    {
      string text1 = HandleTextFile.ReadString(description + "Bonds.txt");
      return MM.CompareMoleculesString(text1, false);
    }
    return false;
  }

  private void SetupNextObjective()
  {
    RemoveAllMolecules();
    Invoke("PlacePartialMolecule", .5f);
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
    {
      string moves = PS.GetMoves().ToString();
      string time = PS.GetTime();
      string wrongAnswers = PS.GetWrongAnswers().ToString();
      CheckNextObjectiveSetup(LM.GetNextObjective());
      SetupNextObjective(); //remove all previous molecules and set the needed one for the next task

      string[] info = new string[6] { level.ToString(), currentTask, moves, time, wrongAnswers, PS.GetPoints().ToString() };
      Logs.EndTask(fileName, info);

      Invoke("NewTaskDisplay", 3f);
    }


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

  public void UpdatePointSystem()
  {
    PS.UpdateMoves();
  }

  //used when all the molecules are removed from the scene
  //if the current task has a partial molecule to be displayed, then when
  //the molecules are removed from the scene, this one must be replaced
  public void RestorePartial()
  {
    if (partialCreated)
    {
      restore = true;
    }
  }

}
