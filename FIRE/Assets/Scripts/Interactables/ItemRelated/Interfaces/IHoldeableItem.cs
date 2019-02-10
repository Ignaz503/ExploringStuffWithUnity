using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHoldeableItem
{
    bool IsThrowable { get; }
    GameObject GameObject { get; }
    void OnPlacedInItemHolder(ItemHolder holder);//place in correct position
    void DropHeldItem();//cleanup when dropped
    void UpdateRoutineWhenHeld();
}
