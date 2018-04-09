using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties {

  public static Dictionary<string, int> BONDS = new Dictionary<string, int> { {"Hydrogen",1 }, {"Carbon",4 }, {"Oxygen", 2 }, {"Nytrogen",3 }, {"Sulfur",2 } };
  public static Dictionary<string, float> DESKTOP_SIZES = new Dictionary<string, float> { {"Hydrogen", 0.05f}, { "Carbon", 0.105f }, { "Oxygen", 0.09f }, { "Nytrogen", 0.0975f }, { "Sulfur", 0.15f } };
  public static Dictionary<string, float> VR_SIZES = new Dictionary<string, float> { { "Hydrogen", 0.0375f }, { "Carbon", 0.105f }, { "Oxygen", 0.09f }, { "Nytrogen", 0.0975f }, { "Sulfur", 0.15f } };
  public static Dictionary<string, float> PIVOTDIST = new Dictionary<string, float> { { "Hydrogen", 0.05f }, { "Carbon", 0.05f }, { "Oxygen", 0.05f }, { "Nytrogen", 0.05f }, { "Sulfur", 0.05f } };
}
