using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuBall : MonoBehaviour {

  private string Type;
  private string Levels;
  private GameObject HandController;
  private Leap.Unity.Interaction.InteractionManager Manager;
  private Material highlightGrasp;
  private bool grabbed = false;

  public void Settings(GameObject controller, Leap.Unity.Interaction.InteractionManager manager, string type, string levels)
  {
    HandController = controller;
    Manager = manager;
    Type = type;
    Levels = levels;
    highlightGrasp = transform.GetComponent<MeshRenderer>().materials[2];
  }

  void Update()
  {
     if (GetComponent<InteractionBehaviour>().isGrasped && !grabbed)
      {
        highlightGrasp.SetFloat("_Outline", 0.015f);
        grabbed = true;
      }
      else if (!GetComponent<InteractionBehaviour>().isGrasped)
      {
        highlightGrasp.SetFloat("_Outline", 0.00f);
        grabbed = false;
      }

  }

  public string GetNextScene()
  {
    return Type;
  }

  public string GetLevels()
  {
    return Levels;
  }

  public void Highlight(bool highlight)
  {
    if (highlight)
      highlightGrasp.SetFloat("_Outline", 0.01f);//.001
    else
      highlightGrasp.SetFloat("_Outline", 0.0f);
  }
}
