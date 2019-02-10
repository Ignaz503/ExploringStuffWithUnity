using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllableEntity : MonoBehaviour
{
    [SerializeField] HoldeableItemController holdController = null;
    [SerializeField] PlayerInteractableHandler interactionHandler = null;
    public PlayerInteractableHandler PlayerInteractableHandler { get { return interactionHandler; } }
    public Camera PlayerCamera { get { return interactionHandler.PlayerCamera; } }

    private void Awake()
    {
        interactionHandler.SetControllingEntity(this);
    }

    public void HoldItem(IHoldeableItem holdeableItem)
    {
        holdController.TryHoldItem(holdeableItem);
    }

}
