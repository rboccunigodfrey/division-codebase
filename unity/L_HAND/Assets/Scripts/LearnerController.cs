using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public enum SActions { 
    TiltX,
    TiltZ,
    TrackPosition,
    Activation,
	Reverse,
}

public enum FHActions { 
    TrackPosition,
    Fretting,
    Moving
}

static class SActionsMethods 
{
    public static int GetFor(this SActions action, SolenoidController solenoid) 
    {
        switch(action) 
        {
            case SActions.TiltX:
                return solenoid.GetDiscreteTiltX();
            case SActions.TiltZ:
                return solenoid.GetDiscreteTiltZ();
            case SActions.TrackPosition:
                return solenoid.GetTrackPosition();
            case SActions.Activation:
                return solenoid.IsActive() ? 1 : 0;
			case SActions.Reverse:
                return solenoid.IsReverse() ? 1 : 0;
			default:
                throw new System.ArgumentException(
"Invalid solenoid getter action");
        }
    }

	public static void SetFor(this SActions action, SolenoidController solenoid, int value) 
    {
        switch(action) 
        {
            case SActions.TiltX:
				if (Mathf.Abs(value) > 1) {
					throw new System.ArgumentException(
"Invalid solenoid X tilt value (must be between -1 and 1, entered " + value + ")");
				}
                solenoid.SetTiltAngleX(value * 10f);
                break;
            case SActions.TiltZ:
				if (Mathf.Abs(value) > 1) {
					throw new System.ArgumentException(
"Invalid solenoid Z tilt value (must be between -1 and 1, entered " + value + ")");
				}
                solenoid.SetTiltAngleZ(value * 10f);
                break;
            case SActions.TrackPosition:
				if (value > (solenoid.reverse ? 2 : 3) || value < 0) {
					throw new System.ArgumentException(
"Invalid solenoid track position (must be between 0 and " + (solenoid.reverse ? 2 : 3) + ", entered " + value + ")");
				}
                solenoid.SetTrackPosition(value);
                break;
            case SActions.Activation:
				if (value != 0 && value != 1) {
					throw new System.ArgumentException(
"Invalid solenoid activation value (must be 0 or 1, entered " + value + ")");
				}
                solenoid.SetActive(value == 1);
                break;
			default:
                throw new System.ArgumentException(
"Invalid solenoid setter action");
        }
    }
}

static class FHActionsMethods 
{
	public static int GetFor(this FHActions action, FretHandController fretHand) 
    {
        switch(action) 
        {
            case FHActions.TrackPosition:
                return fretHand.GetTrackPosition();     
            case FHActions.Fretting:
				return (fretHand.AllSolenoidsRetracted()) ? 0 : 1;
            case FHActions.Moving:
	            return fretHand.IsStationary() ? 0 : 1;
			default:
                throw new System.ArgumentException(
"Invalid frethand getter action");
		}
    }
    public static void SetFor(this FHActions action, FretHandController fretHand, int value) 
    {
        switch(action) 
        {
            case FHActions.TrackPosition:
				if (value < 2 || value > 20) {
					throw new System.ArgumentException(
"Invalid frethand track position (must be between 3 and 19, entered " + value + ")");
				}
                fretHand.SetTrackPosition(value);
                break;
			default:
                throw new System.ArgumentException(
"Invalid frethand setter action");
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
        
		string path = "../../python/twinkle.csv";

		// Read all lines from the CSV file and split them by comma
		string[][] csvData = File.ReadAllLines(path).Select(line => line.Split(',')).ToArray();
		
		int x = csvData.Length, y = 5,  z = 6; // Set the dimensions of your array

		// Initialize the final array with the desired dimensions: (x, y, z)
		int[,,] chords = new int[x, y, z];

		// Iterate through the CSV data and populate the final array
		for (int i = 0; i < x; i++)
		{
			for (int j = 0; j < y; j++)
			{
			    for (int k = 0; k < z; k++)
				{
					chords[i, j, k] = int.Parse(csvData[i][j * z + k]);
				}
		    }
		}
		
		
		HashSet<int> keyframes_set = new HashSet<int>();
		for (int i = 0; i < x; i++)
		{
			keyframes_set.Add(chords[i, 0, 1]);
		}
		int[] keyframes = keyframes_set.ToArray();

		// go thru and print all elements of 3d array to unity console
		string chordString = "[";
		for (int i = 0; i < x; i++)
		{
			chordString += "[";
		    for (int j = 0; j < y; j++)
			{
				chordString += "[";
                for (int k = 0; k < z; k++)
                {
					chordString += chords[i, j, k];
					if (k != z - 1) {chordString += ",";}
                }
				chordString += "]";
				if (j != y - 1) {chordString += ",\n";}
		    }
			chordString += "]";
			if (i != x - 1) {chordString += ",\n\n";}
		}
		chordString += "]";

		string kfString = "[";
		for (int i = 0; i < keyframes.Length; i++)
        {
            kfString += keyframes[i];
			if (i != keyframes.Length - 1) {kfString += ",";}
        }

		kfString += "]";
		Debug.Log(chordString);
		Debug.Log(chords.Length / 24);
		Debug.Log(kfString);
		Debug.Log(keyframes.Length);
		FHActions.TrackPosition.SetFor(fretHand, 3);
        StartCoroutine(Player(solenoids, chords, keyframes));
    }

    IEnumerator Player(SolenoidController[] solenoids, int[,,] chords, int[] keyframes)
	{
		int chordsPlayed = 0;
		int ticks = 0;

		while (chordsPlayed < chords.Length) 
        {
			Debug.Log("ticks: " + ticks);
			Debug.Log("get frethand position: " + FHActions.TrackPosition.GetFor(fretHand));
			if (FHActions.)
			for (int i = 0; i < keyframes.Length; i++)
            {
                if (keyframes[i] == ticks)
                {
	                if (chords[i, 0, 0] != -1)
	                {
		                FHActions.TrackPosition.SetFor(fretHand, chords[i, 0, 0]);
		                Debug.Log("set frethand position: " + chords[i, 0, 0]);

		                for (int j = 0; j < 6; j++)
		                {
			                if (chords[i, 1, j] > 0)
			                {
				                SActions.Activation.SetFor(solenoids[j], chords[i, 1, j] > 0 ? 1 : 0);
				                SActions.TrackPosition.SetFor(solenoids[j], chords[i, 1, j]);
			                }

			                SActions.TiltX.SetFor(solenoids[j], chords[i, 2, j]);
		                }
	                }
	                chordsPlayed ++;
                }
            }

			ticks ++;

			yield return new WaitForSeconds(0f);
        }	
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
