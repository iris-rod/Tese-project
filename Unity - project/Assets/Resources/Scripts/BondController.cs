﻿using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BondController : MonoBehaviour
{
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
  private Material highlightGrasp;

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
    string split = transform.parent.name.Split('_')[0];
    distance = 0.15f;
    factor = 60;
    if (split == "Mini")
    {
      factor = 300f;
      distance = 0.05f;
    }
    detaching = false;
    scale0 = transform.localScale;
    SetDistance();
    highlightGrasp = transform.GetComponent<MeshRenderer>().materials[1];
  }


  //Set the atoms connected by this bond
  public void SetAtoms(GameObject atom1, GameObject atom2, int bType)
  {
    ballA = atom1.transform;
    ballB = atom2.transform;
    bondType = bType;
    ballA.GetComponent<Atom>().AddBond(bondType,bondId);
    ballB.GetComponent<Atom>().AddBond(bondType,bondId);
  }

  public void SetID(int value)
  {
    bondId = value;
  }

  void Update()
  {
    float dist = Vector3.Distance(ballA.position, ballB.position);
    //if it is dettaching, it only detaches after a pre-determined distance and destroys the bond object
    if (detaching)
    {
      if (dist >= distanceToDetach)
      {
        ballB.GetComponent<Atom>().RemoveBond(bondType, bondId);
        ballA.GetComponent<Atom>().RemoveBond(bondType, bondId);
        Destroy(transform.gameObject);
      }
    }
    else
    {
      MaintainDistance(dist);
    }
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

  private void SetDistance()
  {
    Vector3 pA = ballA.position;
    Vector3 pB = ballB.position;
    float dist = Vector3.Distance(pA,pB);
    if (dist != distance)
    {
      ballB.position = (ballB.transform.position - ballA.transform.position).normalized * distance + ballA.transform.position;
      transform.parent.GetComponent<Molecule>().CheckOtherBonds(ballB, bondId);
    }
  }

  //It is only dettaching if both atoms on this bond are grabbed (called by Molecule)
  public void CheckDetaching()
  {
    detaching = false;
    if (ballA.GetComponent<InteractionBehaviour>().isGrasped && ballB.GetComponent<InteractionBehaviour>().isGrasped)
    {
      ballA.GetComponent<Atom>().ToDettach();
      ballB.GetComponent<Atom>().ToDettach();
      detaching = true;
    }
  }

  public void StopDettaching()
  {
    ballA.GetComponent<Atom>().StopDettach();
    ballB.GetComponent<Atom>().StopDettach();
    detaching = false;
  }

  //Keep atoms together
  void MaintainDistance(float currentDist)
  {
    if (currentDist != distance)
    {
      if (ballA.GetComponent<InteractionBehaviour>().isGrasped)
      {
        ballB.position = (ballB.transform.position - ballA.transform.position).normalized * distance + ballA.transform.position;
        transform.parent.GetComponent<Molecule>().CheckOtherBonds(ballB, bondId);
      }
      else if (ballB.GetComponent<InteractionBehaviour>().isGrasped)
      {
        ballA.position = (ballA.transform.position - ballB.transform.position).normalized * distance + ballB.transform.position;
        transform.parent.GetComponent<Molecule>().CheckOtherBonds(ballA, bondId);
      }
      else
      {
        ballB.position = (ballB.transform.position - ballA.transform.position).normalized * distance + ballA.transform.position;
        transform.parent.GetComponent<Molecule>().CheckOtherBonds(ballB, bondId);
      }
    }
  }

  public int getBondID()
  {
    return bondId;
  }

  public void CompareAtom(Transform atom)
  {
    float dist = Vector3.Distance(ballA.position, ballB.position);
    if (atom == ballA)
    {
      if (dist != distance)
      {
        ballB.position = (ballB.transform.position - ballA.transform.position).normalized * distance + ballA.transform.position;

      }
    }
    else if (atom == ballB)
    {
      if (dist != distance)
      {
        ballA.position = (ballA.transform.position - ballB.transform.position).normalized * distance + ballB.transform.position;

      }

    }
  }

  public bool CheckAtomsBond(GameObject atomA, GameObject atomB)
  {
    return (((atomA == ballA) && (atomB == ballB)) || (atomA == ballB) && (atomB == ballA));
  }

  public Transform[] GetAtoms()
  {
    Transform[] atoms = new Transform[2] { ballA,ballB};
    return atoms;
  }

  public void Highlight(bool highlight)
  {
    if (highlight)
      highlightGrasp.SetFloat("_Outline", 0.001f);
    else
      highlightGrasp.SetFloat("_Outline", 0.0f);
  }

}
