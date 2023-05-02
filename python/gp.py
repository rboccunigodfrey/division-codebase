import random
import numpy as np
from deap import base, creator, tools, algorithms
from enum import Enum

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
    if (toIndex >= len(PATTERNS_C_IONIAN)):
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
    generate_chord(pattern=pattern, key=key, scale=scale, position=position, include_position=include_position, note_count=1)

print(generate_chord(
    key="D", 
    scale=Scales.IONIAN, 
    position=random.randrange(0, 5), 
    include_position=True,
    note_count=random.randrange(2, 5)))

'''

# Constants
NUM_INDIVIDUALS = 100
NUM_GENERATIONS = 50
MUTATION_PROB = 0.2
CROSSOVER_PROB = 0.5

def midi_to_solenoid(chord_pattern):
    # Convert MIDI chord pattern to solenoid instructions
    # This function should be implemented to convert the given chord pattern
    # to a list of solenoid instructions in the form of (SActions, value) tuples
    pass

def fitness_function(individual):
    # Evaluate the fitness of an individual
    # This function should be implemented to return a numerical score
    # representing how well the individual performs in converting
    # MIDI chord patterns to solenoid instructions

    pass

creator.create("FitnessMax", base.Fitness, weights=(1.0,))
creator.create("Individual", list, fitness=creator.FitnessMax)

toolbox = base.Toolbox()
toolbox.register("solenoid_instruction", midi_to_solenoid, chord_pattern=None)
toolbox.register("individual", tools.initRepeat, creator.Individual,
                 toolbox.solenoid_instruction, n=1)
toolbox.register("population", tools.initRepeat, list, toolbox.individual)

toolbox.register("evaluate", fitness_function)
toolbox.register("mate", tools.cxTwoPoint)
toolbox.register("mutate", tools.mutGaussian, mu=0, sigma=1, indpb=MUTATION_PROB)
toolbox.register("select", tools.selTournament, tournsize=3)

population = toolbox.population(n=NUM_INDIVIDUALS)

# Run the genetic algorithm
result, _ = algorithms.eaSimple(population, toolbox,
                                cxpb=CROSSOVER_PROB, mutpb=MUTATION_PROB,
                                ngen=NUM_GENERATIONS, verbose=True)

best_individual = tools.selBest(result, k=1)[0]
print("Best individual:", best_individual)

'''