import random
from enum import Enum

import numpy as np

NOTES = ["C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B"]


class Scales(Enum):
    IONIAN = 0
    DORIAN = 2
    PHRYGIAN = 4
    LYDIAN = 5
    MIXOLYDIAN = 7
    AEOLIAN = 9
    LOCRIAN = 11


PATTERNS_C_IONIAN = [
    [[0, 1, 3], [0, 2, 3], [0, 2, 3], [0, 2, 4], [0, 1, 3], [0, 1, 3]],
    [[0, 2, 4], [1, 2, 4], [1, 2, 4], [1, 3, 4], [0, 2, 4], [0, 2, 4]],
    [[1, 3], [0, 1, 3], [0, 1, 3], [0, 2, 3], [1, 3, 4], [1, 3]],
    [[0, 2, 4], [0, 2, 4], [0, 2, 4], [1, 2, 4], [0, 2, 3], [0, 2, 4]],
    [[1, 3, 4], [1, 3, 4], [1, 3], [0, 1, 3], [2, 3, 4], [1, 3, 4]],
    [[0, 2, 3], [0, 2, 3], [0, 2, 4], [0, 2, 4], [0, 1, 3], [0, 2, 3]],
    [[1, 2, 4], [1, 2, 4], [1, 3, 4], [1, 3, 4], [0, 2, 4], [1, 2, 4]],
    [[0, 1, 3], [0, 1, 3], [0, 2, 3], [0, 2, 3], [1, 3], [0, 1, 3]],
    [[0, 2, 4], [0, 2, 4], [1, 2, 4], [1, 2, 4], [0, 2, 4], [0, 2, 4]],
    [[1, 3, 4], [1, 3], [0, 1, 3], [0, 1, 3], [1, 3, 4], [1, 3, 4]],
    [[0, 2, 3], [0, 2, 4], [0, 2, 4], [0, 2, 4], [0, 2, 3], [0, 2, 3]],
    [[1, 2, 4], [1, 3, 4], [1, 3, 4], [1, 3], [1, 2, 4], [1, 2, 4]]]


def get_pattern(key, scale=Scales.IONIAN):
    toIndex = (len(NOTES) - NOTES.index(key)) + scale.value
    if toIndex >= len(PATTERNS_C_IONIAN):
        toIndex = toIndex - len(PATTERNS_C_IONIAN)

    return PATTERNS_C_IONIAN[toIndex:] + PATTERNS_C_IONIAN[:toIndex]


print(get_pattern("D", scale=Scales.MIXOLYDIAN))


def generate_chord(pattern=None, key="C", scale=Scales.IONIAN, position=0, include_position=False, note_count=3):
    # Generate a random chord
    if pattern is None:
        pattern = get_pattern(key, scale=scale)
    chord = []
    chosen_strings = random.sample(range(6), note_count)
    for i in range(6):
        if i in chosen_strings:
            chord.append(random.choice(pattern[position][i]) + include_position * position)
        else:
            chord.append(-1)
    return chord


def generate_note(pattern=None, key="C", scale=Scales.IONIAN, position=0, include_position=False):
    generate_chord(pattern=pattern, key=key, scale=scale, position=position, include_position=include_position,
                   note_count=1)


print(generate_chord(
    key="D",
    scale=Scales.IONIAN,
    position=random.randrange(0, 5),
    include_position=True,
    note_count=random.randrange(2, 5)))

from sklearn.ensemble import RandomForestRegressor

POPULATION = 100
NOTES_PER = 100


def post_process_duration(y_pred):
    # Round duration to the nearest multiple of 0.125
    y_pred[:] = np.round(y_pred[:] / 0.125) * 0.125

    return y_pred


def post_process_velocity(y_pred):
    # Clip velocity to the range 1-127
    y_pred[:] = np.clip(np.round(y_pred[:]), 1, 127)

    return y_pred


def preprocess_input(X):
    # Convert chords to binary
    X = np.array([[''.join([bin(x)[2:].zfill(6) for x in row]) for row in level] for level in (X + 1)])

    return X


# toy dataset
# chordsX = [[chord, position]...]
# chordsY = [[duration, velocity]...]
rand_chords_x = np.array([[generate_chord(position=i % 12, note_count=(random.randrange(1, 5)), include_position=True)
                           for i in range(NOTES_PER)] for _ in range(POPULATION)])
print(rand_chords_x)

rand_chords_x_flat = preprocess_input(rand_chords_x)
print(rand_chords_x_flat)

rand_durations = np.array(
    [[round(random.random() / 0.125) * 0.125 for _ in range(NOTES_PER)] for _ in range(POPULATION)])
rand_velocities = np.array([[random.randrange(50, 127) for _ in range(NOTES_PER)] for _ in range(POPULATION)])
# chordsYFlat = np.reshape(chordsY, (12, 20))
print(rand_durations)
print(rand_velocities)

durRegr = RandomForestRegressor(n_estimators=100)
durRegr.fit(rand_chords_x_flat, rand_durations)
velRegr = RandomForestRegressor(n_estimators=100)
velRegr.fit(rand_chords_x_flat, rand_velocities)

X_test = np.array([[generate_chord(position=i % 12, note_count=(random.randrange(1, 5)), include_position=True) for i in
                    range(NOTES_PER)] for _ in range(POPULATION)])
print(preprocess_input(X_test))
print(post_process_duration(durRegr.predict(preprocess_input(X_test))))
print(post_process_velocity(velRegr.predict(preprocess_input(X_test))))

# print(post_process_output(regr.predict()))

'''

TODO: IDEAS

- Train an ML algorithm on a dataset of chords arrays (like above) and their corresponding duration and velocity

'''
