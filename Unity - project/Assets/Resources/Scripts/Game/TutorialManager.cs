using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialManager {

  private static int phase;
  private static int objective;
  private static string lastObjective;
  private static bool atomsTouched;
  private static bool bondCreated, bondBroken;
  private static bool pivotGrabbed;
  private static bool moleculeRotated;
  private static int atomsGrabbed;

  private static Dictionary<int, List<string>> phasesObjectives;
  private static Dictionary<int, List<string>> phasesObjectivesBoard;
  private static BlackBoardManager BBM;
  private static HandController HC;

  // Use this for initialization
  public static void Start () {
    BBM = GameObject.Find("FrontBoard").GetComponent<BlackBoardManager>();
    HC = GameObject.Find("LeapHandController").GetComponent<HandController>();

    atomsTouched = false;
    moleculeRotated = false;
    pivotGrabbed = false;
    bondBroken = false;
    atomsGrabbed = 0;
    phase = 1;
    objective = 0;

    List<string> obj1 = new List<string>() { "grab atoms", "touch atoms", "bond double", "let go"};
    List<string> obj2 = new List<string>() { "grab gear", "grab atom", "move atom" };
    List<string> obj3 = new List<string>() { "grab atoms", "separate"};
    phasesObjectives = new Dictionary<int, List<string>>() { { 1, obj1 }, { 2, obj2 }, { 3, obj3 } };

    List<string> objB1 = new List<string>() { "Agarra em dois átomos de carbono, um em cada mão", "Toca os átomos um no outro enquanto os agarras", "Continua a dar toques até teres uma ligação tripla", "Larga os átomos" };
    List<string> objB2 = new List<string>() { "Agarra na roda dentada com a mão direita","Agarra num átomo com a mão esquerda. Não largues a roda dentada!", "Mexe o átomo à volta do ponto central da molécula (bola vermelha)" };
    List<string> objB3 = new List<string>() { "Agarra nos dois átomos, um em cada mão", "Afasta os átomos um do outro até a ligação desaparecer" };
    phasesObjectivesBoard = new Dictionary<int, List<string>>() { { 1, objB1 }, { 2, objB2 }, { 3, objB3 } };
  }
	

  public static void CheckObjectiveComplete()
  {
    string obj = phasesObjectives[phase][objective];
    string[] objSplit = obj.Split(' ');
    Debug.Log("grabbed: " + atomsGrabbed);
    switch (objSplit[0])
    {
      case "grab":
        if(objSplit[1].Trim() == "atom")
        {
          if (atomsGrabbed == 1)
          {
            lastObjective = objSplit[1];
            objective++;
          }
        }
        else if (objSplit[1].Trim() == "atoms")
        {
          if (atomsGrabbed == 2)
          {
            lastObjective = objSplit[1];
            objective++;
          }
        }
        else if(objSplit[1].Trim() == "gear")
        {
          if (pivotGrabbed)
          {
            lastObjective = objSplit[1];
            objective++;
          } 
        }
        break;
      case "touch":
        if (atomsTouched)
          objective++;
        break;
      case "move":
        if (moleculeRotated)
          objective++;
        break;
      case "let":
        if(atomsGrabbed == 0)
          objective++;
        break;
      case "bond":
        if (bondCreated)
          objective++;
        break;
      case "separate":
        if (bondBroken)
          objective++;
        break;
    }
    if (objective >= phasesObjectives[phase].Count)
    {
      phase++;
      objective = 0;
    }
    if (atomsTouched) atomsTouched = false;
    if (bondCreated) bondCreated = false;
    if (pivotGrabbed) pivotGrabbed = false;
    if (moleculeRotated) moleculeRotated = false;
    if (bondBroken) bondBroken = false;

    if (phase > phasesObjectives.Count) {

      string space = "\n" + "\n" + "\n";
      BBM.UpdateText(space + "Parabéns, terminaste o tutorial com sucesso!" + "\n" + "Carrega no botão que está em cima da mesa no lado esquerdo para voltares ao menu principal");
    }
    else
      UpdateBoard();
  }


  private static void UpdateBoard()
  {
    string space = "\n" + "\n" + "\n";
    string title = "Criar molécula " + (objective + 1) + "\\" + phasesObjectivesBoard[phase].Count ;
    if (phase == 2)
      title = "Rodar molécula";
    else if (phase == 3)
      title = "Quebrar ligação";
    BBM.UpdateText(space + title + "\n"+ (objective + 1) + " - " + phasesObjectivesBoard[phase][objective]);
  }

  public static void SetAtomsTouched(bool touched)
  {
    atomsTouched = touched;
  }

  public static void SetGrabbedAtoms(int count)
  {
    atomsGrabbed = count;
  }

  public static void SetBondCreated(bool value)
  {
    bondCreated = value;
  }

  public static void SetPivotGrabbed(bool value)
  {
    pivotGrabbed = value;
  }

  public static void SetMoleculeRotated(bool value)
  {
    moleculeRotated = value;
  }


  public static void SetBondBroken(bool value)
  {
    bondBroken = value;
  }

  public static void Reset()
  {
    atomsTouched = false;
    moleculeRotated = false;
    pivotGrabbed = false;
    atomsGrabbed = 0;
    phase = 1;
    objective = 0;
  }

}
