using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TutorialManager {

  private static int phase;
  private static int objective;
  private static string lastObjective;
  private static bool atomsTouched;

  private static Dictionary<int, List<string>> phasesObjectives;
  private static Dictionary<int, List<string>> phasesObjectivesBoard;
  private static BlackBoardManager BBM;
  private static HandController HC;

  // Use this for initialization
  public static void Start () {
    BBM = GameObject.Find("FrontBoard").GetComponent<BlackBoardManager>();
    HC = GameObject.Find("LeapHandController").GetComponent<HandController>();

    atomsTouched = false;
    phase = 1;
    objective = 0;

    List<string> obj1 = new List<string>() { "grab atoms", "touch atoms", "double bond", "let go"};
    List<string> obj2 = new List<string>() { "grab gear", "grab atom", "move atom" };
    List<string> obj3 = new List<string>() { "grab atoms", "separate"};
    phasesObjectives = new Dictionary<int, List<string>>() { { 1, obj1 }, { 2, obj2 }, { 3, obj3 } };

    List<string> objB1 = new List<string>() { "Agarra em dois átomos, um em cada mão", "Toca os átomos um no outro enquanto os agarras", "Continua a dar toques até teres a ligação correcta", "Larga os átomos" };
    List<string> objB2 = new List<string>() { "Agarra na roda dentada com a mão direita","Agarra num átomo com a mão esquerda. Não largues a roda dentada!", "Mexe o átomo à volta do ponto central da molécula (bola vermelha)" };
    List<string> objB3 = new List<string>() { "Agarra nos dois átomos, um em cada mão", "Afasta os átomos um do outro até a ligação desaparecer" };
    phasesObjectivesBoard = new Dictionary<int, List<string>>() { { 1, objB1 }, { 2, objB2 }, { 3, objB3 } };
  }
	

  public static void CheckObjectiveComplete()
  {
    string obj = phasesObjectives[phase][objective];
    string[] objSplit = obj.Split(' ');

    switch (objSplit[0])
    {
      case "grab":
        if(objSplit[1] == "atom")
        {
          if (HC.GetGrabbedAtoms() == 1)
          {
            lastObjective = objSplit[1];
            objective++;
          }
        }
        else if (objSplit[1] == "atoms")
        {
          if (HC.GetGrabbedAtoms() == 2)
          {
            lastObjective = objSplit[1];
            objective++;
          }
        }
        else if(objSplit[1] == "gear")
        {
          if (HC.GetGrabbedPivot())
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
        break;
      case "let":
        if(HC.GetGrabbedAtoms() ==0)
          objective++;
        break;
    }
    if (objective >= phasesObjectives[phase].Count)
    {
      phase++;
      objective = 0;
    }
    UpdateBoard();
    if (atomsTouched) atomsTouched = false;
  }

  private static void UpdateBoard()
  {
    BBM.UpdateText(phasesObjectivesBoard[phase][objective]);
  }

  public static void SetAtomsTouched(bool touched)
  {
    atomsTouched = touched;
  }

}
