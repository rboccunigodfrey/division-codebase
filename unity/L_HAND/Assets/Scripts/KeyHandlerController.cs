using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyHandlerController : MonoBehaviour
{
    // Update is called once per frame
    public SolenoidController solenoid1;

    void Update()
    {
        SolenoidController[] solenoids = {solenoid1};
        foreach (SolenoidController solenoid in solenoids) 
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                if (Input.GetKey(KeyCode.DownArrow)) 
                {
                    solenoid.SetTiltAngleX(0f);
                } else 
                {
                    solenoid.SetTiltAngleX(10f);
                }
            } else if (Input.GetKey(KeyCode.DownArrow)) 
            {
                if (Input.GetKey(KeyCode.UpArrow)) 
                {
                    solenoid.SetTiltAngleX(0f);
                } else 
                {
                    solenoid.SetTiltAngleX(-10f);
                }
            } else
            {
                solenoid.SetTiltAngleX(0f);
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                if (Input.GetKey(KeyCode.RightArrow)) 
                {
                    solenoid.SetTiltAngleZ(0f);
                } else 
                {
                    solenoid.SetTiltAngleZ(-10f);
                }
            } else if (Input.GetKey(KeyCode.RightArrow)) 
            {
                if (Input.GetKey(KeyCode.LeftArrow)) 
                {
                    solenoid.SetTiltAngleZ(0f);
                } else 
                {
                    solenoid.SetTiltAngleZ(10f);
                }
            } else
            {
                solenoid.SetTiltAngleZ(0f);
            }

            solenoid.isActive = Input.GetKey(KeyCode.Space);
        }
    }
}
