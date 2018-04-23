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

  public GameObject axisEnd;
  public bool Axis;


  // Use this for initialization
  void Start ()
  {
    //positionEndX = new Vector3(-0.3f,2,0.2f);
    //positionEndX = new Vector3(-0.3f, 2.5f, 0.2f);
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
      if (Axis)
      {
        if (!grabbed) SetEndAxis();
        CheckAxisEnd();
      }
    }
    else
    {
      highlightGrasp.SetFloat("_Outline", 0);
      if(Axis)
        RemoveAxis();
      grabbed = false;
    }
    //transform.rotation = rotation;
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

  void SetEndAxis()
  {
    positionEndZ = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
    positionEndX = new Vector3(transform.position.x - 0.2f, transform.position.y, transform.position.z);

    endZ = Instantiate(axisEnd,positionEndZ,transform.rotation);
    endZ.transform.parent = transform.parent;
    endZ.GetComponent<InteractionBehaviour>().manager = transform.GetComponent<InteractionBehaviour>().manager;
    HMZ = endZ.GetComponent<MeshRenderer>().materials[1];

    endX = Instantiate(axisEnd, positionEndX, transform.rotation);
    endX.transform.parent = transform.parent;
    endX.GetComponent<InteractionBehaviour>().manager = transform.GetComponent<InteractionBehaviour>().manager;
    HMX = endX.GetComponent<MeshRenderer>().materials[1];
    grabbed = true;
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
      highlightGrasp.SetFloat("_Outline", 0.001f);
    else
      highlightGrasp.SetFloat("_Outline", 0.0f);
  }

}
