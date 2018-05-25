using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeManager : MonoBehaviour {

  Dictionary<int, string> moleculesInScene;
  Dictionary<int, string> moleculesInShelf;
  private int numberOfMolecules;
  private int numberOfMinis;

	// Use this for initialization
	void Start () {
    numberOfMolecules = 0;
    numberOfMinis = 0;
    moleculesInScene = new Dictionary<int, string>();
    moleculesInShelf = new Dictionary<int, string>();
  }

	public void AddMolecule (GameObject molecule, bool shelf)
  {
    if (!shelf) {
      numberOfMolecules++;
      molecule.GetComponent<Molecule>().SetId(numberOfMolecules);
    } else {
      numberOfMinis++;
      molecule.GetComponent<Molecule>().SetId(numberOfMinis);
    }
    string text = "";
    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Bond"))
      {
        Debug.Log("here");
        Transform[] atoms = child.GetComponent<BondController>().GetAtoms();
        int type = child.GetComponent<BondController>().GetBondType();
        text += atoms[0].GetComponent<Atom>().GetAtomType() + "-" + type + "-" + atoms[1].GetComponent<Atom>().GetAtomType() + "_"+"\n";
      }
    }
    Debug.Log(text);
    if (!shelf)
      moleculesInScene.Add(numberOfMolecules, text);
    else
      moleculesInShelf.Add(numberOfMinis, text);
  }

  public void UpdateMolecule(GameObject molecule)
  {
    int id = molecule.GetComponent<Molecule>().GetId();
    moleculesInScene.Remove(id);
    numberOfMolecules--;
    AddMolecule(molecule,false);
  }

  public bool CompareMolecules(GameObject mol1, GameObject mol2)
  {
    if (mol1.transform.childCount != mol2.transform.childCount)
      return false;

    string struc1 = moleculesInScene[mol1.GetComponent<Molecule>().GetId()];
    string struc2 = moleculesInScene[mol2.GetComponent<Molecule>().GetId()];
    

    string[] bonds1 = struc1.Split('_');
    string[] bonds2 = struc2.Split('_');

    for (int i = 0; i < bonds1.Length; i++)
    {
      string bond = bonds1[i].Trim();
      bool equal = false;
      for (int j = 0; j < bonds2.Length; j++)
      {
        string bond2 = bonds2[j].Trim();
        if (bond == bond2)
        {
          equal = true;
          break;
        }
        else if(bond.Split('-')[0] == bond2.Split('-')[2] && bond.Split('-')[2] == bond2.Split('-')[0] && (bond.Split('-')[1] == bond2.Split('-')[1]))
        {
          equal = true;
          break;
        }
      }
      if (!equal)
        return false;
    }
    return true;
  }
  
  public bool CompareMoleculesString (string struc2)
  {
  Debug.Log(moleculesInScene.Count);
    foreach (var par in moleculesInScene) {
    
      string struc1 = moleculesInScene [par.Key];
    Debug.Log(par.Key + " ->" +moleculesInScene [par.Key]);
      string[] bonds1 = struc1.Split ('_');
      string[] bonds2 = struc2.Split ('_');
      Debug.Log(struc1);
      Debug.Log(struc2);
      if(bonds1.Length != bonds2.Length)
        return false;
      
      //compare struct of molecules
      for (int i = 0; i < bonds1.Length; i++) {
        string bond = bonds1 [i].Trim ();
        bool equal = false;
        for (int j = 0; j < bonds2.Length; j++) {
          string bond2 = bonds2 [j].Trim ();
          if (bond == bond2) {
            equal = true;
            break;
          } else if (bond.Split ('-') [0] == bond2.Split ('-') [2] && bond.Split ('-') [2] == bond2.Split ('-') [0] && (bond.Split ('-') [1] == bond2.Split ('-') [1])) {
            equal = true;
            break;
          }
        }
        if (equal)
          return true;
      }
    }
    return false;
  }
  
  //function to compare molecule to minis to see if it was saved

  void Update()
  {
    string t1 = "Carbon-2-Oxygen_" + "\n" + "Hydrogen-1-Carbon_" + "\n";
    string t2 = "Hydrogen-1-Carbon_" + "\n" + "Carbon-2-Oxygen_" + "\n";
  }
}
