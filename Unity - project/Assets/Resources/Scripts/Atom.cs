using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atom : MonoBehaviour {

  public GameObject molecule;
  public GameObject bond;
  public GameObject handController;
  public Leap.Unity.Interaction.InteractionManager manager;
  //public float distance; //At which they stick together

  private string atomType = "Hydrogen";
  private bool grabbed;
  private int numberOfBondsAllowed = 0;
  private bool isAttached;
  private bool toDetach;
  private int numberOfBonds; //bonds that the atom has
  private GameObject nearAtom;
  private Material highlightMat;
  private Material highlightGrasp;
  private bool leftPinch;
  private bool rightPinch;
  private bool canBond;
  private float radiusCollision;
  private Dictionary<int,int> bondIDType; //List<int>

  private Vector3 pointToRotate;
  private bool isRotating;
  private float distanceToPivot;
  private Vector3 pivotPosition;
  private bool toBond;
  private int typeOfBonding;

  //variables to lerp positions
  private Vector3 initialPos, endPos;
  private bool start;
  private float length, startTime;

  // Use this for initialization
  void Start ()
  {
    typeOfBonding = 0;
    distanceToPivot = 0;
    isRotating = false;
    isAttached = false;
    toDetach = false;
    toBond = false;
    start = true;
    highlightMat = transform.GetComponent<MeshRenderer>().materials[1];
    highlightGrasp = transform.GetComponent<MeshRenderer>().materials[2];
    handController = GameObject.FindGameObjectWithTag("HandController");
  }


  public void SetProperties(string type, Material mat, int bonds)
  {
    numberOfBonds = 0;
    numberOfBondsAllowed = bonds;
    bondIDType = new Dictionary<int,int>();
    atomType = type;
    transform.GetComponent<MeshRenderer>().material = mat;
    float size = Properties.DESKTOP_SIZES[atomType];
    transform.localScale = new Vector3(size,size,size);
    if(type == "Hydrogen") radiusCollision = 0.026f;
    else if(type == "Carbon") radiusCollision = 0.055f;
    else if(type == "Oxygen") radiusCollision = 0.048f;
  }

  // Update is called once per frame
  void Update ()
  {
    if (numberOfBonds >= 1 || toBond)
      Attach();
    else Dettach();

    if (transform.parent == null || !(transform.parent.name.Split('_')[0] == "Mini"))
    {
      if (GetComponent<InteractionBehaviour>().isGrasped)
      {
        highlightGrasp.SetFloat("_Outline", 0.015f);
      }
      else
      {
        highlightGrasp.SetFloat("_Outline", 0.00f);
        if (toBond) toBond = false;
      }

      if (isRotating)
        Rotate();
        
      //keep away from other objects
      CheckCollisions();
    }
  }
  
  private void CheckCollisions ()
  {    
  
    Collider[] colliders = Physics.OverlapSphere (transform.position, radiusCollision);
    if (colliders.Length > 1) {
      for (int i = 0; i < colliders.Length; i++) {
        Collider col = colliders [i];
        if (col.transform.CompareTag ("Interactable") && !transform.GetComponent<InteractionBehaviour> ().isGrasped) {
          float distance = Vector3.Distance (transform.position, col.transform.position);
          if (distance < radiusCollision) {
            Vector3 newPos = (transform.position - col.transform.position).normalized * .08f + col.transform.position;
            Vector3 otherNewPos =(col.transform.position - transform.position).normalized * .08f + transform.position;
            transform.position = Vector3.Lerp(transform.position, newPos, Time.time );
            col.transform.position = Vector3.Lerp(col.transform.position, otherNewPos, Time.time );
          }

        }       
      }
    }
  }

  public void SetDistanceToPivot(Vector3 pivotPos)
  {
    pivotPosition = pivotPos;
    distanceToPivot = Vector3.Distance(pivotPosition,transform.position);
  }

  public void EnableBond()
  {
    canBond = true;
  }

  public void DisableBond()
  {
    canBond = false;
  }

  public void SetRotation(Vector3 point, Vector3 axis, float angle)
  {
    transform.RotateAround(point, axis, angle);    
  }

  void Rotate()
  {
    transform.position = (transform.position - pivotPosition).normalized * distanceToPivot + pivotPosition;
  }

  public void SetRotating(bool rotate)
  {
    isRotating = rotate;
  }

  void Dettach()
  {
    transform.parent = null;
    isAttached = false;
  }

  public void Attach()
  {
    isAttached = true;
  }

  public void ToDettach()
  {
    toDetach = true;    
  }

  public void StopDettach()
  {
    toDetach = false;
  }

  public bool CanBond()
  {
    return canBond;
  }

  public string GetAtomType()
  {
    return atomType;
  }

  public void Highlight(bool highlight)
  {
    if(highlight)
      highlightGrasp.SetFloat("_Outline", 0.01f);//.001
    else
      highlightGrasp.SetFloat("_Outline", 0.0f);
  }

  public Dictionary<int,int> GetBonds()
  {
    return bondIDType;
  }

  public void SetBonds(Dictionary<int,int> bonds)
  {
  
    foreach(var par in bonds){
      bondIDType.Add(par.Key, par.Value);
    }
  }

  public int GetNumberBondsMade()
  {
    return numberOfBonds;
  }
  
  public int GetMaxNumberOfBonds ()
  {
    return numberOfBondsAllowed;
  }

  public bool HasFreeBonds()
  {
    return numberOfBonds < numberOfBondsAllowed;
  }

  public void SettingBond(int bondType)
  {
    typeOfBonding = bondType;
    toBond = true;
  }

  public bool IsBonding()
  {
    return toBond;
  }

  void OnCollisionEnter (Collision col)
  {
    if (col.transform.CompareTag ("Interactable") && toBond && col.transform.GetComponent<Atom> ().toBond && typeOfBonding == 2) {
      transform.parent.GetComponent<Molecule> ().UpdateBondType (1);
    } else if (col.transform.CompareTag ("Interactable") && canBond && col.transform.GetComponent<Atom> ().CanBond ()) {
      if (HasFreeBonds () && col.transform.GetComponent<Atom> ().HasFreeBonds ())
        StickToMolecule (col.gameObject);
      else
        highlightMat.SetFloat ("_Outline", 0.02f);
    }
  }

  void OnCollisionExit(Collision col)
  {
      highlightMat.SetFloat("_Outline", 0.0f);
  }

  void StickToMolecule (GameObject obj)
  {
      if (obj.transform.parent == null && transform.parent == null) {
        Vector3 platePos = new Vector3 (transform.position.x, transform.position.y, transform.position.z);
        GameObject mole = Instantiate (molecule, platePos, transform.rotation);
        mole.GetComponent<Molecule> ().SetHandController (handController);
        mole.GetComponent<Molecule> ().CreateBond (obj, transform.gameObject, false, 0);
      } else if (obj.transform.parent != transform.parent) {
        if (transform.parent == null) {
          transform.parent = obj.transform.parent;
        } else if (obj.transform.parent == null) {
          obj.transform.parent = transform.parent;
        }
        transform.parent.GetComponent<Molecule> ().CreateBond (obj, transform.gameObject, false, 0);
      }
  }

  public void AddBond(int type, int bondId)
  {
    bondIDType.Add(bondId, type);
    numberOfBonds += type;
  }

  public void RemoveBond(int type, int bondId)
  {
    bondIDType.Remove(bondId);
    numberOfBonds -= type;
    toDetach = false;
  }

  public int GetAvailableBonds()
  {
    return numberOfBondsAllowed - numberOfBonds;
  }
}
