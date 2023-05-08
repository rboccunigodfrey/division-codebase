using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SActions { 
    TiltX,
    TiltZ,
    TrackPosition,
    Activation,
}

public enum FHActions { 
    TrackPosition,

}

static class SActionsMethods 
{
    public static void SetFor(this SActions action, SolenoidController solenoid, int value) 
    {
        switch(action) 
        {
            case SActions.TiltX:
                solenoid.SetTiltAngleX(value * 10f);
                break;
            case SActions.TiltZ:
                solenoid.SetTiltAngleZ(value * 10f);
                break;
            case SActions.TrackPosition:
                solenoid.SetTrackPosition(value);
                break;
            case SActions.Activation:
                solenoid.SetActive(value == 1);
                break;
        }
    }
}
static class FHActionsMethods 
{
    public static void SetFor(this FHActions action, FretHandController fretHand, int value) 
    {
        switch(action) 
        {
            case FHActions.TrackPosition:
                fretHand.SetTrackPosition(value);
                break;
        }
    }
}

public class LearnerController : MonoBehaviour
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
        int[,] chords = new int[File.ReadLines("twinkle.csv").Count(), 6, 4];
        int[] chords1D = File.ReadAllText("twinkle.csv").Split(',').Select(int.Parse).ToArray();
        for (int i = 0; i < chords1D.Length; i++)
        {
        #   chords[i / 24, (i / 4) % 6, i % 4] = chords1D[i];
        }
        
        // print chords to unity console
		for (int i = 0; i < chords.GetLength(0); i++)
        {
            string chord = "";
            for (int j = 0; j < chords.GetLength(1); j++)
            {
                for (int k = 0; k < chords.GetLength(2); k++)
                {
                    chord += chords[i, j, k];
                }
                chord += " ";
            }
            Debug.Log(chord);
        }

        StartCoroutine(Randomizer(solenoids));
    }

    IEnumerator Randomizer(SolenoidController[] solenoids)
    {
        int position = 3;
        while (true) 
        {
            FHActions.TrackPosition.SetFor(fretHand, position);
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

                    int newActiveState = Random.Range(0, 2);
                    int newTiltZ;
                    int newTrackPosition;

                    newTiltZ = newActiveState == 1 ? Random.Range(-1, 2) : 0;

                    SActions.Activation.SetFor(solenoid, newActiveState);
                    if (solenoid.reverse) 
                    {
                        newTrackPosition = Random.Range(0, 2);
                    } else 
                    {
                        newTrackPosition = Random.Range(0, 3);
                    }
                    SActions.TrackPosition.SetFor(solenoid, newTrackPosition);

                    SActions.TiltZ.SetFor(solenoid, newTiltZ);
                }
            }
            yield return new WaitForSeconds(1);
            foreach (SolenoidController solenoid in solenoids)
            {
                SActions.Activation.SetFor(solenoid, 0);
                SActions.TiltZ.SetFor(solenoid, 0);
            }
            position ++;
            if (position > 19) {position = 3;}
        }
    }
}
