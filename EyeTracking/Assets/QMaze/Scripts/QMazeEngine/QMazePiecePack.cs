using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace qtools.qmaze
{
	/// <summary>
	/// Contains the piece pack of all types used to build the maze
	/// </summary>
	[System.Serializable]
	[ExecuteInEditMode]
	public class QMazePiecePack: MonoBehaviour
	{
		[SerializeField] private QMazePiece[] mazePieceArray = new QMazePiece[20];
		[SerializeField] private bool inited = false;	
		[SerializeField] private bool inited2 = false;	
		[SerializeField] private List<GameObject> dragAndDropPieceGeometryArray = new List<GameObject>();

		private void Awake()
		{
			if (!inited)
			{
				inited = true;			 
				dragAndDropPieceGeometryArray.Clear ();

				addPiece(QMazePieceType.None, true, false, 0.05f, new QMazeOutput());
				addPiece(QMazePieceType.Line, true, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Deadlock, true, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }));
				addPiece(QMazePieceType.Triple, true, false, 0.05f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.W, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Corner, true, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Crossing, true, false, 0.05f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.E, QMazeOutputDirection.S, QMazeOutputDirection.W }));
				 
				addPiece(QMazePieceType.Start, false, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));
				addPiece(QMazePieceType.Finish, false, true, 1, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));

				addPiece(QMazePieceType.DoubleCorner, false, false, 0.5f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.W }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }));
				addPiece(QMazePieceType.Intersection, false, false, 0.2f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W, QMazeOutputDirection.E }));

				addPiece(QMazePieceType.DeadlockCorner, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }));
				addPiece(QMazePieceType.DeadlockLine, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }));

				addPiece(QMazePieceType.DeadlockTriple, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }));
				addPiece(QMazePieceType.DeadlockCrossing, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));

				addPiece(QMazePieceType.TripleDeadlock, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));
				addPiece(QMazePieceType.LineDeadlock, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));

				addPiece(QMazePieceType.LineDeadlockLine, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));
				addPiece(QMazePieceType.CornerDeadlockLeft, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }));

				addPiece(QMazePieceType.CornerDeadlockRight, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));
				addPiece(QMazePieceType.CornerDeadlockCorner, false, false, 0.1f, 
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.E, QMazeOutputDirection.S }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.W }),
				         new QMazeOutput(new List<QMazeOutputDirection> { QMazeOutputDirection.N }));
			}

			if (!inited2)
			{
				inited2 = true;

				getPiece(QMazePieceType.None).use = false;
				getPiece(QMazePieceType.None).frequency = 0.05f;

				getPiece(QMazePieceType.Triple).use = false;
				getPiece(QMazePieceType.Triple).frequency = 0.05f;

				getPiece(QMazePieceType.Crossing).use = false;
				getPiece(QMazePieceType.Crossing).frequency = 0.05f;
			}
		}

		/// <summary>
		/// Getting the particular type of piece from this pack
		/// </summary>
		/// <returns>The maze piece</returns>
		/// <param name="type">Type of the maze piece</param>
		public QMazePiece getPiece(QMazePieceType type)
		{ 
			return mazePieceArray[(int)type];
		}

		/// <summary>
		/// Getting a list of all pack pieces
		/// </summary>
		/// <returns>List of all pack pieces</returns>
		public List<QMazePiece> getMazePieceList()
		{
			List<QMazePiece> result = new List<QMazePiece>();
			foreach (QMazePiece piece in mazePieceArray)	
			{
				checkGeometryList(piece);
				result.Add(piece);
			}
			return result;
		}

		private void checkGeometryList(QMazePiece piece)
		{
			List<GameObject> geometryList = piece.geometryList;
			for (int i = 0; i < geometryList.Count; i++)
			{
				if (geometryList[i] == null)				
				{
					geometryList.RemoveAt(i);
					i--;
				}
			}
		}

		private void addPiece(QMazePieceType type, bool require, bool use, float frequency, params QMazeOutput[] mazeOutput)
		{
			List<QMazeOutput> outputs = new List<QMazeOutput>();
			for (int i = 0; i < mazeOutput.Length; i++) outputs.Add(mazeOutput[i]);
			mazePieceArray[(int)type] = new QMazePiece(type, require, use, frequency, outputs);
		}
	}
}
