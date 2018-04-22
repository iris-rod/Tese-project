using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoardManager : MonoBehaviour {

  private Texture molecule1;
  private Texture molecule2;

  // Use this for initialization
  void Start () {
    molecule1 = Resources.Load("Textures/board_A_molecule") as Texture;
    molecule2 = Resources.Load("Textures/board_A_molecule2") as Texture;
  }
	
  public void SetTexture(string value)
  {
    if (value == "1")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = molecule1;
    else if (value == "2")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = molecule2;
  }

	// Update is called once per frame
	void Update () {
		
	}
}
