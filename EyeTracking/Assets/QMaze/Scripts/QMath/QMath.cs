using System;

namespace qtools.qmaze
{
	public class QMath
	{
		private static Random random = new Random();
		
		public static int getRandom(int min, int max)
		{
			return random.Next(min, max);
		}

		public static float getRandom()
		{
			return (float)random.NextDouble();
		}

		public static void setSeed(int seed)
		{
			random = new System.Random(seed);
		}
	}
}