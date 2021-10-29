// Copyright © 2018 – Property of Tobii AB (publ) - All Rights Reserved

using UnityEditor;

namespace Tobii.XR
{
    public interface IEditorSettings
    {
        void SetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup, string defines);
        string GetScriptingDefineSymbolsForGroup(BuildTargetGroup targetGroup);
    }
}