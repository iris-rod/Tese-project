using Leap.Unity.Interaction;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PivotController : MonoBehaviour {

  private Material highlightGrasp;
  private float lastAngle;
  private float currentAngle;
  private Vector3 axis;
  private bool grabbed;
  private GameObject endZ, endX;
  private Material HMZ, HMX;
  private Vector3 positionEndZ, positionEndX;
  private bool endZGrabbed, endXGrabbed;
  private Quaternion rotation;
  
  public bool Axis;

  private int graspedByLeft;


  // Use this for initialization
  void Start ()
  {
    endZGrabbed = false;
    endXGrabbed = false;
    grabbed = false;
    rotation = transform.rotation;
    axis = Vector3.zero;
    highlightGrasp = transform.GetComponent<MeshRenderer>().materials[1];
    transform.rotation.ToAngleAxis(out lastAngle, out axis); 
  }
	
	// Update is called once per frame
	void Update () {
    //transform.rotation = rotation;
    //transform.Rotate(Vector3.up,50*Time.deltaTime);
    if (GetComponent<InteractionBehaviour>().isGrasped)
    {
      highlightGrasp.SetFloat("_Outline", 0.005f);
      transform.rotation.ToAngleAxis(out currentAngle, out axis);
    }
    else
    {
      highlightGrasp.SetFloat("_Outline", 0);
    }
    //transform.rotation = rotation;
  }

  public int IsGrasped()
  {
    return graspedByLeft;
  }

  public void SetGrasped(int val)
  {
    graspedByLeft = val;
  }

  void CheckAxisEnd()
  {
    endZ.transform.position = positionEndZ;
    endX.transform.position = positionEndX;

    if (endZ.GetComponent<InteractionBehaviour>().isGrasped)
    {
      HMZ.SetFloat("_Outline", 0.005f);
      endZGrabbed = true;
    }
    else
    {
      HMZ.SetFloat("_Outline", 0f);
      endZGrabbed = false;
    }
    if (endX.GetComponent<InteractionBehaviour>().isGrasped)
    {
      HMX.SetFloat("_Outline", 0.005f);
      endXGrabbed = true;
    }
    else
    {
      HMX.SetFloat("_Outline", 0f);
      endXGrabbed = false;
    }
  }

  public bool IsEndZGrabbed()
  {
    return endZGrabbed;
  }

  public bool IsEndXGrabbed()
  {
    return endXGrabbed;
  }

  void RemoveAxis()
  {
    Destroy(endZ);
    Destroy(endX);
  }

  public float GetAngle()
  {
    float sendAngle = currentAngle - lastAngle;
    lastAngle = currentAngle;
    return sendAngle;
  }

  public Vector3 GetAxis()
  {
    return axis;
  }

  public void Highlight(bool highlight)
  {
    if (highlight)
      highlightGrasp.SetFloat("_Outline", 0.01f);
    else
      highlightGrasp.SetFloat("_Outline", 0.0f);
  }

}
