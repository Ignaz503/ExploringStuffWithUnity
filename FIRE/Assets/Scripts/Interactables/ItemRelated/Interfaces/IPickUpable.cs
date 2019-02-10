using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPickUpable : IInteractable, IHoldeableItem
{
    void OnPickUp(GameObject activator);
    void OnDrop();
}
