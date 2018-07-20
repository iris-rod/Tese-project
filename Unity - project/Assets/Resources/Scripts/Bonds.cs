using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Properties {

  public static Dictionary<string, int> BONDS = new Dictionary<string, int> { {"Hidrogen",1 }, {"Carbon",4 }, {"Oxigen", 2 }, {"Nitrogen",3 }, {"Iodine",1 }, { "Chlorine", 1 }, { "Fluorine", 1 }, { "Bromine", 1 } };
  public static Dictionary<string, float> VR_SIZES = new Dictionary<string, float> { { "Hidrogen", 0.0375f }, { "Carbon", .06f }, { "Oxigen", .0514f }, { "Nitrogen", .0557f }, { "Iodine", .12f }, { "Chlorine", .0857f }, { "Fluorine", .0429f }, { "Bromine", .0986f } };
  public static Dictionary<string, float> PIVOTDIST = new Dictionary<string, float> { { "Hidrogen", 0.05f }, { "Carbon", 0.05f }, { "Oxigen", 0.05f }, { "Nitrogen", 0.05f }, { "Iodine", 0.05f }, { "Chlorine", 0.05f }, { "Fluorine", 0.05f }, { "Bromine", .115f } };
}
