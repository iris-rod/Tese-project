using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoardManager : MonoBehaviour {

  private Texture molecule1;
  private Texture2D molecule2;
  private Sprite[] level1,level2,level3,level4,level5;
  private Texture2D[] l1,l2,l3,l4,l5;

  private string currentDisplay;
  private string nextDisplay;
  // Use this for initialization
  void Start () {
    molecule1 = Resources.Load("Textures/board_A_molecule") as Texture;
    molecule2 = Resources.Load("Textures/board_A_molecule2") as Texture2D;

    level1 = Resources.LoadAll<Sprite>("Textures/Levels/board_level1_c");
    level2 = Resources.LoadAll<Sprite>("Textures/Levels/board_level2_c");
    level3 = Resources.LoadAll<Sprite>("Textures/Levels/board_level3_c");
    level4 = Resources.LoadAll<Sprite>("Textures/Levels/board_level4_c");
    level5 = Resources.LoadAll<Sprite>("Textures/Levels/board_level5_c");

    l1 = ConvertSpritesToTexture(level1);
    l2 = ConvertSpritesToTexture(level2);
    l3 = ConvertSpritesToTexture(level3);
    l4 = ConvertSpritesToTexture(level4);
    l5 = ConvertSpritesToTexture(level5);

    currentDisplay = "0_0";
    transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l3[0];
  }
	
  private Texture2D[] ConvertSpritesToTexture(Sprite[] sprites)
  {
    Texture2D[] textures = new Texture2D[sprites.Length];

    for (int i = 0; i < sprites.Length; i++)
    {
      Sprite sprite = sprites[i];
      var croppedTexture = new Texture2D((int)sprite.rect.width, (int)sprite.rect.height);
      var pixels = sprite.texture.GetPixels((int)sprite.rect.x,
                                              (int)sprite.rect.y,
                                              (int)sprite.rect.width,
                                              (int)sprite.rect.height);
      croppedTexture.SetPixels(pixels);
      croppedTexture.Apply();
      textures[i] = croppedTexture;
    }
    return textures;
  }

  public void SetTexture(string value)
  {
    if (value == "1")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = molecule1;
    else if (value == "2")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = molecule2;
    else if(value == "1_1-nc")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l1[1];
    else if (value == "1_1-c")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l1[0];

    else if (value == "2_1-nc")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l2[0];
    else if (value == "2_1-c")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l2[1];
    else if (value == "2_2-c")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l2[2];

    else if (value == "3_1-nc")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l3[0];
    else if (value == "3_1-c")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l3[1];
    else if (value == "3_2-c")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l3[2];

    else if (value == "4_1-nc")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l4[0];
    else if (value == "4_1-c")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l4[1];

    else if (value == "5_1-nc")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l5[0];
    else if (value == "5_1-c")
      transform.GetChild(0).GetComponent<MeshRenderer>().material.mainTexture = l5[1];

    currentDisplay = value;
  }

  public void UpdateDisplay(int nextLevel, int nextSublevel)
  {
    int currentLevel = int.Parse(currentDisplay.Split('_')[0]);
    //check if it is the start of the levels
    if (currentLevel == 0)
      SetTexture("1_1-nc");
    else
    {
      //check if display is for new level or for new sublevel
      string[] current = currentDisplay.Split('-');
      //Debug.Log(currentLevel + " " + nextLevel);
      if (currentLevel == nextLevel)
      {
        Debug.Log("new sublevel update: " + current[0]);
        //nextDisplay = nextLevel.ToString() + "_" + nextSublevel.ToString() + "-nc";
        if (current[1] != "c")
          SetTexture(current[0] + "-c");
        else
          SetTexture(nextLevel.ToString() + "_" + (nextSublevel-1).ToString() + "-c");
      }
      else
      {
        Debug.Log("new level update: " + nextLevel + "_" + nextSublevel);
        SetTexture(nextLevel.ToString() + "_" + nextSublevel.ToString() + "-nc");
      }
    }
  }

  void NewDisplay()
  {
    Debug.Log("nextDisplay");
    SetTexture(nextDisplay);
  }

}
