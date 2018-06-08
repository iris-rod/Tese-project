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
  public GameObject platform;
  public GameObject handController;
  public ShelfManager SManager;

  public Leap.Unity.Interaction.InteractionManager manager;
  public bool VR;
  public bool MultipleLines;
  public int rotationType;
  

  private List<GameObject> atoms = new List<GameObject> (); 
  private List<Vector3> atomsPositions = new List<Vector3> ();
  private List<List<int>> allBondsFormed = new List<List<int>> (); //bonds formed for each atom H - [1] O - [1,2] H - [2]
  private List<int> numberOfBonds = new List<int> ();
  private Dictionary<int, GameObject[]> bonds = new Dictionary<int, GameObject[]> (); //
  private Dictionary<int,int> bondsType = new Dictionary<int,int>();

  private bool canLoad = true;
  private Vector3 initialPos = new Vector3(0,0,0);
  private Vector3 moleculePosition, molPreSaved, molPostSaved;

  void Start()
  {
    molPostSaved = Vector3.zero;// on platform -> new Vector3(.8f,0f,.4f);//molecules saved in runtime
    molPreSaved = new Vector3(.5f, -0.1f, .5f); //molecules saved for testing
  }

  public void SaveMolecule (GameObject molecule, string name)
  {
    HandleTextFile.ClearFile(name + ".txt");
    string text = "M" + molecule.transform.position + "M\n";
    for (int i = 0; i < molecule.transform.childCount; i++) {
      Transform child = molecule.transform.GetChild (i);
      if (child.CompareTag ("Interactable")) {
        string atomType = child.GetComponent<Atom> ().GetAtomType ();
        int allowedBonds = Properties.BONDS [atomType];
        Dictionary<int,int> bondsFormed = child.GetComponent<Atom> ().GetBonds ();
        int bondsMade = child.GetComponent<Atom> ().GetNumberBondsMade ();
        Vector3 pos = new Vector3 (child.position.x, child.position.y, child.position.z);
        text += "N atom: " + atomType + "_" + bondsMade + " _" + pos + "\n";        
        foreach(var par in bondsFormed){
          text += "    _" + par.Key + "_" + par.Value +"\n";
        }
        text += "\n";
      }
    }
    HandleTextFile.SaveFile (name + ".txt", text);
  }

  public GameObject LoadMolecule (string name, bool mini)
  {
    string auxName = name.Split('_')[0];
    string place = name.Split('_')[1];
    bool invi = false;
    if (auxName == "Rotation" || auxName == "Move" || place == "place")
      invi = true;
    numberOfBonds = new List<int> ();
    bondsType = new Dictionary<int, int>();
    atoms = new List<GameObject> ();
    atomsPositions = new List<Vector3> ();
    allBondsFormed = new List<List<int>> ();
    
    string text = "";
    try {
      text = HandleTextFile.ReadString (name + ".txt");
    } catch (Exception e) {
      Debug.Log (e);
      text = "";
    }
    
    //molecule position
    string[] firsSplit = text.Split ('M');
    Vector3 moleculePosition = StringToVector3 (firsSplit [1]);
    GameObject molecule = Instantiate (moleculePrefab, initialPos, Quaternion.identity);
    molecule.GetComponent<Molecule> ().SetHandController (handController);
    
    //read atoms
    string[] split = firsSplit [2].Split ('N');
    for (int j = 1; j < split.Length; j++) {
      string[] atom = split [j].Split ('_');
      List<int> bondsFormed = new List<int> ();
      for (int k = 3; k < atom.Length; k+=2) {
        int bID = int.Parse (atom [k]);
        int type = int.Parse(atom[k+1]);
        bondsFormed.Add (bID);
        if (!numberOfBonds.Contains (bID)) {
          numberOfBonds.Add (bID);
          bondsType.Add(bID,type);
        }
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
      GameObject loadedAtom = Instantiate (atomPrefab, initialPos, Quaternion.identity);
      loadedAtom.GetComponent<Atom> ().handController = handController;
      loadedAtom.GetComponent<Atom> ().manager = manager;
      Material mat = Resources.Load ("Materials/" + atomType + " 2", typeof(Material)) as Material;
      if (invi)
        mat = Resources.Load ("Materials/" + atomType + " Invi", typeof(Material)) as Material;
      loadedAtom.GetComponent<Atom> ().SetProperties (atomType, mat, allowedBonds);
      loadedAtom.transform.parent = molecule.transform;
      atoms.Add (loadedAtom);
      atomsPositions.Add (position);
      AddBondDictionary (loadedAtom, i - 1);
    }
    if (mini)
      molecule.name = "Mini_" + name;
      
    BondAtoms (molecule);
    
    if (!invi)
      molecule.transform.position += GetMoleculeFinalPos (name);
    else {
      molecule.name = name;
      SetMoleculeTransparent(molecule);
    }
    
    return molecule;
  }
  
  private void SetMoleculeTransparent (GameObject molecule)
  {
    for (int i = 1; i < molecule.transform.childCount; i++) {
      Transform child = molecule.transform.GetChild(i);
      if(child.CompareTag("Bond")) {
        child.GetComponent<MeshRenderer>().material = Resources.Load("Materials/" + child.tag.Trim() + " Invi", typeof(Material)) as Material;
      }
  }
}
  private Vector3 GetMoleculeFinalPos(string fileName)
  {
    string first = fileName.Split('_')[0];
    if (first == "saved")
      return molPostSaved;
    else
      return molPreSaved;
  }

  void BondAtoms (GameObject molecule)
  {

    if (atoms.Count == 2) {
      molecule.GetComponent<Molecule> ().CreateBond (atoms [1], atoms [0], true, bondsType[numberOfBonds[0]]);
      float xOffset1 = atomsPositions[0].x - platform.transform.position.x;
      float zOffset1 = atomsPositions[0].z - platform.transform.position.z;
      float xOffset2 = atomsPositions[1].x - platform.transform.position.x;
      float zOffset2 = atomsPositions[1].z - platform.transform.position.z;
      Vector3 offSetPosition1 = new Vector3(atomsPositions[0].x, atomsPositions[0].y, atomsPositions[0].z);
      Vector3 offSetPosition2 = new Vector3(atomsPositions[1].x, atomsPositions[1].y + 0.05f, atomsPositions[1].z);

      atoms[0].transform.position = offSetPosition1; //- atomsPositions [0];
      atoms[1].transform.position = offSetPosition2; // atomsPositions [1];

    } else { 

      for (int j = 0; j < atoms.Count; j++) {
        float xOffset = atomsPositions[j].x - platform.transform.position.x;
        float zOffset = atomsPositions[j].z - platform.transform.position.z;
        Vector3 offSetPosition = new Vector3(atomsPositions[j].x, atomsPositions[j].y, atomsPositions[j].z);
        atoms [j].transform.position = offSetPosition;
      }
      for (int i = 0; i < numberOfBonds.Count; i++)
      {
        int bondId = numberOfBonds[i];
        molecule.GetComponent<Molecule>().CreateBond(bonds[bondId][0], bonds[bondId][1], true,bondsType[bondId]);
      }
    }
  }
  
  
  void AddBondDictionary (GameObject atom, int ind)
  {
    List<int> atomBonds = allBondsFormed [ind]; //os bonds do atomo
    for (int i = 0; i < atomBonds.Count; i++) {
      int bondId = atomBonds [i]; //cada bond do atomo
      if (bonds [bondId] [0] == null) //identificar o atomo como uma das ends do bond
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
    /*if (Input.GetKeyDown ("s") && platform.GetComponent<Platform> ().IsFree ()) {
      //BBManager.SetTexture ("1");
      GetComponent<TestsManager>().CheckReloadTask("partial mol");
      LoadMolecule ("partial mol", false);
    } else if (Input.GetKeyDown ("a") && platform.GetComponent<Platform> ().IsFree ()) {
      //BBManager.SetTexture ("2");
      LoadMolecule ("CO2_a", false);
      GetComponent<TestsManager>().CheckReloadTask("CO2_a");
    }
    //invisible final positions for rotations
    else if (Input.GetKeyDown ("z"))
      LoadMolecule ("Rotation_1", false);
    else if (Input.GetKeyDown ("x"))
      LoadMolecule ("Rotation_2", false);

    //invisible final positions for moves
    else if (Input.GetKeyDown("c"))
      LoadMolecule("Move_1", false);
    else if (Input.GetKeyDown("b"))
      LoadMolecule("Move_2", false);
      */

    if (Input.GetKeyDown ("s")) {
      GameObject mol = GameObject.Find("MoleculeV3(Clone)");
      SaveMolecule(mol,"bla");
    }

  }

  public void SetCanLoad(bool val)
  {
    canLoad = val;
  }




}
