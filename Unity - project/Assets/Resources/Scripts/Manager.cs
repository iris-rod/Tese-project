using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

  public GameObject moleculePrefab;
  public GameObject atomPrefab;
  public GameObject savedMoleculePrefab;
  public GameObject platform;
  public GameObject handController;
  public Text info;
  public Leap.Unity.Interaction.InteractionManager manager;
  public bool touchOtherToSwitch;

  private List<GameObject> atoms = new List<GameObject> ();
  private List<Vector3> atomsPositions = new List<Vector3> ();
  private List<List<int>> allBondsFormed = new List<List<int>> ();
  private List<int> numberOfBonds = new List<int> ();
  private Dictionary<int, GameObject[]> bonds = new Dictionary<int, GameObject[]> ();

  private int maxNumberChild = 5;
  private bool canLoad = true;

  public void SaveMolecule (GameObject molecule, string name)
  {
    HandleTextFile.ClearFile(name + ".txt");
    string text = "children: " + molecule.transform.childCount + "\n";
    for (int i = 0; i < molecule.transform.childCount; i++) {
      Transform child = molecule.transform.GetChild (i);
      if (child.CompareTag ("Interactable")) {
        string atomType = child.GetComponent<Atom> ().GetAtomType ();
        int allowedBonds = Properties.BONDS [atomType];
        List<int> bondsFormed = child.GetComponent<Atom> ().GetBonds ();
        int bondsMade = child.GetComponent<Atom> ().GetNumberBondsMade ();
        Vector3 pos = new Vector3 (child.position.x - 0.2f, child.position.y, child.position.z);
        text += "N atom: " + atomType + "_" + bondsMade + " _" + pos + "\n";        
        for (int j = 0; j < bondsFormed.Count; j++) {
          text += "    _" + bondsFormed [j] + "\n";
        }
        text += "\n";
      }
    }
    HandleTextFile.SaveFile (name + ".txt", text);
    SetMoleculeOnChild (name + ".txt");
  }

  public void LoadMolecule (string name)
  {
    Debug.Log("enter load");
    numberOfBonds = new List<int> ();
    atoms = new List<GameObject> ();
    atomsPositions = new List<Vector3> ();
    allBondsFormed = new List<List<int>> ();
    GameObject molecule = Instantiate (moleculePrefab, platform.transform.position, platform.transform.rotation);
    molecule.GetComponent<Molecule> ().SetHandController (handController);
    string text = "";
    try {
      text = HandleTextFile.ReadString (name);
    } 
    catch (Exception e) {
      Debug.Log(name);
      info.text = "File does not exist.";
      text = "";
      Invoke ("ResetInfo", 3);
    }
      string[] split = text.Split ('N');

      for (int j = 1; j < split.Length; j++) {
        string[] atom = split [j].Split ('_');
        List<int> bondsFormed = new List<int> ();
        for (int k = 3; k < atom.Length; k++) {
          int value = int.Parse (atom [k]);
          bondsFormed.Add (value);
          if (!numberOfBonds.Contains (value))
            numberOfBonds.Add (value);
        }
        allBondsFormed.Add (bondsFormed);
      }
      InitBondDictionary ();
      for (int i = 1; i < split.Length; i++) {
        string[] atom = split [i].Split ('_');
        string atomType = atom [0].Substring (7);
        int allowedBonds = Properties.BONDS [atomType];
        int bondsMade = int.Parse (atom [1]);
        Vector3 position = StringToVector3 (atom [2]);

        GameObject loadedAtom = Instantiate (atomPrefab, platform.transform.position, platform.transform.rotation);
        loadedAtom.GetComponent<Atom> ().handController = handController;
        loadedAtom.GetComponent<Atom> ().manager = manager;
        Material mat = Resources.Load ("Materials/" + atomType, typeof(Material)) as Material;
        loadedAtom.GetComponent<Atom> ().SetProperties (atomType, mat, allowedBonds);
        loadedAtom.transform.parent = molecule.transform;
        atoms.Add (loadedAtom);
        atomsPositions.Add (position);
        AddBondDictionary (loadedAtom, i - 1);
      }
      BondAtoms (molecule);

  }

  void SetMoleculeOnChild (string fileName)
  {
    if (transform.childCount < maxNumberChild) {
      Vector3 pos = new Vector3 (transform.position.x + (transform.childCount * 0.2f + 0.1f), transform.position.y, transform.position.z);
      GameObject saved = Instantiate (savedMoleculePrefab, pos, transform.rotation);
      saved.transform.parent = transform;
      saved.GetComponent<SavedMolecule> ().SetFileName (fileName);      
    }
  }

  void ResetInfo ()
  {
    info.text = "";
  }

  void BondAtoms (GameObject molecule)
  {

    if (atoms.Count == 2) {
      molecule.GetComponent<Molecule> ().CreateBond (atoms [1], atoms [0]);
      atoms [0].transform.position = atomsPositions [0];
      atoms [1].transform.position = atomsPositions [1];
    } else { 
      for (int i = 0; i < numberOfBonds.Count; i++) {
        int bondId = numberOfBonds [i];
        molecule.GetComponent<Molecule> ().CreateBond (bonds [bondId] [0], bonds [bondId] [1]);
      }
      for (int j = 0; j < atoms.Count; j++) {
        atoms [j].transform.position = atomsPositions [j];
      }
    }
    Vector3 posMol = new Vector3(platform.transform.position.x,platform.transform.position.y+.2f,platform.transform.position.z);
    molecule.transform.position = posMol;
  }
  

  void AddBondDictionary (GameObject atom, int ind)
  {
    List<int> atomBonds = allBondsFormed [ind];
    for (int i = 0; i < atomBonds.Count; i++) {
      int bondId = atomBonds [i];
      if (bonds [bondId] [0] == null)
        bonds [bondId] [0] = atom;
      else
        bonds [bondId] [1] = atom;
    }
  }

  void InitBondDictionary ()
  {
    bonds = new Dictionary<int, GameObject[]> ();
    for (int i = 0; i < numberOfBonds.Count; i++) {
      GameObject[] objs = new GameObject[2];
      bonds.Add (numberOfBonds [i], objs);
    }
  }

  public static Vector3 StringToVector3 (string sVector)
  {
    // Remove the parentheses
    sVector = sVector.Trim ();
    if (sVector.StartsWith ("(") && sVector.EndsWith (")")) {
      sVector = sVector.Substring (1, sVector.Length - 2);
    }
    // split the items
    string[] sArray = sVector.Split (',');
    // store as a Vector3
    Vector3 result = new Vector3 (
                       float.Parse (sArray [0]),
                       float.Parse (sArray [1]),
                       float.Parse (sArray [2]));

    return result;
  }

  void Update ()
  {
    if (Input.GetKeyDown ("space")) {
      LoadMolecule ("molec.txt");
    }
    if(touchOtherToSwitch)
      CheckLoadDoubleTouch();
    else CheckLoadOneTouch();
  }

  void CheckLoadDoubleTouch ()
  {
    bool check = true;
    int selectedID = -1;
    for (int i = 0; i < transform.childCount; i++) {
      Transform child = transform.GetChild (i);
      if (!check)
        child.GetComponent<SavedMolecule> ().SetOneSelected (true);
      if (child.GetComponent<SavedMolecule> ().IsSelected ()) {
        check = false;        
        selectedID = i;
      }
    }
    canLoad = check;
    if (check) {
      for (int i = 0; i < transform.childCount; i++) {
        Transform child = transform.GetChild (i);
        child.GetComponent<SavedMolecule> ().SetOneSelected (false);
      }
    } else {
      for (int i = 0; i < transform.childCount; i++) {
        if (i != selectedID) {
          Transform child = transform.GetChild (i);
          child.GetComponent<SavedMolecule> ().SetOneSelected (true);
        }
      }
    }
  }
  
  void CheckLoadOneTouch ()
  {
    for (int i = 0; i < transform.childCount; i++) {
      Transform child = transform.GetChild(i);
      
    }
  }
  
  public void CheckSelectedItems (GameObject newSelected)
  {
    for (int i = 0; i < transform.childCount; i++) {
      Transform child = transform.GetChild(i);
      if(!GameObject.ReferenceEquals( child, newSelected) && child.GetComponent<SavedMolecule> ().IsSelected ()){
        child.GetComponent<SavedMolecule> ().SetSelected (false);
      }
    }
  }
}
