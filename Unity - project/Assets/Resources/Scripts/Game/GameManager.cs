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

  //partial molecule
  private GameObject partialGO;
  private string partialName;
  private bool partialCreated;


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
    level = 1;

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

    if (partialCreated && partialGO == null)
      Invoke("RestorePartial",3f);
    
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
    IM.UpdateLevel(LM.GetLevel());
    IM.UpdateDisplay(LM.GetNextObjective(),true);
    if(getAnswer)
      correctAnswerMC = IM.GetCorrectAnswer();
  }

  private void RestorePartial()
  {
    partialGO = manager.LoadMolecule(partialName, false);
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
        if (pressedAnswer.ToLower().Trim() == correctAnswerMC.ToLower().Trim())
        {
          completed = true;
          PS.StopTimer();
        }
        break;
    }
    return completed;
  }

  private void CheckNextObjectiveSetup(string nextObj)
  {
    Debug.Log("new sublevel");
    string[] objSplit = nextObj.Split('>');
    string type = objSplit[0].Trim();
    switch (type)
    {
      case "build":
        partialCreated = false;
        SM.LevelChecking(false);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
        PS.StartMovesCounter();
        break;
      case "complete":
        partialCreated = true;
        SM.LevelChecking(false);
        partialName = GetPartialMolecule(objSplit[1].Trim());
        partialGO = manager.LoadMolecule(partialName, false);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
        PS.StartMovesCounter();
        break;
      case "transform":
        partialCreated = true;
        SM.LevelChecking(false);
        //partialName = GetPartialMolecule(objSplit[2].Trim());
        partialGO = manager.LoadMolecule(objSplit[2].Trim(), false);
        APMultiple.Disappear();
        APSingle.Appear(); //make control panel with buttons appear
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
        partialGO = manager.LoadMolecule(partialName, false);
        SM.LevelChecking(false);
        PS.StartTimer();
        APSingle.Disappear();
        APMultiple.Appear(); //make control panel with buttons appear
        getAnswer = true;
        PS.StartTimer();
        break;
    }
  }

  //for tasks that require a partial molecule to be displayed, that partial has to be loaded
  //if the description is name&class, we get the name and add "_partial"
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
        if (description.Substring(9, description.Length) == "estrutura")
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
      if (rawObj.Substring(9, rawObj.Length) == "estrutura")
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
      string text1 = HandleTextFile.ReadString(name + ".txt");
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
        string text1 = HandleTextFile.ReadString(description + ".txt");
        return MM.CompareMoleculesString(text1, false);
      }
    }

    //with upper case letters and no spaces, it means it is the formule of the molecule (Ex. CH4)
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
