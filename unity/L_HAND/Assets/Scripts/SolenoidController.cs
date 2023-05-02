using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolenoidController : MonoBehaviour
{ 
    public GameObject core; // The core of the solenoid
    public bool reverse = false;
    public bool isActive = false; // Whether the solenoid is active or not
    public float tiltAngleX = 0.0f; // The angle at which the solenoid is tilted
    public float tiltAngleZ = 0.0f; // The angle at which the solenoid is tilted
    public float trackDistance = 0.0f;
    
    [SerializeField] private int fretHandPosition;
    [SerializeField] private int trackPosition;
    [SerializeField] private float encodedRepresentation;
    [SerializeField] private float fretTrackLength;
    [SerializeField] private float tiltSpeed = 100.0f; // The speed at which the core moves
    [SerializeField] private float maxTiltX = 10f;
    [SerializeField] private float maxTiltZ = 10f;
    [SerializeField] private float coreSpeed = 100.0f;
    [SerializeField] private float trackLength = 5.0f;
    [SerializeField] private float rawTiltX;
    [SerializeField] private float rawTiltZ;
    [SerializeField] private float rawCoreDepth;
    [SerializeField] private float rawTrackDistance;
    [SerializeField] private bool isStationary;
    [SerializeField] private float solenoidZSpacing = 1.75f;
    private float coreStartY;
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
        coreStartY = core.transform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        // Clamp tilt angle X
        tiltAngleX = Mathf.Round(Mathf.Clamp(tiltAngleX, -maxTiltX, maxTiltX) * 10f) * 0.1f;
        // Clamp tilt angle Z
        tiltAngleZ = Mathf.Round(Mathf.Clamp(tiltAngleZ, -maxTiltZ, maxTiltZ) * 10f) * 0.1f;
        // Apply tilt angle to solenoid transform
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(tiltAngleX, 0f, tiltAngleZ), Time.deltaTime * tiltSpeed);
        
        trackPosition = GetTrackPosition();
        if (isStationary) {encodedRepresentation = GetEncoded();}

        // Apply position change to solenoid (along track)
        // adjust for reverse position:
        float adjust = System.Convert.ToInt32(reverse) * solenoidZSpacing;
        trackDistance = Mathf.Round(trackDistance * 1000f) * 0.001f;
        float newTrackDistance = trackDistance + adjust;
        newTrackDistance = Mathf.Clamp(newTrackDistance, adjust, trackLength + adjust);
        trackDistance = newTrackDistance - adjust;
        if (reverse) {newTrackDistance = newTrackDistance * -1;}
        float newZ = Mathf.Lerp(transform.localPosition.z, newTrackDistance, Time.deltaTime * coreSpeed);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, newZ);

        // Raw readonly data out
        rawTiltX = transform.rotation.eulerAngles.x;
        if (rawTiltX > 340f)
        {
            rawTiltX = rawTiltX - 360f;
        }
        rawTiltX = Mathf.Round(rawTiltX * 1000f) * 0.001f;

        rawTiltZ = transform.rotation.eulerAngles.z;
        if (rawTiltZ > 340f)
        {
            rawTiltZ = rawTiltZ - 360f;
        }
        rawTiltZ = Mathf.Round(rawTiltZ * 1000f) * 0.001f;
        rawTrackDistance = Mathf.Round((transform.localPosition.z + adjust) * 1000f) * 0.001f;

        isStationary = Mathf.Abs(Mathf.Abs(rawTrackDistance) - Mathf.Abs(trackDistance)) < 0.05f && Mathf.Abs(rawTiltX - tiltAngleX) <= 0.1f && Mathf.Abs(rawTiltZ - tiltAngleZ) <= 0.1f;


        // Apply position change to core

        float yGoal = coreStartY;
        if (isActive && isStationary) {yGoal = -0.4f;}
        float newY = Mathf.Lerp(core.transform.localPosition.y, yGoal, Time.deltaTime * coreSpeed);
        core.transform.localPosition = new Vector3(core.transform.localPosition.x, newY, core.transform.localPosition.z);

    
        rawCoreDepth = Mathf.Round(core.transform.localPosition.y * 1000f) * 0.001f;
        

        // update colors
        var coreRenderer = core.GetComponent<Renderer>();

        if (isActive&& isStationary) 
        {
            coreRenderer.material.SetColor("_Color", Color.red);
        } else 
        {
            coreRenderer.material.SetColor("_Color", Color.white);
        }
    }

        public void SetActive(bool a)
    {
        isActive = a;
    }
    public void UpdateHandPosition(int position)
    {
        fretHandPosition = position;
    }
    public void UpdateFretTrackLength(float length)
    {
        fretTrackLength = length;
    }
    public void SetTiltSpeed(float speed)
    {
        tiltSpeed = speed;
    }
    public void SetTiltAngleX(float angle)
    {
        tiltAngleX = angle;
    }
    public void SetTiltAngleZ(float angle)
    {
        tiltAngleZ = angle;
    }
    public void SetMaxTiltX(float angle)
    {
        maxTiltX = angle;
    }
    public void SetMaxTiltZ(float angle)
    {
        maxTiltZ = angle;
    }
    public void SetCoreSpeed(float speed)
    {
        coreSpeed = speed;
    }
    public void SetTrackDistance(float distance)
    {
        trackDistance = distance;
    }

    public void SetTrackLength(float length)
    {
        trackLength = length;
    }

    public void SetSolenoidZSpacing(float spacing) {
        solenoidZSpacing = spacing;
    }

    public float GetTrackLength()
    {
        return trackLength;
    }

    public int GetDiscreteTiltX()
    {
        return System.Convert.ToInt32(tiltAngleX / maxTiltX);
    }

    public int GetDiscreteTiltZ()
    {
        return System.Convert.ToInt32(tiltAngleZ / maxTiltZ);
    }

    public int GetTrackPosition(float distance = -1f)
    {
        /* If no distance is given, use the raw distance
        if (distance == -1f) {distance = rawTrackDistance;}

        // Loop through all the frets to see if the given distance is close enough to one of them
        for (int i = 0; i < fretLengths.Length; i++)
        {
            // If the distance is within 0.1 units of the fret, then the fret is the closest match
            if (((fretTrackLength + distance +) * fretLengths[i]) < 0.1f)
            {
                return i;
            }
        }

        // If no fret is close enough, return -1
        return -1; */
        return trackPosition;
    }

    public int GetEncoded()
    {
        return ((GetDiscreteTiltZ() + 2) * 10 + (GetTrackPosition()+1)) * (isActive? 1 : -1);
    }

    public void SetTrackPosition(int position)
    {
        trackPosition = position;
        if (reverse) {position = position * -1;}
        SetTrackDistance(Mathf.Abs(fretLengths[fretHandPosition + position] - fretLengths[fretHandPosition]) * fretTrackLength);
    }

    public bool IsStationary()
    {
        return isStationary;
    }
}
