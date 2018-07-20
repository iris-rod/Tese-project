using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBall : MonoBehaviour {

  private string Type;
  private string Levels;
  private GameObject HandController;
  private Leap.Unity.Interaction.InteractionManager Manager;
  
  public void Settings(GameObject controller, Leap.Unity.Interaction.InteractionManager manager, string type, string levels)
  {
    HandController = controller;
    Manager = manager;
    Type = type;
    Levels = levels;
  }

  public string GetNextScene()
  {
    return Type;
  }

  public string GetLevels()
  {
    return Levels;
  }

}
