using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractableHandler : MonoBehaviour {

    public event Action<IInteractable> OnInteractableInRange;
    public event Action<IInteractable> OnInteractableOutOfRange;
    public event Action<IInteractable> OnInteractionStarted;

    [SerializeField] protected Camera playerCamera = null;
    [SerializeField] protected float interactionRange = 5f;
    [SerializeField] protected KeyCode interactionKey = KeyCode.E;

    ControllableEntity controllingEntity;

    IInteractable possibleInteractable;
    public IInteractable PossibleInteractable
    {
        get { return possibleInteractable; }
        set
        {
            IInteractable oldVal = possibleInteractable;
            possibleInteractable = value;

            if (possibleInteractable == null && oldVal != null)
            {
                //out of range, but only call once if null, not every frame
                OnInteractableOutOfRange?.Invoke(oldVal);
            }
            else if (possibleInteractable != oldVal)
            {
                OnInteractableInRange?.Invoke(possibleInteractable);
            }
        }
    }

    public Camera PlayerCamera { get { return playerCamera; } }

    protected virtual Ray RaycastRay { get { return playerCamera.ViewportPointToRay(new Vector3(.5f, .5f)); } }

    private void Update()
    {
        PerformRayCast();
        CheckIfTryingToInteract();
    }

    void PerformRayCast()
    {
        RaycastHit h;
        if (Physics.Raycast(RaycastRay, out h, interactionRange))
        {
            //hit something
            PossibleInteractable = h.transform.gameObject.GetComponent<IInteractable>();

        }
        else
            PossibleInteractable = null;
    }

    public void SetControllingEntity(ControllableEntity ent)
    {
        controllingEntity = ent;
    }

    void CheckIfTryingToInteract()
    {
        if(possibleInteractable != null && Input.GetKeyDown(interactionKey))
        {
            possibleInteractable.Interact(controllingEntity);
            OnInteractionStarted?.Invoke(possibleInteractable);
            possibleInteractable = null;
        }
    }
}
