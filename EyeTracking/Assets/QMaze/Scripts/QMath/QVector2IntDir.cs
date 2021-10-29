using UnityEngine;
using System.Collections;

namespace qtools.qmaze
{
	[System.Serializable]
	public class QVector2IntDir: QVector2Int 
	{
		public QMazeOutputDirection direction;

		public QVector2IntDir(int x, int y, QMazeOutputDirection direction): base(x, y)
		{
			this.direction = direction;
		}
		
		public override bool equal(QVector2Int otherPoint)
		{
			return x == otherPoint.x && y == otherPoint.y;
		}

		public bool equal(QVector2IntDir otherPoint)
		{
			return x == otherPoint.x && y == otherPoint.y && (direction == otherPoint.direction || direction == QMazeOutputDirection.NotSpecified || otherPoint.direction == QMazeOutputDirection.NotSpecified);
		}
		
		public new QVector2IntDir clone()
		{
			return new QVector2IntDir(x, y, direction); 
		}
		
		public void set(int nx, int ny, QMazeOutputDirection direction)
		{
			this.x = nx;
			this.y = ny;
			this.direction = direction;
		}
	}
}