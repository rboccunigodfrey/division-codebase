import pymunk
import pygame
import pymunk.pygame_util
import sys 


pygame.init()

screen = pygame.display.set_mode((600, 600))
pygame.display.set_caption("L_HAND SIM")
clock = pygame.time.Clock()
fps = 50.0
space = pymunk.Space()
space.gravity = (0.0, 9.0)
draw_options = pymunk.pygame_util.DrawOptions(screen)

sols = []

for i in range(6):
    sol_fixt_body = pymunk.Body(body_type=pymunk.Body.STATIC)
    sol_fixt_body.position = (150 + 150*(i%3), 150 + 150*(int(i/3)))
    sol_body = pymunk.Body(body_type=pymunk.Body.KINEMATIC)
    sol_body.position = sol_fixt_body.position
    sol_shape = pymunk.Poly.create_box(sol_body, (100, 170))

    sol_joint = pymunk.PinJoint(sol_body, sol_fixt_body, (0, 0), (0, 0))
    space.add(sol_body, sol_joint, sol_shape)

while True:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            sys.exit(0)
        elif event.type == pygame.KEYDOWN and event.key == pygame.K_ESCAPE:
            sys.exit(0)
        elif event.type == pygame.KEYDOWN and event.key == pygame.K_p:
            pygame.image.save(screen, "screencaps/l_hand_sim.png")
                
    space.step(1/fps)
    screen.fill(((255, 255, 255)))
    space.debug_draw(draw_options)

    pygame.display.flip()
    clock.tick(fps)
