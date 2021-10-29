#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace qtools.qmaze
{
	/// <summary>
	/// Class contains functions to generate mazes
	/// </summary>
	[ExecuteInEditMode]
	public class QMazeEngine : MonoBehaviour 
	{
		[Serializable] public class QMazeGeneratedEvent 		: UnityEvent<QMazeEngine> {}
		[Serializable] public class QMazePieceGeneratedEvent 	: UnityEvent<QMazePieceData> {}	
		[Serializable] public class QMazeGenerateProgressEvent 	: UnityEvent<float> {}

		[SerializeField] private QMazePiecePack piecePack;

		[SerializeField] private int mazeWidth = 25;
		[SerializeField] private int mazeHeight = 25;

		[SerializeField] private float mazePieceWidth = 10;
		[SerializeField] private float mazePieceHeight = 10;
		
		[SerializeField] private float mazeScale = 1.0f;

		[SerializeField] private bool startRandomPosition = false;
		[SerializeField] private int  startRandomPositionCount = 0;
		[SerializeField] private List<QVector2IntDir> startPositionList = new List<QVector2IntDir>();

		[SerializeField] private bool finishRandomPosition = false;
		[SerializeField] private int  finishRandomPositionCount = 0;
		[SerializeField] private List<QVector2IntDir> finishPositionList = new List<QVector2IntDir>();

		[SerializeField] private List<QVector2IntDir> exitPositionList = new List<QVector2IntDir>();

		[SerializeField] private bool obstacleIsNone = false;
		[SerializeField] private List<QVector2Int> obstaclePositionList = new List<QVector2Int>();

		[SerializeField] private bool onlyPath = false;

		[SerializeField] private bool useSeed = false;
		[SerializeField] private int seed = 0;	 

		[SerializeField] private QMazePieceData[][] mazeArray;	
		[SerializeField] private bool inited = false;

		/// <summary>
		/// Event is triggered at the end of the maze generation
		/// </summary>
		public QMazeGeneratedEvent mazeGeneratedEvent = new QMazeGeneratedEvent();

		/// <summary>
		/// Event is triggered at the generation of each maze piece
		/// </summary>
		public QMazePieceGeneratedEvent mazePieceGeneratedEvent = new QMazePieceGeneratedEvent();

		/// <summary>
		/// Event is triggered at the maze generation in asynchronous mode
		/// </summary>
		public QMazeGenerateProgressEvent mazeGenerateProgressEvent = new QMazeGenerateProgressEvent();

		private float generationProgress;
		private float instantiatingProgress;
		private List<CheckTask> checkTaskList = new List<CheckTask>();
		private List<QVector2Int> path;
		private QMazeOutputDirection lastDirection;
		private int startFinishLeft;
		
		private void Awake()		
		{
			if (!inited)
			{
				inited = true;
				
				if (gameObject.GetComponent<QMazePiecePack>() == null)
					piecePack = gameObject.AddComponent<QMazePiecePack>();
			}
		}

		/// <summary>
		/// Get the maze piece pack
		/// </summary>
		/// <returns>The maze piece pack</returns>
		public QMazePiecePack getMazePiecePack() 
		{
			return piecePack;
		}

		/// <summary>
		/// Set the maze piece pack
		/// </summary>
		/// <param name="piecePack">The maze piece pack</param>
		public void setMazePiecePack(QMazePiecePack piecePack)
		{
			this.piecePack = piecePack;
		}

		/// <summary>
		/// Get the width of the maze
		/// </summary>
		/// <returns>Width of the maze</returns>
		public int getMazeWidth() 
		{
			return mazeWidth;
		}

		/// <summary>
		/// Set the width of the maze
		/// </summary>
		/// <param name="mazeWidth">Width of the maze</param>
		public void setMazeWidth(int mazeWidth)
		{
			this.mazeWidth = mazeWidth;
		}

		/// <summary>
		/// Get the height of the maze
		/// </summary>
		/// <returns>Height of the maze </returns>
		public int getMazeHeight() 
		{
			return mazeHeight;
		}

		/// <summary>
		/// Set the height of the maze
		/// </summary>
		/// <param name="mazeHeight">Height of the maze </param>
		public void setMazeHeight(int mazeHeight)
		{
			this.mazeHeight = mazeHeight;
		}

		/// <summary>
		/// Get the width of the maze piece
		/// </summary>
		/// <returns>Width of the maze piece  </returns>
		public float getMazePieceWidth() 
		{
			return mazePieceWidth;
		}

		/// <summary>
		/// Set the width of the maze piece
		/// </summary>
		/// <param name="mazePieceWidth">Width of the maze piece</param>
		public void setMazePieceWidth(float mazePieceWidth)
		{
			this.mazePieceWidth = mazePieceWidth;
		}

		/// <summary>
		/// Get the height of the maze piece
		/// </summary>
		/// <returns>Height of the maze piece </returns>
		public float getMazePieceHeight() 
		{
			return mazePieceHeight;
		}

		/// <summary>
		/// Set the height of the maze piece
		/// </summary>
		/// <param name="mazePieceHeight">Height of the maze piece </param>
		public void setMazePieceHeight(float mazePieceHeight)
		{
			this.mazePieceHeight = mazePieceHeight;
		}

		/// <summary>
		/// Set the size of the maze piece
		/// </summary>
		/// <param name="mazePieceSize">Width and height of the maze piece</param>
		public void setMazePieceSize(float mazePieceSize)
		{
			this.mazePieceWidth = this.mazePieceHeight = mazePieceSize;
		}

		/// <summary>
		/// Set the width and height of the maze piece
		/// </summary>
		/// <param name="mazePieceWidth">Width of the maze piece  </param>
		/// <param name="mazePieceHeight">Height of the maze piece </param>
		public void setMazePieceSize(float mazePieceWidth, float mazePieceHeight)
		{
			this.mazePieceWidth = mazePieceWidth;
			this.mazePieceHeight = mazePieceHeight;
		}

		/// <summary>
		/// Sets the maze scale.
		/// </summary>
		/// <param name="mazeScale">Maze scale.</param>
		public void setMazeScale(float mazeScale)
		{
			this.mazeScale = mazeScale;
		}

		/// <summary>
		/// Gets the maze scale.
		/// </summary>
		/// <returns>The maze scale.</returns>
		public float getMazeScale()
		{
			return mazeScale;
		}

		/// <summary>
		/// Use random position of the start pieces
		/// </summary>
		/// <returns><c>true</c>, if random start positions are used, <c>false</c> otherwise.</returns>
		public bool isStartRandomPosition()
		{
			return startRandomPosition;
		}

		/// <summary>
		/// Setting the use of random start pieces. 
		/// If specific positions of the start pieces have previously been set, they will be removed
		/// </summary>
		/// <param name="startRandomPosition">If set to <c>true</c>, random positions of start pieces are used</param>
		public void setStartRandomPosition(bool startRandomPosition)
		{
			this.startRandomPosition = startRandomPosition;
			if (startRandomPosition) startPositionList.Clear();
			else startRandomPositionCount = 0;
		}

		/// <summary>
		/// Getting the number of random start pieces
		/// </summary>
		/// <returns>Number of random start pieces</returns>
		public int getStartRandomPositionCount()
		{
			return startRandomPositionCount;
		}

		/// <summary>
		/// Setting the number of random start pieces. 
		/// If specific positions of the start pieces have previously been set, they will be removed
		/// </summary>
		/// <param name="startRandomPositionCount">Number of random start pieces</param>
		public void setStartRandomPositionCount(int startRandomPositionCount)
		{
			startRandomPosition = true;
			this.startRandomPositionCount = startRandomPositionCount;
			startPositionList.Clear();
		}

		/// <summary>
		/// Getting a list of positions of the start pieces. 
		/// If random positions of the start pieces are used, their positions will be available after the maze generation.
		/// </summary>
		/// <returns>List of start positions</returns>
		public List<QVector2IntDir> getStartPositionList()
		{
			return startPositionList;
		}

		/// <summary>
		/// Setting a list of start positions.
		/// If the mode of random positions of the start pieces was used, it will be switched off.
		/// </summary>
		/// <param name="startPositionList">List of start positions</param>
		public void setStartPositionList(List<QVector2IntDir> startPositionList)
		{
			startRandomPosition = false;
			startRandomPositionCount = 0;
			this.startPositionList = startPositionList;
		}

		/// <summary>
		/// Use random position of the finish pieces
		/// </summary>
		/// <returns><c>true</c>, if random finish positions are used, <c>false</c> otherwise.</returns>
		public bool isFinishRandomPosition()
		{
			return finishRandomPosition;
		}

		/// <summary>
		/// Setting the use of random finish pieces. 
		/// If specific positions of the finish pieces have previously been set, they will be removed
		/// </summary>
		/// <param name="finishRandomPosition">If set to <c>true</c>', random positions of finish pieces are used</param>
		public void setFinishRandomPosition(bool finishRandomPosition)
		{
			this.finishRandomPosition = finishRandomPosition;
			if (finishRandomPosition) finishPositionList.Clear();
			else finishRandomPositionCount = 0;
		}

		/// <summary>
		/// Getting the number of random finish pieces
		/// </summary>
		/// <returns>Number of random finish pieces</returns>
		public int getFinishRandomPositionCount()
		{
			return finishRandomPositionCount;
		}

		/// <summary>
		/// Getting a list of positions of the finish pieces. 
		/// If random positions of the finish pieces are used, their positions will be available after the maze generation.
		/// </summary>
		/// <returns>List of finish positions</returns>
		public List<QVector2IntDir> getFinishPositionList()
		{
			return finishPositionList;
		}

		/// <summary>
		/// Setting the number of random finish pieces. 
		/// If specific positions of the finish pieces have previously been set, they will be removed
		/// </summary>
		/// <param name="finishRandomPositionCount">Number of random finish pieces</param>
		public void setFinishRandomPositionCount(int finishRandomPositionCount)
		{
			finishRandomPosition = true;
			this.finishRandomPositionCount = finishRandomPositionCount;
			finishPositionList.Clear();
		}

		/// <summary>
		/// Setting a list of finish positions. 
		/// If the mode of random positions of the finish pieces was used, it will be switched off.
		/// </summary>
		/// <param name="finishPositionList">List of finish positions</param>
		public void setFinishPositionList(List<QVector2IntDir> finishPositionList)
		{
			finishRandomPosition = false;
			finishRandomPositionCount = 0;
			this.finishPositionList = finishPositionList;
		}

		/// <summary>
		/// Getting a list of positions of the exits
		/// </summary>
		/// <returns>List of exit positions</returns>
		public List<QVector2IntDir> getExitPositionList()
		{
			return exitPositionList;
		}

		/// <summary>
		/// Setting a list of exit positions
		/// </summary>
		/// <param name="exitPositionList">List of exit positions</param>
		public void setExitPositionList(List<QVector2IntDir> exitPositionList)
		{
			this.exitPositionList = exitPositionList;
		}

		/// <summary>
		/// Use the None pieces in the areas of obstacles
		/// </summary>
		/// <returns><c>true</c>, if None pieces are used in the areas of obstacles, <c>false</c> otherwise.</returns>
		public bool isObstacleIsNone()
		{
			return obstacleIsNone;
		}

		/// <summary>
		/// Sets the obstacle is none.
		/// </summary>
		/// <param name="obstacleIsNone">If set to <c>true</c> None pieces are used in the areas of obstacles.</param>
		public void setObstacleIsNone(bool obstacleIsNone)
		{
			this.obstacleIsNone = obstacleIsNone;
		}

		/// <summary>
		/// Getting a list of positions of the obstacles
		/// </summary>
		/// <returns>List of obstacle positions</returns>
		public List<QVector2Int> getObstaclePositionList()
		{
			return obstaclePositionList;
		}

		/// <summary>
		/// Setting a list of obstacle positions
		/// </summary>
		/// <param name="obstaclePositionList">List of obstacle positions</param>
		public void setObstaclePositionList(List<QVector2Int> obstaclePositionList)
		{
			this.obstaclePositionList = obstaclePositionList;
		}
		
		/// <summary>
		/// Getting the flag of using the only path mode.
		/// </summary>
		/// <returns>If <c>true</c> is returned, then the only path mode is used, <c>false</c> otherwise.</returns>
		public bool isOnlyPathMode()
		{
			return onlyPath;
		}

		/// <summary>
		/// Setting the only path mode. 
		/// In this case, a maze is constructed so that there may be the only path from any point A to any point B.
		/// </summary>
		/// <param name="onlyPath">If set to <c>true</c>  the only path mode is enabled.</param>
		public void setOnlyPathMode(bool onlyPath)
		{
			this.onlyPath = onlyPath;
		}

		/// <summary>
		/// Getting the flag of using the seed generation.
		/// </summary>
		/// <returns><c>true</c>, if seed is used, <c>false</c> otherwise.</returns>
		public bool isUseSeed()
		{
			return useSeed;
		}

		/// <summary>
		/// Setting the flag of using the seed generation. 
		/// When using seed, the maze will be generated under a specific scenario, so you can re-build a specific maze at a particularly given seed.
		/// </summary>
		/// <param name="useSeed">If set to <c>true</c> use the seed generation</param>
		public void setUseSeed(bool useSeed)
		{
			this.useSeed = useSeed;
		}

		/// <summary>
		/// Getting the value of the seed
		/// </summary>
		/// <returns>Value of the seed</returns>
		public int getSeed()
		{
			return seed;
		}

		/// <summary>
		/// Setting the value of the seed
		/// </summary>
		/// <param name="seed">Value of the seed</param>
		public void setSeed(int seed)
		{
			this.seed = seed;
		}
        	
		/// <summary>
		/// Maze generation
		/// </summary>
		/// <param name="generateWithGeometry">If set to <c>true</c> geometry will also be generated</param>
		public void generateMaze(bool generateWithGeometry = true)
		{
			if (useSeed) QMath.setSeed(seed);
			IEnumerator generationEnumerator = generate(generateWithGeometry, false);
			while (generationEnumerator.MoveNext());
		}

		/// <summary>
		/// Maze generation in asynchronous mode
		/// </summary>
		/// <param name="activeMonoBehaviour">Object calling a coroutine</param>
		/// <param name="maxTime">Maximum time of asynchronous generation step</param>
		/// <param name="generateWithGeometry">If set to <c>true</c> geometry will also be generated</param>
		public void generateMazeAsync(MonoBehaviour activeMonoBehaviour, float maxTime = 0.1f, bool generateWithGeometry = true)
		{	
			if (useSeed) QMath.setSeed(seed);
			activeMonoBehaviour.StartCoroutine(generate(generateWithGeometry, true, maxTime));				 
		}

		/// <summary>
		/// Immediate destruction of geometry objects of the previously built maze.
		/// It will destroy all child objects.
		/// </summary>
		public void destroyImmediateMazeGeometry()
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				DestroyImmediate(transform.GetChild(i).gameObject);
			}
		}

		/// <summary>
		/// Getting the current maze generation progress in asynchronous mode
		/// </summary>
		/// <returns>The progress of the generation</returns>
	    public float getGenerationProgress()
	    {
			return (generationProgress + instantiatingProgress) / (2 * mazeWidth * mazeHeight);
	    }

		/// <summary>
		/// Getting the data of the generated maze
		/// </summary>
		/// <returns>Two-dimensional data array of pieces of the generated maze</returns>
		public QMazePieceData[][] getMazeData()
		{
			return mazeArray;
		}

		/// <summary>
		/// Determines whether a given point is inside the maze space
		/// </summary>
		/// <returns><c>true</c>, if the point is inside the maze space, <c>false</c> otherwise.</returns>
		/// <param name="point">Point</param>
		public bool pointInMaze(QVector2Int point)
		{
			bool inMaze = point.x >= 0 && point.x < mazeWidth && point.y >= 0 && point.y < mazeHeight;
			bool notInObstacle = !QListUtil.has(obstaclePositionList, point);
			return inMaze && notInObstacle;
		}

		private delegate int CheckTask(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir);
		private const int CHECK_CONTINUE = 0;
		private const int CHECK_BREAK    = 1;
		private const int CHECK_FAILED   = 2;

		private IEnumerator generate(bool generateWithGeometry = true, bool asynchronous = true, float maxTime = 0.1f)
		{
			generationProgress = 0;

            startFinishLeft = 0;

            if (startRandomPosition)
            {
                startPositionList.Clear();
                generateRandomPoints(startPositionList, startRandomPositionCount);
            }
            startFinishLeft += startPositionList.Count;

            if (finishRandomPosition)
            {
                finishPositionList.Clear();
                generateRandomPoints(finishPositionList, finishRandomPositionCount);
            }
            startFinishLeft += finishPositionList.Count;

			QVector2Int startGenerationPoint = new QVector2Int(QMath.getRandom(0, mazeWidth), QMath.getRandom(0, mazeHeight));
			while (QListUtil.has(startPositionList, startGenerationPoint) || 
			       QListUtil.has(finishPositionList, startGenerationPoint) || 
			       QListUtil.has(obstaclePositionList, startGenerationPoint))
			{
				startGenerationPoint.x = QMath.getRandom(0, mazeWidth);
				startGenerationPoint.y = QMath.getRandom(0, mazeHeight);
			}

			path = new List<QVector2Int>();
			mazeArray = new QMazePieceData[mazeWidth][];
			for (int px = 0; px < mazeWidth; px++) 
			{
				mazeArray[px] = new QMazePieceData[mazeHeight];
				for (int py = 0; py < mazeHeight; py++)
				{
					mazeArray[px][py] = new QMazePieceData(px, py);
				}
			}
			
			lastDirection = QMazeOutputDirection.NotSpecified;
			QVector2Int currentPosition = new QVector2Int(startGenerationPoint.x, startGenerationPoint.y);
			
			QMazeOutput output = new QMazeOutput();
			mazeArray[currentPosition.x][currentPosition.y].outputs = new List<QMazeOutput>();
			mazeArray[currentPosition.x][currentPosition.y].outputs.Add(output);
			
			path.Add(new QVector2Int(currentPosition.x, currentPosition.y));

			checkTaskList.Clear();
			if (startPositionList.Count  > 0) checkTaskList.Add(checkStartPoint);
            if (finishPositionList.Count > 0) checkTaskList.Add(checkFinishPoint);
			checkTaskList.Add(checkStandard);
			if (piecePack.getPiece(QMazePieceType.Intersection		  ).use) checkTaskList.Add(checkUnder);
			if (piecePack.getPiece(QMazePieceType.Crossing			  ).use && !onlyPath) checkTaskList.Add(checkCrossing);
			if (piecePack.getPiece(QMazePieceType.Triple			  ).use && !onlyPath) checkTaskList.Add(checkTripple);
			if (piecePack.getPiece(QMazePieceType.DoubleCorner		  ).use) checkTaskList.Add(checkDoubleCorner);
			if (piecePack.getPiece(QMazePieceType.DeadlockCorner	  ).use) checkTaskList.Add(checkDeadlockCorner);
			if (piecePack.getPiece(QMazePieceType.DeadlockLine	      ).use) checkTaskList.Add(checkDeadlockLine);
			if (piecePack.getPiece(QMazePieceType.DeadlockTriple	  ).use) checkTaskList.Add(checkDeadlockTriple);
			if (piecePack.getPiece(QMazePieceType.DeadlockCrossing	  ).use) checkTaskList.Add(checkDeadlockCrossing);
			if (piecePack.getPiece(QMazePieceType.TripleDeadlock	  ).use) checkTaskList.Add(checkTripleDeadlock);
			if (piecePack.getPiece(QMazePieceType.LineDeadlock		  ).use) checkTaskList.Add(checkLineDeadlock);
			if (piecePack.getPiece(QMazePieceType.LineDeadlockLine	  ).use) checkTaskList.Add(checkLineDeadlockLine);
			if (piecePack.getPiece(QMazePieceType.CornerDeadlockLeft	  ).use) checkTaskList.Add(checkCornerDeadlock1);
			if (piecePack.getPiece(QMazePieceType.CornerDeadlockRight	  ).use) checkTaskList.Add(checkCornerDeadlock2);
			if (piecePack.getPiece(QMazePieceType.CornerDeadlockCorner).use) checkTaskList.Add(checkCornerDeadlockCorner);
			if (piecePack.getPiece(QMazePieceType.None				  ).use) checkTaskList.Add(checkNone);

			float time = Time.realtimeSinceStartup;

			do
			{
				int lastPathIndex = path.Count - 1;
				currentPosition.set(path[lastPathIndex].x, path[lastPathIndex].y);             
				
				lastDirection = QMazeOutputDirection.NotSpecified;
				QMazeOutput outputArray = QMazeOutput.getShuffleOutput();

				foreach (QMazeOutputDirection dir in outputArray.outputDirList)
				{
					QVector2Int newPosition = new QVector2Int(currentPosition.x + QMazeOutput.dx[dir], currentPosition.y + QMazeOutput.dy[dir]);
					if (pointInMaze(newPosition))
					{
						if (mazeArray[currentPosition.x][currentPosition.y].outputs.Count == 1)
						{
							List<QMazeOutput> newPositionOutputs = mazeArray[newPosition.x][newPosition.y].outputs;

							int checkResult = 0;
							for (int i = 0; i < checkTaskList.Count; i++)
							{
	                            CheckTask checkTask = checkTaskList[i];
								checkResult = checkTask(currentPosition, newPosition, newPositionOutputs, dir);
								if (checkResult != CHECK_FAILED) break;
							}

							QVector2IntDir exit = QListUtil.get(exitPositionList, currentPosition);
							if (exit != null)
							{
								if (!mazeArray[currentPosition.x][currentPosition.y].outputs[0].outputDirList.Contains(exit.direction))
									mazeArray[currentPosition.x][currentPosition.y].outputs[0].outputDirList.Add(exit.direction);
							}

							if (checkResult == CHECK_CONTINUE) continue;
							if (checkResult == CHECK_BREAK) 
							{
								generationProgress++;
								break;
							}
						}
					}
				}
				
				if (lastDirection == QMazeOutputDirection.NotSpecified)            
					path.RemoveAt(path.Count - 1);   

				if (asynchronous && Time.realtimeSinceStartup - time > maxTime)
				{
					time = Time.realtimeSinceStartup;
					yield return null;
					if (mazeGenerateProgressEvent != null)
						mazeGenerateProgressEvent.Invoke(getGenerationProgress());
				} 
			}
			while (path.Count > 0);		

			List<QMazePiece> pieces = piecePack.getMazePieceList();
			for (int i = 0; i < pieces.Count; i++)
			{
				if ((!pieces[i].use && !pieces[i].isRequire()) || 
				    pieces[i].getType() == QMazePieceType.Start || 
				    pieces[i].getType() == QMazePieceType.Finish)
				{
					pieces.RemoveAt(i);
					i--;
				}
			}

			float count = 0;
			instantiatingProgress = 0;
			bool wasError = false;
			float mazeSize = mazeWidth * mazeHeight;

			for (int ix = 0; ix < mazeWidth; ix++) 
			{          
				for (int iy = 0; iy < mazeHeight; iy++) 
				{
					QMazePieceData mazePieceData = mazeArray[ix][iy];
					
					QMazePiece targetPiece = null;
					
					if (QListUtil.has(startPositionList, ix, iy) && mazePieceData.outputs != null && piecePack.getPiece(QMazePieceType.Start).checkFit(mazePieceData.outputs))
					{
						targetPiece = piecePack.getPiece(QMazePieceType.Start);
					}
					else if (QListUtil.has(finishPositionList, ix, iy) && mazePieceData.outputs != null && piecePack.getPiece(QMazePieceType.Finish).checkFit(mazePieceData.outputs))
					{
						targetPiece = piecePack.getPiece(QMazePieceType.Finish);
					}
					else
					{
						QListUtil.Shuffle<QMazePiece>(pieces);
						for (int i = 0; i < pieces.Count; i++)
						{
							if (pieces[i].checkFit(mazePieceData.outputs))
							{
								targetPiece = pieces[i];
								break;
							}
						} 
					}
					
					if (targetPiece == null)
					{
						if (pointInMaze(new QVector2Int(ix, iy)) || obstacleIsNone)
						{
							targetPiece = piecePack.getPiece(QMazePieceType.None);
						}
						else
						{
							continue;
						}
					}
					else if (targetPiece.geometryList.Count == 0)
					{
						if (pointInMaze(new QVector2Int(ix, iy)))
						{
							if (!wasError)
							{
								wasError = true;
								Debug.LogWarning("QMaze: Geometry for " + targetPiece.getType() + " piece is not found. Please check that geometry is specified for it in the piece pack.");		
							}
						}
						continue;
					}

					mazePieceData.type = targetPiece.getType();
					mazePieceData.rotation = -targetPiece.getRotation();

					if (generateWithGeometry) generateGeometry(mazePieceData);	

					count++;
					instantiatingProgress = count / mazeSize;

					if (mazePieceGeneratedEvent != null)
						mazePieceGeneratedEvent.Invoke(mazePieceData);						
					
					if (Time.realtimeSinceStartup - time > maxTime)
					{
						time = Time.realtimeSinceStartup;
						yield return null;
						if (mazeGenerateProgressEvent != null)
							mazeGenerateProgressEvent.Invoke(getGenerationProgress());
					}
				}
			}

			if (mazeGeneratedEvent != null)
				mazeGeneratedEvent.Invoke(this);
		}

		private void generateGeometry(QMazePieceData pieceData)
		{
			QMazePiece targetPiece = piecePack.getPiece(pieceData.type);
			
			GameObject prefab = targetPiece.geometryList[QMath.getRandom(0, targetPiece.geometryList.Count)];
			GameObject go;
			#if UNITY_EDITOR
			if (Application.isPlaying)
			{
				go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(pieceData.rotation, Vector3.up));
			}
			else
			{
				PrefabType type = PrefabUtility.GetPrefabType(prefab);
				if (type == PrefabType.Prefab)
				{
					go = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				}
				else
				{
					go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(pieceData.rotation, Vector3.up));
				}
			}	
			#else
				go = (GameObject)GameObject.Instantiate(prefab, new Vector3(), Quaternion.AngleAxis(-targetPiece.getRotation(), Vector3.up));
			#endif
			go.transform.parent = transform;
			go.transform.localPosition = new Vector3(pieceData.x * mazePieceWidth * mazeScale, 0, -pieceData.y * mazePieceHeight * mazeScale);
			go.transform.rotation = transform.rotation * Quaternion.AngleAxis(pieceData.rotation, Vector3.up);
			
			Vector3 scale = go.transform.localScale;
			go.transform.localScale = scale * mazeScale;
			
			pieceData.geometry = go;
		}
		
        private void generateRandomPoints(List<QVector2IntDir> pointList, int randomCount)
        {
            for (int i = 0; i < randomCount; i++)
            {
                QVector2IntDir newPoint = new QVector2IntDir(QMath.getRandom(0, mazeWidth), QMath.getRandom(0, mazeHeight), QMazeOutputDirection.NotSpecified);
                while (!pointInMaze(newPoint) || QListUtil.has(startPositionList, newPoint) || QListUtil.has(finishPositionList, newPoint) || QListUtil.has(exitPositionList, newPoint))
                {
                    newPoint.x = QMath.getRandom(0, mazeWidth);
                    newPoint.y = QMath.getRandom(0, mazeHeight);
                }               
                pointList.Add(newPoint);
            }
        }

		private int checkStartPoint(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QListUtil.has(startPositionList, newPosition)) 
			{
				if (mazeArray[newPosition.x][newPosition.y].outputs == null)
				{
					QVector2IntDir startPoint = QListUtil.get(startPositionList, new QVector2IntDir(newPosition.x, newPosition.y, QMazeOutput.opposite[dir]));
					if (startPoint != null)
					{
						QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
						output.outputDirList.Add(dir);
						
						output = new QMazeOutput();
						output.outputDirList.Add(QMazeOutput.opposite[dir]);
						mazeArray[newPosition.x][newPosition.y].outputs = new List<QMazeOutput>();
						mazeArray[newPosition.x][newPosition.y].outputs.Add(output);

						if (startPoint.direction == QMazeOutputDirection.NotSpecified)
							startPoint.direction = QMazeOutput.opposite[dir];
						startFinishLeft--;
					}
					else
					{
						return CHECK_CONTINUE;
					}
				}
				return CHECK_CONTINUE;
			}
			else if (QListUtil.has(startPositionList, currentPosition)) 
			{
				QVector2IntDir startPoint = QListUtil.get(startPositionList, new QVector2IntDir(currentPosition.x, currentPosition.y, QMazeOutput.opposite[dir]));
				if (startPoint != null && startPoint.direction == QMazeOutputDirection.NotSpecified)
				    startPoint.direction = QMazeOutput.opposite[dir];				
				return CHECK_BREAK;
			}
			return CHECK_FAILED;
		}

        private int checkFinishPoint(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
        {
            if (QListUtil.has(finishPositionList, newPosition)) 
            {
                if (mazeArray[newPosition.x][newPosition.y].outputs == null)
                {
                    QVector2IntDir finishPoint = QListUtil.get(finishPositionList, new QVector2IntDir(newPosition.x, newPosition.y, QMazeOutput.opposite[dir]));
                    if (finishPoint != null)
                    {
                        QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
                        output.outputDirList.Add(dir);
                        
                        output = new QMazeOutput();
                        output.outputDirList.Add(QMazeOutput.opposite[dir]);
                        mazeArray[newPosition.x][newPosition.y].outputs = new List<QMazeOutput>();
                        mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
                        
                        if (finishPoint.direction == QMazeOutputDirection.NotSpecified)
                            finishPoint.direction = QMazeOutput.opposite[dir];
                        startFinishLeft--;
                    }
                    else
                    {
                        return CHECK_CONTINUE;
                    }
                }
                return CHECK_CONTINUE;
            }
            else if (QListUtil.has(finishPositionList, currentPosition)) 
            {
                QVector2IntDir finishPoint = QListUtil.get(finishPositionList, new QVector2IntDir(currentPosition.x, currentPosition.y, QMazeOutput.opposite[dir]));
                if (finishPoint != null && finishPoint.direction == QMazeOutputDirection.NotSpecified)
                    finishPoint.direction = QMazeOutput.opposite[dir];                
                return CHECK_BREAK;
            }
            return CHECK_FAILED;
        }

		private int checkStandard(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (mazeArray[newPosition.x][newPosition.y].outputs == null)
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs = new List<QMazeOutput>();
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				path.Add(new QVector2Int(newPosition.x, newPosition.y));
				lastDirection = dir;

				return CHECK_BREAK;
			}
			else
			{
				bool found = false;
				for (int i = 0; i < mazeArray[currentPosition.x][currentPosition.y].outputs.Count; i++)
				{
					QMazeOutput mazeOutput = mazeArray[currentPosition.x][currentPosition.y].outputs[i];
					
					for (int k = 0; k < mazeOutput.outputDirList.Count; k++)
					{
						if (mazeOutput.outputDirList[k] == dir)
						{
							found = true;
							break;
						}
					}
				}

				if (!found)
				{
					for (int i = 0; i < mazeArray[newPosition.x][newPosition.y].outputs.Count; i++)
					{
						QMazeOutput mazeOutput = mazeArray[newPosition.x][newPosition.y].outputs[i];

						for (int k = 0; k < mazeOutput.outputDirList.Count; k++)
						{
							if (mazeOutput.outputDirList[k] == QMazeOutput.opposite[dir])
							{
								QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[0];
								output.outputDirList.Add(dir);
							}
						}
					}
				}
			}
			return CHECK_FAILED;
		}

		private int checkUnder(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
	    {
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.Intersection).frequency &&
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 &&
			    !newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QVector2Int newPosition2 = newPosition.clone();
				newPosition2.x += QMazeOutput.dx[dir];
				newPosition2.y += QMazeOutput.dy[dir];

				if (pointInMaze(newPosition2) && 
				    mazeArray[newPosition2.x][newPosition2.y].outputs == null &&
				    !QListUtil.has(startPositionList, newPosition2) && 
				    !QListUtil.has(finishPositionList, newPosition2)) 
	            {
					QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
					output.outputDirList.Add(dir);
					
					output = new QMazeOutput();
					output.outputDirList.Add(dir);
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					mazeArray[newPosition.x][newPosition.y].outputs.Add(output);							
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					mazeArray[newPosition2.x][newPosition2.y].outputs = new List<QMazeOutput>();
					mazeArray[newPosition2.x][newPosition2.y].outputs.Add(output);
					
					path.Add(new QVector2Int(newPosition2.x, newPosition2.y));
					lastDirection = dir;
					
					return CHECK_BREAK;
	            }		        
			}
			return CHECK_FAILED;
	    }

		private int checkCrossing(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.Crossing).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 &&
			    !newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QVector2Int newPosition2 = newPosition.clone();
				newPosition2.x += QMazeOutput.dx[dir];
				newPosition2.y += QMazeOutput.dy[dir];

				if (pointInMaze(newPosition2) && 
				    mazeArray[newPosition2.x][newPosition2.y].outputs == null)
				{
					QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
					output.outputDirList.Add(dir);

					mazeArray[newPosition.x][newPosition.y].outputs[0].outputDirList.Add(dir);
					mazeArray[newPosition.x][newPosition.y].outputs[0].outputDirList.Add(QMazeOutput.opposite[dir]);
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					mazeArray[newPosition2.x][newPosition2.y].outputs = new List<QMazeOutput>();
					mazeArray[newPosition2.x][newPosition2.y].outputs.Add(output);
					
					path.Add(new QVector2Int(newPosition2.x, newPosition2.y));
					lastDirection = dir;
					
					return CHECK_BREAK;					
				}
			}
			return CHECK_FAILED;
		}

		private int checkTripple(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.Triple).frequency && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);

				newPositionOutputs[newPositionOutputs.Count - 1].outputDirList.Add(QMazeOutput.opposite[dir]);

				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDoubleCorner(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
	    {
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DoubleCorner).frequency && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
	        {
	            QVector2Int newPos1 = new QVector2Int(newPosition.x + QMazeOutput.dx[QMazeOutput.rotateCW[dir]],
	                                                  newPosition.y + QMazeOutput.dy[QMazeOutput.rotateCW[dir]]);
	            QVector2Int newPos2 = new QVector2Int(newPosition.x + QMazeOutput.dx[QMazeOutput.rotateCCW[dir]],
	                                                  newPosition.y + QMazeOutput.dy[QMazeOutput.rotateCCW[dir]]);

				if ((pointInMaze(newPos1) && 
				     mazeArray[newPos1.x][newPos1.y].outputs == null && 
				     newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCCW[dir]) && 
				     !QListUtil.has(startPositionList, newPos1) && 
				     !QListUtil.has(finishPositionList, newPos1)) 
				    ||
				    (pointInMaze(newPos2) && 
				 	mazeArray[newPos2.x][newPos2.y].outputs == null && 
				 	newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCW [dir]) &&
				    !QListUtil.has(startPositionList, newPos2) && 
				    !QListUtil.has(finishPositionList, newPos2)))
	            {
					QMazeOutputDirection dir2 = dir;
					
					QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
					output.outputDirList.Add(dir);
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir]);
					if (!mazeArray[newPosition.x][newPosition.y].outputs[0].outputDirList.Contains(QMazeOutput.rotateCW[dir]))
					{
						output.outputDirList.Add(QMazeOutput.rotateCW[dir]);
						dir2 = QMazeOutput.rotateCW[dir];
					}
					else 
					{
						output.outputDirList.Add(QMazeOutput.rotateCCW[dir]);
						dir2 = QMazeOutput.rotateCCW[dir];
					}
					mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
					
					newPosition.x += QMazeOutput.dx[dir2];
					newPosition.y += QMazeOutput.dy[dir2];
					
					output = new QMazeOutput();
					output.outputDirList.Add(QMazeOutput.opposite[dir2]);
					mazeArray[newPosition.x][newPosition.y].outputs = new List<QMazeOutput>();
					mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
					
					path.Add(new QVector2Int(newPosition.x, newPosition.y));
					lastDirection = dir2;
					
					return CHECK_BREAK;
	            }
	        }
			return CHECK_FAILED;
	    }

		private int checkDeadlockCorner(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{		
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockCorner).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 1 &&
			    (newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCW[dir]) || newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCCW[dir])))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDeadlockLine(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockLine).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Contains(dir))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDeadlockTriple(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockTriple).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 2 &&
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
			    newPositionOutputs[1].outputDirList.Count == 1 &&
			    !newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkDeadlockCrossing(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.DeadlockCrossing).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 3 &&
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
			    newPositionOutputs[1].outputDirList.Count == 1 && 
			    !newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
			    newPositionOutputs[2].outputDirList.Count == 1 &&
			    !newPositionOutputs[2].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkTripleDeadlock(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.TripleDeadlock).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 3 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkLineDeadlock(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.LineDeadlock).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    !newPositionOutputs[0].outputDirList.Contains(dir) && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;
			}
			return CHECK_FAILED;
		}

		private int checkLineDeadlockLine(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.LineDeadlockLine).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 2 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
				!newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]) && 
				((newPositionOutputs[0].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList.Count == 1 && newPositionOutputs[0].outputDirList[0] == QMazeOutput.opposite[newPositionOutputs[0].outputDirList[1]]) || 
				 (newPositionOutputs[0].outputDirList.Count == 1 && newPositionOutputs[1].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList[0] == QMazeOutput.opposite[newPositionOutputs[1].outputDirList[1]])))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;											
			}
			return CHECK_FAILED;
		}

		private int checkCornerDeadlock1(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.CornerDeadlockLeft).frequency && 
			    newPositionOutputs != null && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCW[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;						
			}
			return CHECK_FAILED;
		}

		private int checkCornerDeadlock2(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.CornerDeadlockRight).frequency && 
			    newPositionOutputs.Count == 1 &&
			    newPositionOutputs[0].outputDirList.Count == 2 && 
			    newPositionOutputs[0].outputDirList.Contains(dir) && 
			    newPositionOutputs[0].outputDirList.Contains(QMazeOutput.rotateCCW[dir]))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;			
			}
			return CHECK_FAILED;
		}

		private int checkCornerDeadlockCorner(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (QMath.getRandom() < piecePack.getPiece(QMazePieceType.CornerDeadlockCorner).frequency && 
			    newPositionOutputs.Count == 2 && 
			    !newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]) && 
				!newPositionOutputs[1].outputDirList.Contains(QMazeOutput.opposite[dir]) &&
				((newPositionOutputs[0].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList.Count == 1 && newPositionOutputs[0].outputDirList[0] != QMazeOutput.opposite[newPositionOutputs[0].outputDirList[1]]) ||
				 (newPositionOutputs[0].outputDirList.Count == 1 && newPositionOutputs[1].outputDirList.Count == 2 && newPositionOutputs[1].outputDirList[0] != QMazeOutput.opposite[newPositionOutputs[1].outputDirList[1]])))
			{
				QMazeOutput output = mazeArray[currentPosition.x][currentPosition.y].outputs[mazeArray[currentPosition.x][currentPosition.y].outputs.Count - 1];
				output.outputDirList.Add(dir);
				
				output = new QMazeOutput();
				output.outputDirList.Add(QMazeOutput.opposite[dir]);
				mazeArray[newPosition.x][newPosition.y].outputs.Add(output);
				
				return CHECK_CONTINUE;											
			}
			return CHECK_FAILED;
		}

		private int checkNone(QVector2Int currentPosition, QVector2Int newPosition, List<QMazeOutput> newPositionOutputs, QMazeOutputDirection dir)
		{
			if (startFinishLeft == 0 &&
				QMath.getRandom() < piecePack.getPiece(QMazePieceType.None).frequency && 
			    newPositionOutputs.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Count == 1 && 
			    newPositionOutputs[0].outputDirList.Contains(QMazeOutput.opposite[dir]))
			{
				newPositionOutputs.Clear();
				newPositionOutputs.Add(new QMazeOutput());

				List<QMazeOutput> currentOutputs = mazeArray[currentPosition.x][currentPosition.y].outputs;
				for (int i = 0; i < currentOutputs.Count; i++)
				{
					currentOutputs[i].outputDirList.Remove(dir);
				}

				return CHECK_BREAK;
			}
			return CHECK_FAILED;
		}
	}
}
