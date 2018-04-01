using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour{

  public GameObject moleculePrefab;
  public GameObject atomPrefab;
  public GameObject platform;
  public GameObject handController;
  public Text info;
  public Leap.Unity.Interaction.InteractionManager manager;

  private List<GameObject> atoms = new List<GameObject>();
  private List<Vector3> atomsPositions = new List<Vector3>();
  private List<List<int>> allBondsFormed = new List<List<int>>();
  private List<int> numberOfBonds = new List<int>();
  private Dictionary<int, GameObject[]> bonds = new Dictionary<int, GameObject[]>();

  private static List<GameObject> moleculesSaved = new List<GameObject>();

  public static void SaveMolecule(GameObject molecule, string name)
  {
    string text = "children: " + molecule.transform.childCount +"\n";
    for(int i = 0; i < molecule.transform.childCount; i++)
    {
      Transform child = molecule.transform.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        string atomType = child.GetComponent<Atom>().GetAtomType();
        int allowedBonds = Properties.BONDS[atomType];
        List<int> bondsFormed = child.GetComponent<Atom>().GetBonds();
        int bondsMade = child.GetComponent<Atom>().GetNumberBondsMade();
        Vector3 pos = new Vector3(child.position.x-0.2f,child.position.y,child.position.z);
        text += "N atom: " + atomType + "_" + bondsMade + " _"+pos+"\n";        
        for(int j = 0; j < bondsFormed.Count; j++)
        {
          text += "    _" + bondsFormed[j] + "\n";
        }
        text += "\n";
      }
    }
    HandleTextFile.SaveFile(name+".txt",text);
  }

  public void LoadMolecule(string name)
  {
    GameObject molecule = Instantiate(moleculePrefab, platform.transform.position, platform.transform.rotation);
    molecule.GetComponent<Molecule>().SetHandController(handController);
    string text = "";
    try
    {
      text = HandleTextFile.ReadString(name);
      string[] split = text.Split('N');

      for (int j = 1; j < split.Length; j++)
      {
        string[] atom = split[j].Split('_');
        List<int> bondsFormed = new List<int>();
        for (int k = 3; k < atom.Length; k++)
        {
          int value = int.Parse(atom[k]);
          bondsFormed.Add(value);
          if (!numberOfBonds.Contains(value))
            numberOfBonds.Add(value);
        }
        allBondsFormed.Add(bondsFormed);
      }
      InitBondDictionary();
      for (int i = 1; i < split.Length; i++)
      {
        string[] atom = split[i].Split('_');
        string atomType = atom[0].Substring(7);
        int allowedBonds = Properties.BONDS[atomType];
        int bondsMade = int.Parse(atom[1]);
        Vector3 position = StringToVector3(atom[2]);

        GameObject loadedAtom = Instantiate(atomPrefab, platform.transform.position, platform.transform.rotation);
        loadedAtom.GetComponent<Atom>().handController = handController;
        loadedAtom.GetComponent<Atom>().manager = manager;
        Material mat = Resources.Load("Materials/" + atomType, typeof(Material)) as Material;
        loadedAtom.GetComponent<Atom>().SetProperties(atomType, mat, allowedBonds);
        //loadedAtom.GetComponent<Atom>().SetBonds(bondsFormed);
        loadedAtom.transform.parent = molecule.transform;
        atoms.Add(loadedAtom);
        atomsPositions.Add(position);
        AddBondDictionary(loadedAtom, i - 1);
      }
      BondAtoms(molecule);
    }
    catch (Exception e)
    {
      info.text = "File does not exist.";
      Invoke("ResetInfo",3);
    }

  }

  void ResetInfo()
  {
    info.text = "";
  }

  void BondAtoms(GameObject molecule)
  {

    if (atoms.Count == 2)
    {
      molecule.GetComponent<Molecule>().CreateBond(atoms[1], atoms[0]);
      atoms[0].transform.position = atomsPositions[0];
      atoms[1].transform.position = atomsPositions[1];
    }
    else
    { 
      for(int i = 0; i < numberOfBonds.Count; i++)
      {
        int bondId = numberOfBonds[i];
        molecule.GetComponent<Molecule>().CreateBond(bonds[bondId][0], bonds[bondId][1]);
      }
      for(int j = 0; j < atoms.Count; j++)
      {
        atoms[j].transform.position = atomsPositions[j];
      }
    }
  }

  void AddBondDictionary(GameObject atom, int ind)
  {
    List<int> atomBonds = allBondsFormed[ind];
    for(int i = 0; i < atomBonds.Count; i++)
    {
      int bondId = atomBonds[i];
      if (bonds[bondId][0] == null)
        bonds[bondId][0] = atom;
      else
        bonds[bondId][1] = atom;
    }
  }

  void InitBondDictionary()
  {
    for(int i = 0; i < numberOfBonds.Count; i++)
    {
      GameObject[] objs = new GameObject[2];
      bonds.Add(numberOfBonds[i],objs);
    }
  }

  public static Vector3 StringToVector3(string sVector)
  {
    // Remove the parentheses
    sVector = sVector.Trim();
    if (sVector.StartsWith("(") && sVector.EndsWith(")"))
    {
      sVector = sVector.Substring(1, sVector.Length - 2);
    }
    // split the items
    string[] sArray = sVector.Split(',');
    // store as a Vector3
    Vector3 result = new Vector3(
        float.Parse(sArray[0]),
        float.Parse(sArray[1]),
        float.Parse(sArray[2]));

    return result;
  }



  public static void SaveMolecule3(GameObject molecule, string name)
  {
    moleculesSaved.Add(molecule);
    molecule.SetActive(false);
  }

  void Update()
  {
    if (Input.GetKeyDown("space"))
    {
      LoadMolecule("molec.txt");
      //GameObject t = Resources.Load("Prefabs/Temp/temp1") as GameObject;
      //Instantiate(t, new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0));
      //moleculesSaved[0].SetActive(true);
    }
  }
}
