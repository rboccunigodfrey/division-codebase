using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FretHandController : MonoBehaviour
{
    public SolenoidController solenoid1;
    public SolenoidController solenoid2;
    public SolenoidController solenoid3;
    public SolenoidController solenoid4;
    public SolenoidController solenoid5;
    public SolenoidController solenoid6;

    public float travelSpeed = 40.0f;
    public float trackDistance = 5.0f;

    public float solenoidReverseRowOffset = 1.75f/2;
    public float solenoidTiltSpeed = 100.0f; // The speed at which the core moves
    public float solenoidCoreSpeed = 100.0f;
    public float solenoidTrackLength = 5.0f;
    public float solenoidMaxTiltX = 10f;
    public float solenoidMaxTiltZ = 10f;
    public float solenoidXSpacing = 1.75f;
    public float solenoidZSpacing = 1.75f;

    [SerializeField] private float trackLength = 20.0f;
    [SerializeField] private float rawTrackDistance;
    [SerializeField] private bool isStationary;
    [SerializeField] private bool allSolenoidsStationary;
    [SerializeField] private bool allSolenoidsRetracted;

    
    // Start is called before the first frame update
    void Start()
    {
        SolenoidController[] solenoids = {solenoid1, solenoid2, solenoid3, solenoid4, solenoid5, solenoid6};

        // setup solenoid matrix

        int rowLength = 3; // number of objects in each row
        Vector3 startPosition = transform.position; // starting position for placing objects
        int index = 0; // current index in objectsToPlace array

        // iterate through each row
        for (int row = 0; row < solenoids.Length / rowLength; row++)
        {
            // iterate through each column in the row
            for (int col = 0; col < rowLength; col++)
            {
                // calculate the position for the current object
                Vector3 position = new Vector3(
                    startPosition.x + (col * solenoidXSpacing) + (solenoidReverseRowOffset)*row,
                    startPosition.y,
                    startPosition.z
                );

                // place the current object at the calculated position
                solenoids[index].transform.position = position;

                // increment the index for the next object in the array
                index++;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        SolenoidController[] solenoids = {solenoid1, solenoid2, solenoid3, solenoid4, solenoid5, solenoid6};


        allSolenoidsRetracted = true;
        allSolenoidsStationary = true;
        foreach (SolenoidController solenoid in solenoids) 
        {
            solenoid.SetTiltSpeed(solenoidTiltSpeed);
            solenoid.SetMaxTiltX(solenoidMaxTiltX);
            solenoid.SetMaxTiltZ(solenoidMaxTiltZ);
            solenoid.SetCoreSpeed(solenoidCoreSpeed);
            solenoid.SetTrackLength(solenoidTrackLength);
            solenoid.SetSolenoidZSpacing(solenoidZSpacing);
            if (solenoid.isActive) 
            {
                allSolenoidsRetracted = false;
            }
            if (!solenoid.IsStationary()) 
            {
                allSolenoidsStationary = false;
            }
        }

        trackDistance = Mathf.Round(Mathf.Clamp(trackDistance, 0, trackLength) * 1000f) * 0.001f;
        if (allSolenoidsRetracted)
        {
            float newZ = Mathf.Lerp(transform.localPosition.z, trackDistance, Time.deltaTime * travelSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);
        }

        rawTrackDistance = Mathf.Round(transform.localPosition.z * 1000f) * 0.001f;
        isStationary = Mathf.Abs(Mathf.Abs(trackDistance) - Mathf.Abs(rawTrackDistance)) < 0.05f;
        if (!allSolenoidsRetracted) {isStationary = true;}
    }

    public void SetTravelSpeed(float speed)
    {

        travelSpeed = speed;
    }
    public void SetTrackDistance(float distance)
    {
        trackDistance = distance;
    }
    public float GetTrackLength()
    {
        return trackLength;
    }
    public bool IsStationary()
    {
        return isStationary;
    }

    public void SetSolenoidTiltSpeed(float speed)
    {
        solenoidTiltSpeed = speed;
    }
    public void SetSolenoidMaxTiltX(float angle)
    {
        solenoidMaxTiltX = angle;
    }
    public void SetSolenoidMaxTiltZ(float angle)
    {
        solenoidMaxTiltZ = angle;
    }
    public void SetSolenoidCoreSpeed(float speed)
    {
        solenoidCoreSpeed = speed;
    }

    public void SetSolenoidTrackLength(float length)
    {
        solenoidTrackLength = length;
    }
}
