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

    public float travelSpeed = 20.0f;
    public float trackDistance = 5.0f;
    public int trackPosition;

    public float solenoidReverseRowOffset = 1.75f/2;
    public float solenoidTiltSpeed = 20.0f; // The speed at which the core moves
    public float solenoidCoreSpeed = 20.0f;
    public float solenoidTrackLength = 5.0f;
    public float solenoidTrackReverseLength = 5.0f;
    public float solenoidMaxTiltX = 10f;
    public float solenoidMaxTiltZ = 10f;
    public float solenoidXSpacing = 1.75f;
    public float solenoidZSpacing = 1.75f;

    [SerializeField] private float trackLength = 20.0f;
    [SerializeField] private float rawTrackDistance;
    [SerializeField] private bool isStationary;
    [SerializeField] private bool allSolenoidsStationary;
    [SerializeField] private bool allSolenoidsRetracted;
    [SerializeField] private float[,] currentPositionMatrix;

    private float[] fretLengths = new float[23] 
    {
        0f,
        37.941f/486.304f,
        73.752f/486.304f,
        107.554f/486.304f,
        139.458f/486.304f,
        169.572f/486.304f,
        197.996f/486.304f,
        224.824f/486.304f,
        250.147f/486.304f,
        274.048f/486.304f,
        296.608f/486.304f,
        317.901f/486.304f,
        338.000f/486.304f,
        356.970f/486.304f,
        374.876f/486.304f,
        391.777f/486.304f,
        407.729f/486.304f,
        422.786f/486.304f,
        436.998f/486.304f,
        450.412f/486.304f,
        463.073f/486.304f,
        475.024f/486.304f,
        486.304f/486.304f
    };

    
    // Start is called before the first frame update
    void Start()
    {
        SolenoidController[] solenoids = {solenoid1, solenoid2, solenoid3, solenoid4, solenoid5, solenoid6};

        // setup solenoid matrix

        Vector3 startPosition = transform.position; // starting position for placing objects
        int index = 0; // current index in objectsToPlace array

        // iterate through each row
        for (int row = 0; row < solenoids.Length / 3; row++)
        {
            // iterate through each column in the row
            for (int col = 0; col < 3; col++)
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
        // Update current position matrix

        SolenoidController[] solenoids = {solenoid1, solenoid2, solenoid3, solenoid4, solenoid5, solenoid6};

        allSolenoidsRetracted = true;
        allSolenoidsStationary = true;
        
        foreach (SolenoidController solenoid in solenoids) 
        {
            solenoid.UpdateHandPosition(trackPosition);
            solenoid.UpdateFretTrackLength(trackLength);
            solenoid.SetTiltSpeed(solenoidTiltSpeed);
            solenoid.SetMaxTiltX(solenoidMaxTiltX);
            solenoid.SetMaxTiltZ(solenoidMaxTiltZ);
            solenoid.SetCoreSpeed(solenoidCoreSpeed);
            if (solenoid.reverse)
            {
                solenoid.SetTrackLength(solenoidTrackReverseLength);
            }
            else 
            {
                solenoid.SetTrackLength(solenoidTrackLength);
            }
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
            trackPosition = GetTrackPosition();
            float newZ = Mathf.Lerp(transform.localPosition.z, trackDistance, Time.deltaTime * travelSpeed);
            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);
        }

        rawTrackDistance = Mathf.Round(transform.localPosition.z * 1000f) * 0.001f;
        isStationary = Mathf.Abs(Mathf.Abs(trackDistance) - Mathf.Abs(rawTrackDistance)) < 0.05f;
        if (!allSolenoidsRetracted) {isStationary = true;}
        if (isStationary) {
            currentPositionMatrix = GetCurrentPositionMatrix();
            trackDistance = rawTrackDistance;
        }

        // calculate solenoid track length/spacing that varies with track position
        solenoidTrackLength = (fretLengths[trackPosition+2] - fretLengths[trackPosition]) * trackLength;
        solenoidTrackReverseLength = (fretLengths[trackPosition-1] - fretLengths[trackPosition-2]) * trackLength;
        solenoidZSpacing = (fretLengths[trackPosition] - fretLengths[trackPosition-1]) * trackLength;
    }

    public float[,] GetCurrentPositionMatrix()
    {
        float[,] positionMatrix = new float[3,2];
        SolenoidController[] solenoids = {solenoid1, solenoid2, solenoid3, solenoid4, solenoid5, solenoid6};
        // iterate through all 6 solenoids, and map their current position representation to the 3x2 matrix
        for (int i = 0; i < solenoids.Length; i++)
        {
            int row = i / 2;
            int col = i % 2;
            // set the current position matrix value to the integer of the current solenoid's encoded xtilt, ztilt, and trackposition as 100ths, 10ths, and 1's place, respectively
            positionMatrix[row, col] = solenoids[i].GetEncoded();
        }
        return positionMatrix;
    }

    public int GetTrackPosition(float distance = -1f)
    {
        if (distance == -1f) {distance = rawTrackDistance;}
        for (int i = 0; i < fretLengths.Length; i++)
        {
            if ((distance - fretLengths[i] * trackLength) < 0.1f)
            {
                return i;
            }
        }
        return -1;
    }

    public void SetTrackPosition(int position) {
        SetTrackDistance(fretLengths[position] * trackLength);
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
    public void SetSolenoidTrackReverseLength(float length)
    {
        solenoidTrackReverseLength = length;
    }
}
