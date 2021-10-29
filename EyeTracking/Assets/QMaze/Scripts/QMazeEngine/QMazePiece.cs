using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

namespace qtools.qmaze
{
	/// <summary>
	/// Type of the maze piece
	/// </summary>
	[System.Serializable]
	public enum QMazePieceType
	{
		None = 0, 
		Line = 1,
		Deadlock = 2,
		Triple = 3,
		Corner = 4,
		Crossing = 5,
		Start = 6,
		Finish = 7,
		DoubleCorner = 8,
		Intersection = 9,
		DeadlockCorner = 10,
		DeadlockLine = 11,
		DeadlockTriple = 12,
		DeadlockCrossing = 13,
		TripleDeadlock = 14,
		LineDeadlock = 15,
		LineDeadlockLine = 16,
		CornerDeadlockLeft = 17,
		CornerDeadlockRight = 18,
		CornerDeadlockCorner = 19
	}

	/// <summary>
	/// Describes the maze piece for generation
	/// </summary>
	[System.Serializable]
	public class QMazePiece
	{
		[SerializeField] private QMazePieceType type;	
		[SerializeField] private bool require;

		/// <summary>
		/// Getting the piece type
		/// </summary>
		/// <returns>Type of the maze piece</returns>
		public QMazePieceType getType()
		{
			return type;
		}

		/// <summary>
		/// Indicates whether the piece is required
		/// </summary>
		/// <returns><c>true</c>, if the piece is required, <c>false</c> otherwise.</returns>
		public bool isRequire()
		{
			return require;
		}

		/// <summary>
		/// Flag indicating whether this piece should be used for maze generation. 
		/// Required blocks will be used even when the flag is disabled, but only in the places where it is needed.
		/// </summary> 
		public bool use = false;

		/// <summary>
		/// Frequency of the piece occurrence in the maze
		/// </summary>
		public float frequency = 0.05f;

		/// <summary>
		/// List of game objects that represent the geometry for this maze piece
		/// </summary>
		public List<GameObject> geometryList = new List<GameObject>();

		/// <summary>
		/// List of all the passages in the piece
		/// </summary>
		public List<QMazeOutput> outputList;	

		[SerializeField] private float rotation;	

		// CONSTRUCTOR
		public QMazePiece(QMazePieceType type, bool require, bool use, float frequency, List<QMazeOutput> outputList)
		{
			this.type = type;
			this.require = require;
			this.use = use;
			this.frequency = frequency;
			this.outputList = outputList;
		}

		/// <summary>
		/// Verifies that the passages into parameters coincide with passages in the piece.
		/// </summary>
		/// <returns><c>true</c>, if the piece corresponds to the passages, <c>false</c> otherwise.</returns>
		/// <param name="sourceOutputs">List of passages</param>
		public bool checkFit(List<QMazeOutput> sourceOutputs)
		{
			if (sourceOutputs == null)
			{
				if (outputList.Count > 0) return false;
				else return true;
			}

			rotation = 0;
			for (int i = 0; i < 4; i++)
			{
				if (check(sourceOutputs)) return true;
				rotation += 90;
				rotate(sourceOutputs);
			}
			return false;
		}

		/// <summary>
		/// Getting the current rotation angle of the maze piece
		/// </summary>
		/// <returns>Current rotation angle of the maze piece</returns>
		public float getRotation()		
		{
			return rotation;
		}

	    private void rotate(List<QMazeOutput> sourceOutputs)
	    {
	        int sourceOutputsCount = sourceOutputs.Count;
	        for (int i = 0; i < sourceOutputsCount; i++)
	        {
	            QMazeOutput sourceOutput = sourceOutputs[i];
	            List<QMazeOutputDirection> directions = sourceOutput.outputDirList;
	            int directionCount = directions.Count;
	            for (int j = 0; j < directionCount; j++)
	            {
					directions[j] = QMazeOutput.rotateCW[directions[j]];
	            }
	        }
	    }

	    private bool check(List<QMazeOutput> sourceOutputs)
	    {
			if (outputList.Count != sourceOutputs.Count) return false;

	        int found = 0;
	        int outputCount = outputList.Count;
	        int sourceOutputsCount = sourceOutputs.Count;
	        for (int oi = 0; oi < outputCount; oi++)
	        {
	            List<QMazeOutputDirection> outputDirections = outputList[oi].outputDirList;
	            for (int si = 0; si < sourceOutputsCount; si++)
	            {
	                List<QMazeOutputDirection> sourceOutputDirections = sourceOutputs[si].outputDirList;
	                if (outputDirections.Count == sourceOutputDirections.Count)
	                {
	                    int contains = 0;
	                    int outputDirectionsCount = outputDirections.Count;
	                    for (int di = 0; di < outputDirectionsCount; di++)
	                    {
	                        if (outputDirections.Contains(sourceOutputDirections[di])) contains++;
	                    }
	                    if (contains == outputDirectionsCount) found++;
	                }
	            }
	        }

	        if (found == outputList.Count) return true;
	        else return false;
	    }
	}
}