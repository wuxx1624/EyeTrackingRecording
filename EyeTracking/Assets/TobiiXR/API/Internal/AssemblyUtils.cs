namespace Tobii.XR
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;

    public static class AssemblyUtils 
    {
        public static Type EyetrackingProviderType(string typeName)
        {
            return EyetrackingProviderTypes().Where(t => t.FullName == typeName).FirstOrDefault();
        }

        internal static IEnumerable<Type> EyetrackingProviderTypes()
        {
            var type = typeof(IEyeTrackingProvider);
            var types = (AppDomain.CurrentDomain.GetAssemblies().SelectMany(s => s.GetTypes()).Where(p => type.IsAssignableFrom(p) && p.IsClass));
            return types;
        }
    }
}