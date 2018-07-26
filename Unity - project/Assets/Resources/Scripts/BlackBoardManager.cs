using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BlackBoardManager : MonoBehaviour {

  private Texture mainMenuText;
  private Texture2D gameplayText;
  private GameObject board;

  private Sprite[] level1,level2,level3,level4,level5;
  private Texture2D[] l1,l2,l3,l4,l5;

  private string Scene = "MainMenu";
  private TextMeshPro information;
  private bool changeScene;

  // Use this for initialization
  void Start () {
    changeScene = true;
    mainMenuText = Resources.Load("Textures/board_B") as Texture;
    gameplayText = Resources.Load("Textures/front_board") as Texture2D;
    board = transform.Find("Board").gameObject;

    level1 = Resources.LoadAll<Sprite>("Textures/Levels/board_level1_c");

    l1 = ConvertSpritesToTexture(level1);
    information = transform.Find("Instructions").GetChild(0).GetComponent<TextMeshPro>();

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

  public void SetScene(string s)
  {
    Scene = s;
    changeScene = true;
  }

  void Update()
  {
    if (changeScene)
    {
      if (Scene == "MainMenu")
      {
        board.GetComponent<MeshRenderer>().material.mainTexture = mainMenuText;
        information.text = "Bem vindo!" + "\n" + "Escolhe o modo de jogo que queres jogar colocando a bola que está em cima da caixa na plataforma!";
      }
      else if (Scene == "Gameplay")
      {
        board.GetComponent<MeshRenderer>().material.mainTexture = gameplayText;
        information.text = "";
      }
      else if (Scene == "Tutorial")
      {
        board.GetComponent<MeshRenderer>().material.mainTexture = mainMenuText;
      }
      changeScene = false;
    }
    if(Scene=="Tutorial")
      TutorialManager.CheckObjectiveComplete();
  }

  public void UpdateText(string s)
  {
    information.text = s;
  }

  /*
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
  */

}
