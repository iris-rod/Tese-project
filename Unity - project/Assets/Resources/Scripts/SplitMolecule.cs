using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SplitMolecule  {

  public static void Split(GameObject molecule, GameObject bond)
  {
    Transform[] atoms = bond.GetComponent<BondController>().GetAtoms();
    Transform ballA = atoms[0];
    Transform ballB = atoms[1];
    if(ballA.GetComponent<Atom>().GetAtomType() == "Carbon" && ballB.GetComponent<Atom>().GetAtomType() == "Carbon")
    {
      //separar ballA do resto, incluindo os bonds e os atomos ligados a ela
    }
  }

}
