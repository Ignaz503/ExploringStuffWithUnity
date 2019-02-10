using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnviromentEntity : MonoBehaviour, IInteractable
{
    [SerializeField] protected string interactionSpriteLocator;
    public string SpriteResourceLocator { get { return interactionSpriteLocator; } }

    public GameObject GameObject { get { return gameObject; } }

    public void Interact(ControllableEntity Activator)
    {
        OnSelected(Activator);
        ShowUI(Activator);
    }

    protected void OnSelected(ControllableEntity Activator)
    {
        //inform Selection manager and so on
    }
    protected abstract void ShowUI(ControllableEntity Activator);
}
