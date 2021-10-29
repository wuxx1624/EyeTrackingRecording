
using System;
using System.Collections.Generic;

namespace qtools.qmaze
{
	public class QListUtil
	{
	    public static void Shuffle<T>(List<T> list)
	    {
			int count = list.Count;
			int halfCount = count / 2;
			for(int i = 0; i < halfCount; i++)
			{
				int j = QMath.getRandom(i, count);

				T temp = list[i];
				list[i] = list[j];
				list[j] = temp;
			}
	    }

		public static bool has(List<QVector2Int> list, QVector2Int element)
	    {
			List<QVector2Int>.Enumerator listEnumerator = list.GetEnumerator();
	        while (listEnumerator.MoveNext())	        
	            if (listEnumerator.Current.equal(element))  
					return true;	        
	        return false;
	    }

		public static bool has(List<QVector2IntDir> list, QVector2Int element)
		{
			List<QVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();
			while (listEnumerator.MoveNext())	        
				if (listEnumerator.Current.equal(element))  
					return true;	        
			return false;
		}

		public static bool has(List<QVector2IntDir> list, QVector2IntDir element)
		{
			List<QVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();
			while (listEnumerator.MoveNext())	        
				if (listEnumerator.Current.equal(element))  
					return true;	        
			return false;
		}

		public static QVector2IntDir get(List<QVector2IntDir> list, QVector2IntDir element)
		{
			List<QVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();
			while (listEnumerator.MoveNext())	        
				if (listEnumerator.Current.equal(element))  
					return listEnumerator.Current;	        
			return null;
		}

		public static QVector2IntDir get(List<QVector2IntDir> list, QVector2Int element)
		{
			List<QVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();
			while (listEnumerator.MoveNext())	        
				if (listEnumerator.Current.equal(element))  
					return listEnumerator.Current;	        
			return null;
		}

	    public static bool has(List<QVector2Int> list, int ix, int iy)
	    {
	        List<QVector2Int>.Enumerator listEnumerator = list.GetEnumerator();
	        while (listEnumerator.MoveNext())	        
	            if (listEnumerator.Current.x == ix && listEnumerator.Current.y == iy) 
					return true;	        
	        return false;
	    } 

		public static bool has(List<QVector2IntDir> list, int ix, int iy)
		{
			List<QVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();
			while (listEnumerator.MoveNext())	        
				if (listEnumerator.Current.x == ix && listEnumerator.Current.y == iy) 
					return true;	        
			return false;
		}
	}
}

