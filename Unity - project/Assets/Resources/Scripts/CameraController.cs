using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

  private float speed;

	// Use this for initialization
	void Start () {
    speed = 5f;
	}
	
	// Update is called once per frame
	void Update () {
    if (Input.GetKey(KeyCode.RightArrow))
    {
      transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
    }
    if (Input.GetKey(KeyCode.LeftArrow))
    {
      transform.Rotate(new Vector3(0, -speed * Time.deltaTime, 0));
    }
    if (Input.GetKey(KeyCode.DownArrow))
    {
      transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
    }
    if (Input.GetKey(KeyCode.UpArrow))
    {
      transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
    }
  }
}
