using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizerController : MonoBehaviour
{
    public SolenoidController solenoid1;
    public SolenoidController solenoid2;
    public SolenoidController solenoid3;
    public SolenoidController solenoid4;
    public SolenoidController solenoid5;
    public SolenoidController solenoid6;
    public FretHandController fretHand;
    // Start is called before the first frame update
    void Start()
    {
        SolenoidController[] solenoids = {solenoid1, solenoid2, solenoid3, solenoid4, solenoid5, solenoid6};
        StartCoroutine(fretHandRandomizer(solenoids));
    }


    IEnumerator fretHandRandomizer(SolenoidController[] solenoids)
    {   
        while (true) 
        {
            fretHand.SetTrackDistance(Random.Range(0f, fretHand.GetTrackLength()));
            List<SolenoidController> chosenSolenoids = new List<SolenoidController>();

            while (chosenSolenoids.Count < 4)
            {
                int randomIndex = Random.Range(0, solenoids.Length);
                SolenoidController randomSolenoid = solenoids[randomIndex];
                if (!chosenSolenoids.Contains(randomSolenoid)) 
                {
                    chosenSolenoids.Add(randomSolenoid);
                }
            }
            foreach (SolenoidController solenoid in chosenSolenoids) 
            {
                if (fretHand.IsStationary()) {

                    bool newActiveState = Random.value < 0.4f;
                    float newTiltAngleX;
                    float newTiltAngleZ;
                    float newTrackDistance;

                    if (newActiveState)
                    {
                        newTiltAngleX = Random.Range(-1, 2)*10f;
                        newTiltAngleZ = Random.Range(-1, 2)*10f;
                        newTrackDistance = Random.Range(0f, solenoid.GetTrackLength());
                    } 
                    else 
                    {
                        newTiltAngleX = 0f;
                        newTiltAngleZ = 0f;
                        newTrackDistance = 0f;
                    }

                    solenoid.SetActive(newActiveState);
                    solenoid.SetTiltAngleX(newTiltAngleX);
                    solenoid.SetTiltAngleZ(newTiltAngleZ);
                    solenoid.SetTrackDistance(newTrackDistance);
                }
            }
            yield return new WaitForSeconds(Random.Range(1, 4)*0.25f);
            foreach (SolenoidController solenoid in solenoids)
            {
                solenoid.SetActive(false);
                solenoid.SetTiltAngleX(0f);
                solenoid.SetTiltAngleZ(0f);
            }
        }
    }
}
