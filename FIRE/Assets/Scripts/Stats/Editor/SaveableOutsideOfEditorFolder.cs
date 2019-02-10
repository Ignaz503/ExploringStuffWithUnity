using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SaveableOutsideOfEditorFolder : ScriptableObject
{
    public string CreateUseableAssetPath = "Assets/Resources/";
    public string CreateUseableAssetName = "StatInfluenceGraph";

    public abstract void CreateUseableDataOutsideOfEditor();
    public abstract void ClearData();
}
