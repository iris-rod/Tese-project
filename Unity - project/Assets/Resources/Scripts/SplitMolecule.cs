using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SplitMolecule  {

  private static Vector3 initialPosition = new Vector3(0,0,0);

  public static void Split(GameObject molecule, GameObject bond)
  {
    Transform[] atoms = bond.GetComponent<BondController>().GetAtoms();
    Transform ballA = atoms[0];
    Transform ballB = atoms[1];

    //if the split is between two carbon atoms, the molecule is split in half, because the carbons are always the main link
    if(ballA.GetComponent<Atom>().GetAtomType() == "Carbon" && ballB.GetComponent<Atom>().GetAtomType() == "Carbon")
    {
      int bondsA = ballA.GetComponent<Atom>().GetAvailableBonds();
      int bondsB = ballB.GetComponent<Atom>().GetAvailableBonds();
      // if both atoms still have other bonds, it doesnt matter which part should be separated, so it was decided to use atom A
      // if the atom A has fewer bonds than B, then A is the new molecule
      if(bondsA < bondsB || (bondsA > 0 && bondsB > 0) || (bondsA==bondsB)) 
      {
        List<GameObject> bonds = molecule.GetComponent<Molecule>().GetAllBonds(ballA.transform, bond.GetComponent<BondController>().getBondID());
        GameObject newMolecule = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/MoleculeV3"), initialPosition, Quaternion.identity);
        newMolecule.GetComponent<Molecule>().SetHandController(ballA.GetComponent<Atom>().handController);
        SetBonds(bonds,newMolecule);
      }
      // if the atom B has fewer bonds than A, then A is the new molecule
      else if (bondsB < bondsA)
      {
        List<GameObject> bonds = molecule.GetComponent<Molecule>().GetAllBonds(ballB.transform, bond.GetComponent<BondController>().getBondID());
        GameObject newMolecule = GameObject.Instantiate((GameObject)Resources.Load("Prefabs/MoleculeV3"), initialPosition, Quaternion.identity);
        newMolecule.GetComponent<Molecule>().SetHandController(ballA.GetComponent<Atom>().handController);
        SetBonds(bonds, newMolecule);
      }
    }
  }


  private static void SetBonds(List<GameObject> bonds, GameObject molecule)
  {
    for(int i = 0; i < bonds.Count; i++)
    {
      molecule.GetComponent<Molecule>().AddExistingBond(bonds[i]);      
    }
  }

  public static void JoinMolecules(GameObject moleculeA, GameObject moleculeB)
  {
    for(int i = 0; i < moleculeB.transform.childCount; i++)
    {
      Transform child = moleculeB.transform.GetChild(i);
      Debug.Log(child.tag);
      if (child.CompareTag("Bond"))
      {
        Debug.Log("here");
        moleculeA.GetComponent<Molecule>().AddExistingBond(child.gameObject);
      }
    }

    //GameObject.Destroy(moleculeA);
  }



}
