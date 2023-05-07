import bpy
import math


class Solenoid:
    def __init__(self, name, x=0, y=0, z=0, reverse=False):
        self.x = x
        self.y = y
        self.z = z
        self.name = name
        self.core = None
        self.housing = None
        self.reverse = reverse
        self.create()

    def _create_core(self):
        bpy.ops.mesh.primitive_cylinder_add(vertices=32, radius=0.6, depth=6, end_fill_type='NGON', location=(self.x, self.y, self.z))
        self.core = bpy.context.active_object
        self.core.name = self.name + "_core"
        self.core.scale = (1, 1, 1.5)

    def _create_housing(self):
        bpy.ops.mesh.primitive_cube_add(size=3, enter_editmode=False, align='WORLD', location=(self.x, self.y, self.z), scale=(1, 1, 1))
        self.housing = bpy.context.active_object
        self.housing.name = self.name + "_housing"
        self.housing.scale = (1, 1, 2.2)  # Adjust the housing scale to make it longer

    def _group_objects(self):
        bpy.ops.object.select_all(action='DESELECT')
        self.core.select_set(True)
        self.housing.select_set(True)
        bpy.context.view_layer.objects.active = self.housing
        bpy.ops.object.join()

    def create(self):
        self._create_core()
        self._create_housing()
        self._group_objects()
        
    def update_core_position(self, active):
        if active:
            self.core.location.z = self.z - 0.6
        else:
            self.core.location.z = self.z
        self.core.keyframe_insert(data_path="location", frame=frame)
    
    def update(self, active): 
        self._update_core(self, active)
        
    def get_tilt_x(self):
        return math.degrees(self.housing.rotation_euler.x)

    def set_tilt_x(self, tilt_x):
        self.x_tilt = tilt_x
        self.housing.rotation_euler.x = math.radians(tilt_x)

    def get_core_z(self):
        return self.core.location.z

    def set_core_z(self, core_z):
        self.core_position = core_z - self.z
        self.core.location.z = core_z

    def get_distance_y(self):
        return self.y_travel

    def set_distance_y(self, distance_y):
        self.y_travel = distance_y
        self.housing.location.y = self.y + distance_y
        
class FretHand:
    def __init__(self):
        self.solenoids = []
        self.create()
        
    def create(self):
        # Create 6 Solenoids in a 3x2 grid
        solenoid_count_x = 3
        solenoid_count_y = 2
        solenoid_spacing = 5

        for i in range(solenoid_count_x):
            for j in range(solenoid_count_y):
                x = i * solenoid_spacing  + (solenoid_spacing * 0.5*j)
                y = j * solenoid_spacing
                z = 0
                name = "solenoid_" + str(i) + "_" + str(j)
                solenoid = Solenoid(name, x=x, y=y, z=z, reverse=bool(j))
                self.solenoids.append(solenoid)
                
                
class LerpHandler:
    def __init__(self, fret_hand, fps):
        self.fret_hand = fret_hand
        self.fps = fps
        self.animations = []

    def set_solenoid_core(self, solenoid, target_position, duration_seconds):
        duration = int(duration_seconds * self.fps)
        current_position = solenoid.get_core_z()

        for frame in range(duration + 1):
            progress = frame / duration
            position = current_position + (target_position - current_position) * progress
            solenoid.set_core_position(position)
            solenoid.set_core_z(position)
            # solenoid.core.keyframe_insert(data_path="location", frame=frame)
            yield

    def set_solenoid_tilt(self, solenoid, target_tilt, duration_seconds):
        duration = int(duration_seconds * self.fps)
        current_tilt = solenoid.get_tilt_x()

        for frame in range(duration + 1):
            progress = frame / duration
            tilt = current_tilt + (target_tilt - current_tilt) * progress
            solenoid.set_tilt_x(tilt)
            solenoid.set_tilt_x(tilt, frame)
            yield

    def add(self, animation):
        # Add animations to the list
        for solenoid in self.fret_hand.solenoids:
            self.animations.append(self.animate_solenoid_core(solenoid, 0.6, 0.2))
            self.animations.append(self.animate_solenoid_tilt(solenoid, 20, 0.1))

        # Run animations
        while self.animations:
            finished_animations = []

            # Execute one step of each animation
            for animation in self.animations:
                try:
                    next(animation)
                except StopIteration:
                    finished_animations.append(animation)

            # Remove finished animations
            for animation in finished_animations:
                self.animations.remove(animation)

            # Update the frame
            bpy.context.scene.frame_set(bpy.context.scene.frame_current + 1)


fh = FretHand()
ah = AnimationHandler(fh, bpy.context.scene.render.fps)
ah.animate()
