# DnD Battler Project

For this project, you will create a simple Dungeons & Dragons (DnD) battle simulator. The goal is to implement a system that allows 2 teams of 3 players to fight each other

## Specification
- Each player will create a team of 3 characters. The options are:
    - Fighter
    - Wizard
    - Cleric

The team can be any combination of these characters, but each team must have exactly 3 characters.

The battle will then be simulated in a turn-based manner, where each character in a team takes turns to attack a member of the opposing team randomly. The battle will be played out until all three members of one team have no health and are defeated.

### Classes
Each class has a `Health` property, an `Attack` property. When a class is created the health and attack is given a separate random value between 1 and 10. The classes also have the following properties:
- **Warrior:** The Warrior gets a bonus of 5 to their health regardless of the random value.
- **Wizard:** The Wizard's attack is doubled regardless of the random value but looses 1 health point when they attack.
- **Cleric:** The Cleric can heal themselves for 1 health point whenever they attack.

### Battle Simulation
- Each player will take turns deciding on their 3 players to create a team. They will then give the team a name
- Once this is done, the system will simulate the battle by alternating turns between the two teams and reporting on the battle using a logger or console window.
- During the battle, the system should report on the current health of the remaining characters in each team after each turn.
- The battle continues until one team has no remaining characters.

### Submission
- You will need to submit your code along with a thorough test suite and a readme file that explains how to run your code and tests. This will be done via a Github link to your repository
- The readme should also include a brief explanation of how the battle simulation works and any assumptions made during the implementation.
- Any work submitted after the deadline with be reverted to the last commit before the deadline
- You are able to use any past projects or online documentation but no AI tools