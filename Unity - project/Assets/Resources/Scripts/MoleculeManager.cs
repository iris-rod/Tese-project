﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleculeManager : MonoBehaviour {

  Dictionary<int, string> moleculesInScene;
  Dictionary<int, string> moleculesInShelf;
  Dictionary<int, GameObject> molecules;

  private int numberOfMolecules;
  private int numberOfMinis;

  private LogManager LM;

	// Use this for initialization
	void Start () {
    numberOfMolecules = 0;
    numberOfMinis = 0;
    moleculesInScene = new Dictionary<int, string>();
    moleculesInShelf = new Dictionary<int, string>();
    molecules = new Dictionary<int, GameObject>();
  }

  public void SetUp()
  {
    LM = GameObject.Find("LogSheet").GetComponent<LogManager>();
  }

  public void AddMolecule(GameObject molecule, bool shelf)
  {
    if (!shelf)
    {
      molecule.GetComponent<Molecule>().SetId(numberOfMolecules);
    }
    else
    {
      molecule.GetComponent<Molecule>().SetId(numberOfMinis);
    }
    string text = "";
    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Bond"))
      {
        Transform[] atoms = child.GetComponent<BondController>().GetAtoms();
        int type = child.GetComponent<BondController>().GetBondType();
        text += OrderAtoms(atoms[0], atoms[1], type);
      }
    }

    if (!shelf)
    {
      moleculesInScene.Add(numberOfMolecules, text);
      molecules.Add(numberOfMolecules, molecule);
      numberOfMolecules++;
    }
    else
    {
      moleculesInShelf.Add(numberOfMinis, text);
      numberOfMinis++;
    }
  }

  // to log a molecule created, it has to be a molecule that exists, not an incomplete one
  // returns true if it can be logged, and false if it can't
  public void CheckAvailableBonds(GameObject molecule)
  {
    for(int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.gameObject.CompareTag("Interactable"))
      {
        if (child.gameObject.GetComponent<Atom>().GetAvailableBonds() > 0)
          return;
      }
    }
    LM.AddLog("Built "+ GetMoleculeName(molecule)+ " molecule");
  }

  public void UpdateMolecule(GameObject molecule)
  {
    int id = molecule.GetComponent<Molecule>().GetId();
    if (moleculesInScene.ContainsKey(id))
    {
      moleculesInScene.Remove(id);
      molecules.Remove(id);
      numberOfMolecules--;
      AddMolecule(molecule, false);
      CheckAvailableBonds(molecule);
    }
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
      if (bond != "") { 
        for (int j = 0; j < bonds2.Length; j++)
        {
          string bond2 = bonds2[j].Trim();
          if (bond == bond2)
          {
            equal = true;
            break;
          }
          else if (bond.Split('-')[0] == bond2.Split('-')[2] && bond.Split('-')[2] == bond2.Split('-')[0] && (bond.Split('-')[1] == bond2.Split('-')[1]))
          {
            equal = true;
            break;
          }
        }
        if (!equal)
          return false;
      }
    }
    return true;
  }
  
  public bool CompareMoleculesString (string struc2, bool inShelf)
  {
    Dictionary<int, string> molecules = moleculesInScene;
    string[] bonds2 = struc2.Trim().Split('_');
    if (struc2 != "")
    {
      if (inShelf) molecules = moleculesInShelf;
      foreach (var par in molecules)
      {

        string struc1 = molecules[par.Key];
        string[] bonds1 = struc1.Trim().Split('_');
        if (bonds1.Length != bonds2.Length)
          continue;
        //compare struct of molecules
        for (int i = 0; i < bonds1.Length; i++)
        {
          string bond = bonds1[i].Trim();
          bool equal = false;
          if (bond != "")
          {
            for (int j = 0; j < bonds2.Length; j++)
            {
              string bond2 = bonds2[j].Trim();
              if (bond == bond2)
              {
                equal = true;
                break;
              }
              else if (bond.Split('-')[0] == bond2.Split('-')[2] && bond.Split('-')[2] == bond2.Split('-')[0] && (bond.Split('-')[1] == bond2.Split('-')[1]))
              {
                equal = true;
                break;
              }
            }
          }
          if (equal)
            return true;
        }
      }
    }
    return false;
  }

  public bool CompareMoleculeString(string struc2, GameObject mol)
  {

      string struc1 = moleculesInScene[mol.GetComponent<Molecule>().GetId()];
      string[] bonds1 = struc1.Trim().Split('_');
      string[] bonds2 = struc2.Trim().Split('_');
    if (bonds1.Length != bonds2.Length)
      return false;
      //compare struct of molecules
      for (int i = 0; i < bonds1.Length; i++)
      {
        string bond = bonds1[i].Trim();
        bool equal = false;
        if (bond != "")
        {
          for (int j = 0; j < bonds2.Length; j++)
          {
            string bond2 = bonds2[j].Trim();
            if (bond == bond2)
            {
              equal = true;
              break;
            }
            else if (bond.Split('-')[0] == bond2.Split('-')[2] && bond.Split('-')[2] == bond2.Split('-')[0] && (bond.Split('-')[1] == bond2.Split('-')[1]))
            {
              equal = true;
              break;
            }
        }
        }
        if (!equal && bond != "")
          return false;
      }
    return true;
  }

  public void Clear()
  {
    moleculesInShelf = new Dictionary<int, string>();
    moleculesInScene = new Dictionary<int, string>();
    molecules = new Dictionary<int, GameObject>();
    numberOfMinis = 0;
    numberOfMolecules = 0;
  }

  public void RemoveMolecule(GameObject mol)
  {
    int molID = mol.GetComponent<Molecule>().GetId();
    moleculesInScene.Remove(molID);
    molecules.Remove(molID);
  }

  private string OrderAtoms(Transform atom1, Transform atom2, int type)
  {
    string name1 = atom1.GetComponent<Atom>().GetAtomType();
    string name2 = atom2.GetComponent<Atom>().GetAtomType();

    if (name1 == "Hidrogen")
      return name2 + "-" + type + "-" + name1 + "_" + "\n";
    else if (name2 == "Hidrogen")
      return name1 + "-" + type + "-" + name2 + "_" + "\n";
    else if (name1 == "Carbon")
      return name1 + "-" + type + "-" + name2 + "_" + "\n";
    else if (name2 == "Carbon")
      return name2 + "-" + type + "-" + name1 + "_" + "\n";
    else
      return name1 + "-" + type + "-" + name2 + "_" + "\n";
  }


  private string GetMoleculeName(GameObject molecule)
  {
    string result = "";
    int C = 0;
    int H = 0;
    int O = 0;
    for (int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.gameObject.CompareTag("Interactable"))
      {
        string type = child.gameObject.GetComponent<Atom>().GetAtomType();
        switch (type)
        {
          case "Hydrogen":
            H++;
            break;
          case "Carbon":
            C++;
            break;
          case "Oxygen":
            O++;
            break;
        }
      }
    }
    if(O != 0)
      result = "C" + C.ToString() + "H" + H.ToString() + "O" + O.ToString();
    else
      result = "C" + C.ToString() + "H" + H.ToString();
    return result;
  }

  public bool CheckMoleculesClass(string taskDescription)
  {

    foreach(var par in moleculesInScene)
    {
      GameObject molecule = molecules[par.Key];
      if (MoleculesCharacteristics.CheckTheClass(taskDescription, molecule))
        return true;
      else
        if(MoleculesCharacteristics.CheckTheClass(taskDescription, par.Value))
          return true;
    }
    return false;
  }

}
