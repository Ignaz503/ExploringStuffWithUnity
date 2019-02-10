using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EditorGraphStorage<T,C> : SaveableOutsideOfEditorFolder where T : EditorGraphNode where C : Connection
{
    public abstract void StoreGraph(List<T> nodes, List<C> connections);   
}

