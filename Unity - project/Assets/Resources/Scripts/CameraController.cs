using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

  private float speed;

  void Start()
  {
    speed = 50f;
  }

  // Update is called once per frame
  void Update()
  {
    CheckKeyboardInput();
  }

  void CheckKeyboardInput()
  {
    int dir = -1;
    if (Input.GetKey(KeyCode.LeftArrow))
    {
      dir = 0;
    }
    if (Input.GetKey(KeyCode.RightArrow))
    {
      dir = 1;
    }

    Rotate(dir);
  }

  void Rotate(int dir)
  {
    if (dir != -1)
    {
      Vector3 rotateValue = new Vector3(0, speed * Time.deltaTime, 0);
      if (dir == 1) transform.eulerAngles += rotateValue;
      else transform.eulerAngles -= rotateValue;
    }
  }
}
