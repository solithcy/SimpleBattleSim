# SimpleBattleSim
A simple battle simulator with 3 classes.

## Gameplay
Two players will decide their team names and team composition. Teams can be made up of
characters from the following classes:
- Warrior
  - Health is between 6-15 instead of 1-10.
- Wizard
  - Attack is doubled (2-20), but takes 1 damage per attack.
- Cleric
  - Heals 1 HP every attack.

Characters get a random amount of health between 1-10, and attack between 1-10, unless
stated otherwise above. Characters will get a random "initiative" value in order to decide
turn order.

## Prerequisites
- Git
 - Dotnet SDK & Runtime

## Building
First, clone the repository:
```bash
$ git clone https://github.com/Solithcy/SimpleBattleSim
$ cd SimpleBattleSim
```
Then, to build the executable, from the project root run:
```bash
$ cd SimpleBattleSim # The project root has the SimpleBattleSim and Testing directories
$ dotnet build -c Release
```

The built executable will then be available at `bin/Release/net[version]/SimpleBattleSim.exe`.

## Running
To run without building, from the project root run:
```bash
$ cd SimpleBattleSim # The project root has the SimpleBattleSim and Testing directories
$ dotnet run
```

## Testing
To run the tests, from the project root run:
```bash
$ dotnet test # no need to cd into Testing
```

This project has 90% test coverage.

## Assumptions
This project deviates from the [brief](./Brief.md) in a few ways.

 - Fighters/Warriors are named Warriors. The name seemed to be interchangeable in the brief.
 - Turn order is decided by initiative.
 - The enemy character that's targeted isn't entirely random, as there are 3 different targeting strategies that characters will be assigned randomly. I believe this adds more interesting behaviour to the game. The behaviours are as follows:
   - **LowestHealth:** Attacks the lowest health enemy
   - **HighestHealth:** Attacks the highest health enemy
   - **Random:** Attacks a random enemy
 - Attack is random every turn and not decided upon class instantiation. In my opinion this makes fights more balanced and fair.
 
