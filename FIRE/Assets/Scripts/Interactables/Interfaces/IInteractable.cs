using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    string SpriteResourceLocator { get; }
    GameObject GameObject { get; }
    void Interact(ControllableEntity Activator);// should probably take a player argument or something
}
