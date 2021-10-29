using UnityEngine;
using System.Collections.Generic;

namespace qtools.qmaze
{
	/// <summary>
	/// Class describes the piece of the generated maze
	/// </summary>
	public class QMazePieceData
	{
		/// <summary>
		/// Piece position on the X axis
		/// </summary>
		public int x;

		/// <summary>
		/// Piece position on the Y axis
		/// </summary>
		public int y;

		/// <summary>
		/// List of passages in the piece
		/// </summary>
		public List<QMazeOutput> outputs;

		/// <summary>
		/// Game object that describes the geometry of this piece.
		/// If the maze was generated without geometry, the value will be null
		/// </summary>
		public GameObject geometry;

		/// <summary>
		/// Type of the maze piece
		/// </summary>
		public QMazePieceType type;

		/// <summary>
		/// Rotation of the maze piece from the initial rotation.
		/// </summary>
		public float rotation;
		
		public QMazePieceData(int x, int y)
		{
			this.x = x;
			this.y = y;
			type = QMazePieceType.None;
		}
	}
}