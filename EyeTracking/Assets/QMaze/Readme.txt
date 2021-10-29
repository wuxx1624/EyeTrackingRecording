QMaze
=====

Thank you for downloading QMaze!
Please rate it if you liked it!

Using the Package
=================

1. Attach QMazeEngine script to the game object.
2. Specify the necessary parameters in the QMazeEngine script:
- Maze size
- Maze piece size
- Maze piece pack
- You can also specify the number, positions and directions of the start and finish maze piece, as well as obstacles and exits.
- Seed, if necessary
3. Specify the necessary parameters in the QMazePiecePack script:
- Set the game objects to corresponding pieces.
- If necessary, several game objects can be set to each piece. Click plus/minus to add/remove it.
- Use and frequency of occurrence can be specified for some pieces.
4. Click the "Generate a Maze" button to generate a maze in the edit-time mode or call "generateMaze" function from code

The package includes three demo scene, which you can study for a more complete understanding of the package

Support
=======

If there are any difficulties
or you have questions or suggestions,
please contact me: qtools.develop@gmail.com

Versions History
================
2.1
- Fixed bug when using a scene object as a piece of the maze

2.0
Important: if you have a previous version you will need to remove it before importing the new version. All QMazePiecePacks must be recreated.
- Option to add listeners for maze generated events added
- Now you can set the direction of the start and finish pieces and exits
- You can now drag&drop the geometry of the maze pieces into a pack (by object name)
- API improved
- Demo scenes are improved and restructured
- Some code documentation added
- Bugs fixed

1.3
- A list of obstacles added
- The option to use the None pieces added
- The option to generate mazes with the only path added
- QMazeEngine and QMazePiecePack inspectors improved
- Some bugs fixed

1.2
- Now you can create entries to the maze
- A demo scene added

1.1
- Two demo scenes added
- QMazeEngine inspector improved
- Broken Prefab links fixed

1.0
- Initial Release