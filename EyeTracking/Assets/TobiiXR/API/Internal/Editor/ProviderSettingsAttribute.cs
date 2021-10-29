// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using System;

namespace Tobii.XR
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ProviderSettingsAttribute : System.Attribute
    {
        public Type Type { get; private set; }

        public ProviderSettingsAttribute(Type type)
        {
            Type = type;
        }
    }
}