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
  private Material highlightMat;
  private Material highlightGrasp;
  private bool canBond;
  private float radiusCollision;
  private Dictionary<int,int> bondIDType; //List<int>

  private bool isRotating;
  private float distanceToPivot, distanceToGrabbed;
  private Vector3 molCenter, grabbedPos;
  private bool toBond;
  private int typeOfBonding;

  private Vector3[] offsets;
  private float[] dists;
  private GameObject grabbedAtom;

  //tutorial
  private float totalDist;

  // Use this for initialization
  void Start ()
  {
    typeOfBonding = 0;
    distanceToPivot = 0;
    isRotating = false;
    isAttached = false;
    toDetach = false;
    toBond = false;
    grabbed = false;
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
    float size = Properties.VR_SIZES[atomType];
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
      if (GetComponent<InteractionBehaviour>().isGrasped && !grabbed)
      {
        highlightGrasp.SetFloat("_Outline", 0.015f);
        grabbed = true;
        SoundEffectsManager.PlaySound("atomGrabbed");
      }
      else if(!GetComponent<InteractionBehaviour>().isGrasped)
      {
        highlightGrasp.SetFloat("_Outline", 0.00f);
        grabbed = false;
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

  public void SetDistanceToCenter(Vector3 center)
  {
    molCenter = center;
    distanceToPivot = Vector3.Distance(center,transform.position);
  }

  public void SetDistanceToGrabbed(Vector3 pos)
  {
    grabbedPos = pos;
    distanceToGrabbed = Vector3.Distance(pos, transform.position);
  }

  public void SetDistanceToOtherAtoms(GameObject obj)
  {
    grabbedAtom = obj;
    int number = 0;
    for (int i = 0; i < transform.parent.childCount; i++)
    {
      Transform child = transform.parent.GetChild(i);
      if (child.CompareTag("Interactable"))
      {
        number++;
      }
    }
    offsets = new Vector3[number-2];
    dists = new float[number-2];
    int ind = 0;
    for (int i = 0; i < transform.parent.childCount; i++)
    {
      Transform child = transform.parent.GetChild(i);
      if (child.CompareTag("Interactable") && child != transform && child != obj.transform)
      {
        offsets[ind] = transform.position - child.position;
        dists[ind] = Vector3.Distance(transform.position,child.position);
        ind++;
      }
    }
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
    if(totalDist >= 60)
      TutorialManager.SetMoleculeRotated(true);
    grabbedPos = transform.parent.GetComponent<Molecule>().GetGrabbedAtom().position;
    float dist = Vector3.Distance(grabbedPos, transform.position);
    if(dist != distanceToGrabbed && !transform.GetComponent<InteractionBehaviour>().isGrasped)
    {
      transform.position = (transform.position - grabbedPos).normalized * distanceToGrabbed + grabbedPos;
    }
    float dist1 = Vector3.Distance(molCenter, transform.position);
    if (dist1 != distanceToPivot)
    {
      transform.position = (transform.position - molCenter).normalized * distanceToPivot + molCenter;
    }

    //check the distances to other atoms if it isnt the grabbed atom
    if (!transform.GetComponent<InteractionBehaviour>().isGrasped)
    {
      int ind = 0;
      for (int i = 0; i < transform.parent.childCount; i++)
      {
        Transform child = transform.parent.GetChild(i);
        float distAtoms = Vector3.Distance(child.position, transform.position);
        if (child.CompareTag("Interactable") && child != transform && child != grabbedAtom.transform)
        {
          if(dists[ind] != Vector3.Distance(transform.position, child.position))
          {
            child.position = (child.position - transform.position).normalized * dists[ind] + transform.position;
          }
          ind++;
        }
      }
    }
    totalDist += dist;
  }

  public void SetRotating(bool rotate)
  {
    isRotating = rotate;
  }

  public void Dettach()
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

  public void SettingBond()
  {
    toBond = true;
  }

  public bool IsBonding()
  {
    return toBond;
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
    TutorialManager.SetBondBroken(true);
  }

  public int GetAvailableBonds()
  {
    return numberOfBondsAllowed - numberOfBonds;
  }

  void OnCollisionEnter (Collision col)
  {
    if (col.transform.CompareTag ("Interactable") && toBond && col.transform.GetComponent<Atom> ().toBond) {
      transform.parent.GetComponent<Molecule> ().UpdateBondType (1);
    } else if (col.transform.CompareTag ("Interactable") && canBond && col.transform.GetComponent<Atom> ().CanBond ()) {
      if (HasFreeBonds () && col.transform.GetComponent<Atom> ().HasFreeBonds ())
        StickToMolecule (col.gameObject);
      else
        highlightMat.SetFloat ("_Outline", 0.02f);

      TutorialManager.SetAtomsTouched(true);
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


}
