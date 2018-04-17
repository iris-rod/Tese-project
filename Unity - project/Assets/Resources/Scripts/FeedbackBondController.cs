using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackBondController : MonoBehaviour {

  Transform ballA; // drag sphereA here
  Transform ballB; // drag sphereB here
  Vector3 scale0;// initial localScale

  private bool detaching;
  private Vector3 positionA;
  private Vector3 positionB;
  private int bondId;
  private float factor;
  private float doubleBond = 0.02f;
  private float tripleBond = 0.03f;

  private int bondType;
  public float distanceToDetach;
  public float distance; //At which they stick together

  void Start()
  {
    switch (bondType)
    {
      case 1:
        distanceToDetach = 0.3f;
        break;
      case 2:
        distanceToDetach = 0.35f;
        break;
      case 3:
        distanceToDetach = 0.45f;
        break;
      case 4:
        distanceToDetach = 0.55f;
        break;

    }
    distance = 0.15f;
    factor = 60;
    detaching = false;
    scale0 = transform.localScale;
  }

  //Set the atoms connected by this bond
  public void SetAtoms(GameObject atom1, GameObject atom2, int bType)
  {
    ballA = atom1.transform;
    ballB = atom2.transform;
    bondType = bType;
  }

  void Update()
  {
    //if it is dettaching, it only detaches after a pre-determined distance and destroys the bond object
    Vector3 pA = ballA.position;
    Vector3 pB = ballB.position;

    transform.position = (pA + pB) / 2; // place the cube in the middle of A-B
    transform.LookAt(pB); // make it look to ballB position
    // adjust cube length so it will have its ends at the sphere centers
    Vector3 scale = scale0;
    scale.z = scale0.z * Vector3.Distance(pA, pB) * factor;
    // stretch it in the direction it's looking
    transform.localScale = scale;
  }

  public void DestroyBond(GameObject atomA, GameObject atomB)
  {

    if ((atomA.transform == ballA && atomB.transform == ballB) || (atomB.transform == ballA && atomB.transform == ballB))
    {
      Destroy(transform.gameObject);
    }
  }
}
