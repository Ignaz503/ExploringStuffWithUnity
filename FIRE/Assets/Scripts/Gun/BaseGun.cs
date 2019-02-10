using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseGun : MonoBehaviour, IHoldeableItem, IInteractable
{
    public abstract bool IsThrowable { get; }
    public GameObject GameObject { get { return gameObject; } }
    [SerializeField] string spriteResouceLocator = "";
    public string SpriteResourceLocator { get{ return spriteResouceLocator; } }

    [SerializeField]protected KeyCode fireKey = KeyCode.Mouse0;

    protected abstract bool canFire { get; }

    public abstract void DropHeldItem();

    public virtual void Interact(ControllableEntity Activator)
    {
        Activator.HoldItem(this);
    }

    public abstract void OnPlacedInItemHolder(ItemHolder holder);

    public abstract void UpdateRoutineWhenHeld();

    protected abstract void Fire();
}
