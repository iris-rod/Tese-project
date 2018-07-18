using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Settings : MonoBehaviour {

  public bool English;

  private static Material CarbonBoxEn;
  private static Material HidrogenBoxEn;
  private static Material OxigenBoxEn;
  private static Material BromineBoxEn;
  private static Material ChlorineBoxEn;
  private static Material FluorineBoxEn;
  private static Material NitrogenBoxEn;
  private static Material IodineBoxEn;

  private static Material CarbonBoxPT;
  private static Material HidrogenBoxPT;
  private static Material OxigenBoxPT;
  private static Material BromineBoxPT;
  private static Material ChlorineBoxPT;
  private static Material FluorineBoxPT;
  private static Material NitrogenBoxPT;
  private static Material IodineBoxPT;

  public GameObject CarbonBox;
  public GameObject HidrogenBox;
  public GameObject OxigenBox;
  public GameObject BromineBox;
  public GameObject ChlorineBox;
  public GameObject FluorineBox;
  public GameObject NitrogenBox;
  public GameObject IodineBox;


  private Texture2D[] AtomsLetters;

  // Use this for initialization
  void Start () {
    CarbonBoxEn   = Resources.Load("Materials/Boxes/CarbonBoxEn", typeof(Material)) as Material;
    HidrogenBoxEn = Resources.Load("Materials/Boxes/HydrogenBoxEn", typeof(Material)) as Material;
    OxigenBoxEn   = Resources.Load("Materials/Boxes/OxygenBoxEn", typeof(Material)) as Material;
    BromineBoxEn  = Resources.Load("Materials/Boxes/BromineBoxEn", typeof(Material)) as Material;
    ChlorineBoxEn = Resources.Load("Materials/Boxes/ChlorineBoxEn", typeof(Material)) as Material;
    FluorineBoxEn = Resources.Load("Materials/Boxes/FluorineBoxEn", typeof(Material)) as Material;
    NitrogenBoxEn = Resources.Load("Materials/Boxes/NitrogenBoxEn", typeof(Material)) as Material;
    IodineBoxEn   = Resources.Load("Materials/Boxes/IodineBoxEn", typeof(Material)) as Material;

    CarbonBoxPT   = Resources.Load("Materials/Boxes/CarbonBoxPT", typeof(Material)) as Material;
    HidrogenBoxPT = Resources.Load("Materials/Boxes/HydrogenBoxPT", typeof(Material)) as Material;
    OxigenBoxPT   = Resources.Load("Materials/Boxes/OxygenBoxPT", typeof(Material)) as Material;
    BromineBoxPT  = Resources.Load("Materials/Boxes/BromineBoxPT", typeof(Material)) as Material;
    ChlorineBoxPT = Resources.Load("Materials/Boxes/ChlorineBoxPT", typeof(Material)) as Material;
    FluorineBoxPT = Resources.Load("Materials/Boxes/FluorineBoxPT", typeof(Material)) as Material;
    NitrogenBoxPT = Resources.Load("Materials/Boxes/NitrogenBoxPT", typeof(Material)) as Material;
    IodineBoxPT   = Resources.Load("Materials/Boxes/IodineBoxPT", typeof(Material)) as Material;

    Sprite[] allLetters = Resources.LoadAll<Sprite>("Textures/atoms letters");
    AtomsLetters = ConvertSpritesToTexture(allLetters);
    SetBoxes(English);
  }

  public Texture2D GetAtomTexture(string atomType)
  {
    Texture2D text = AtomsLetters[0];
    switch (atomType)
    {
      case "Oxygen":
        text = AtomsLetters[0];
        break;
      case "Hydrogen":
        text = AtomsLetters[7];
        break;
      case "Carbon":
        text = AtomsLetters[6];
        break;
      case "Nitrogen":
        text = AtomsLetters[2];
        break;
      case "Fluorine":
        text = AtomsLetters[1];
        break;
      case "Chlorine":
        text = AtomsLetters[4];
        break;
      case "Bromine":
        text = AtomsLetters[5];
        break;
      case "Iodine":
        text = AtomsLetters[3];
        break;
    }
    return text;
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

  private void SetBoxes(bool en)
  {
    if (en)
    {
      CarbonBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = CarbonBoxEn;
      HidrogenBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = HidrogenBoxEn;
      OxigenBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = OxigenBoxEn;
      BromineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = BromineBoxEn;
      ChlorineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = ChlorineBoxEn;
      FluorineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = FluorineBoxEn;
      NitrogenBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = NitrogenBoxEn;
      IodineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = IodineBoxEn;
    }else
    {
      CarbonBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = CarbonBoxPT;
      HidrogenBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = HidrogenBoxPT;
      OxigenBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = OxigenBoxPT;
      BromineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = BromineBoxPT;
      ChlorineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = ChlorineBoxPT;
      FluorineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = FluorineBoxPT;
      NitrogenBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = NitrogenBoxPT;
      IodineBox.transform.GetChild(1).GetComponent<MeshRenderer>().material = IodineBoxPT;
    }
  }
}
