using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCrosshair : MonoBehaviour {

    [SerializeField] Image crosshair = null;
    [SerializeField] PlayerInteractableHandler interactionController = null;

    Dictionary<string, Sprite> spriteResources;
    [SerializeField] Sprite defaultSprite = null;
    [SerializeField] Sprite unkownSprite = null;

    private void Start()
    {

        spriteResources = new Dictionary<string, Sprite>();

        interactionController.OnInteractableInRange += (item) => ChangeSprite(item); 

        interactionController.OnInteractionStarted += (a) => SetToDefault();

        interactionController.OnInteractableOutOfRange += (a) => SetToDefault();

    }

    private void SetToDefault()
    {
        ApplySprite(defaultSprite);
    }

    void ChangeSprite(IInteractable interactable)
    {
        Sprite s;
        if (spriteResources.ContainsKey(interactable.SpriteResourceLocator))
        {
           s= spriteResources[interactable.SpriteResourceLocator];
        }
        else
        {
            //try load
            s = Resources.Load<Sprite>(interactable.SpriteResourceLocator);

            if (s != null)
                spriteResources.Add(interactable.SpriteResourceLocator, s);
            else
                s = unkownSprite;
        }

        ApplySprite(s);
    }

    void ApplySprite(Sprite s)
    {
        crosshair.sprite = s;
    }

}
