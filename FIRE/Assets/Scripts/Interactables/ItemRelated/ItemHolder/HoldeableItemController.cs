using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldeableItemController : MonoBehaviour
{
    [SerializeField] List<ItemHolder> itemHolders = null;

    [SerializeField] Transform realParentTransform = null;

    [SerializeField] PlayerInteractableHandler interactableHandler = null;

    private void Start()
    {
#if UNITY_EDITOR
        if (interactableHandler == null)
            Debug.Log("no interaction handler, is not importatn msg only shown to remove warning in console log");
#endif
        foreach(ItemHolder t in itemHolders)
        {
            t.SetParentTransform(realParentTransform);
            t.SetPlayerCamera(interactableHandler.PlayerCamera);
        }
    }

    public void TryHoldItem(IHoldeableItem holdeableItem)
    {
        if (holdeableItem != null)
        {
            foreach (ItemHolder h in itemHolders)
            {
                if (!h.HasItem)
                {
                    h.HoldItem(holdeableItem);
                    break;
                }
            }
        }
    }
}
