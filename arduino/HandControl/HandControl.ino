// Define pin connections & motor's steps per revolution
#include <Servo.h>

int pins[6][2] = {{13, 12},{11, 10},{9, 8},{7, 6},{5, 4},{3, 2}};
int pluckPins[6] = {A0, A1, A2, A3, A4, A5};

Servo pluckServo0;
Servo pluckServo1;
Servo pluckServo2;
Servo pluckServo3;
Servo pluckServo4;
Servo pluckServo5;

Servo pluckServos[6] = {pluckServo0, pluckServo1, pluckServo2, pluckServo3, pluckServo4, pluckServo5};

double curPosns[6] = {0, 0, 0, 0, 0, 0};
int pServoValues[6] = {-20, -20, -20, -20, -20, -20};

const int stepsPerRevolution = 400;
const int noteCount = 8;

int instructions[noteCount][7] = 
{
  {0, 0, 0, 1, -1, -1, 0},
  {-1, -1, 1, 1, -1, -1, 400},
  {3, -1, 2, 1, -1, -1, 800},
  {-1, -1, 3, 1, -1, -1, 1200},
  {2, 0, 3, 1, -1, -1, 1600},
  {-1, -1, 2, 1, -1, -1, 2000},
  {1, -1, 1, 1, -1, -1, 2400},
  {-1, -1, 0, 1, -1, -1, 2800},
 };

int timings[noteCount];


void setup()
{
  // Declare pins as Outputs
  for (int c = 0; c < noteCount; c++)
  {
    timings[c] = instructions[c][6];
  }
  for (int f = 2; f < 14; f++) {
    pinMode(f, OUTPUT);
  }

  for (int pPin = 0; pPin < 6; pPin++) {
    pinMode(pluckPins[pPin], OUTPUT);
    pluckServos[pPin].attach(pluckPins[pPin]);
  }
}

void SetNotePosns(int index) {
  int dists[6];
  int distsum = 0;
  for (int f = 0; f < 6; f++) 
  {
    dists[f] = curPosns[f] - instructions[index][f];
    if (instructions[index][f] == -1)
    {
      if (index < sizeof(instructions)/ sizeof(instructions[0]) - 1)
      {
        if (instructions[index+1][f] == -1) 
        {
          dists[f] = 0;
        } else 
        {
          dists[f] = curPosns[f] - instructions[index+1][f];
        }
      } else 
      {
        dists[f] = 0;
      }
    }
    distsum = distsum + dists[f];
  }
  while (distsum != 0) 
  {
    distsum = 0;
    for (int f = 0; f < 6; f++) 
    {
      dists[f] = curPosns[f] - instructions[index][f];
      if (instructions[index][f] == -1)
      {
        if (index < sizeof(instructions)/ sizeof(instructions[0]) - 1)
        {
          if (instructions[index+1][f] == -1) 
          {
            dists[f] = 0;
          } else 
          {
            dists[f] = curPosns[f] - instructions[index+1][f];
          }
        } else 
        {
          dists[f] = 0;
        }
      }
      distsum = distsum + dists[f];
    }
    for (int f = 0; f < 6; f++) 
    {
      int stepPin = pins[f][0];
      int dirPin = pins[f][1];
      float delta = 0.025;
      if (dists[f] == 0) 
      {
        digitalWrite(stepPin, LOW);
        continue;
      }
      else if (dists[f] > 0) 
      {
        digitalWrite(dirPin, LOW);
        //delta = 0.05 * dists[f];
        curPosns[f] = curPosns[f] - delta;
      }
      else {
        digitalWrite(dirPin, HIGH);
        //delta = -0.05 * dists[f];
        curPosns[f] = curPosns[f] + delta;
      }
      digitalWrite(stepPin, HIGH);
    }
    delayMicroseconds(1000);
    for (int f = 0; f < 6; f++) 
    {
      int stepPin = pins[f][0];
      digitalWrite(stepPin, LOW);
    }
    delayMicroseconds(1000);
  }
}

void PluckStrings(int index) 
{
  for (int f = 0; f < 6; f++) 
  {
    if (instructions[index][f] != -1)
    {
      pServoValues[f] = pServoValues[f] * -1;
      pluckServos[f].write(pServoValues[f] + 90);
    }
  }
  delay(30);
}


void loop()
{
  int previousTiming = 0;

  for (int c = 0; c < noteCount; c++)
  {
    SetNotePosns(c);
    PluckStrings(c);
    delay(timings[c] - previousTiming);
    previousTiming = timings[c];
  }
 
}
