// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using System;

namespace Tobii.XR
{
	
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class CompilerFlagAttribute : Attribute
	{
		public readonly string Flag;

		public CompilerFlagAttribute(string flag)
		{
            Flag = flag;
        }
    }
}