using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ThrowableItem : MonoBehaviour, IHoldeableItem,IInteractable
{
    [Header("path display")]
    [SerializeField] LineRenderer pathDisplay = null;
    [SerializeField] int resolution = 30;
    [Header("Throw Related")]
    [SerializeField] Rigidbody body = null;
    public Rigidbody Rigidbody { get { return body; } set { body = value; } }
    [SerializeField] ForceMode forceMode = ForceMode.Force;
    [SerializeField] float defaultForce = 5f;
    [SerializeField] float chargeSpeed = 1f;
    float currentForce;

    public bool IsThrowable
    {
        get
        {
            return true;
        }
    }

    public GameObject GameObject
    {
        get
        {
            return gameObject;
        }
    }

    [SerializeField] string interactionSpriteResourceLocator = "";
    public string SpriteResourceLocator { get { return interactionSpriteResourceLocator; } }

    ItemHolder currentHolder;

    private void Start()
    {
        pathDisplay.positionCount = resolution+1;
    }

    public void DropHeldItem()
    {
        body = gameObject.AddComponent<Rigidbody>();
        currentHolder = null;
        //mayeb scale up
    }

    public void OnPlacedInItemHolder(ItemHolder holder)
    {
        currentHolder = holder;

        transform.SetParent(holder.transform);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler((Vector3.up + Vector3.forward) * 90);
        Destroy(body);
        //maybe scale down
        //set rotation so that dot product is zero
    }

    public void UpdateRoutineWhenHeld()
    {
        //throw in correct direction;
        DrawPath();
        if (currentHolder.UseKeyDown())
            StartThrow();
        if (currentHolder.UseKeyHeld())
            ChargeThrow();
        if (currentHolder.UseKeyUp())
            Throw();
    }

    void StartThrow()
    {
        currentForce = defaultForce;
        pathDisplay.enabled = true;

    }

    void ChargeThrow()
    {
        currentForce += (chargeSpeed * Time.deltaTime);
    }

    void Throw()
    {
        pathDisplay.enabled = false;
        Vector3 forward = Vector3.Lerp(currentHolder.ParentTransform.forward,currentHolder.transform.up,.5f);
        transform.rotation = currentHolder.ParentTransform.rotation * Quaternion.Euler(Vector3.forward * 90f);
        currentHolder.DropItem();

        body.AddForce(forward * currentForce, forceMode);
    }

    void DrawPath()
    {
        Vector3 forward = Vector3.Lerp(currentHolder.ParentTransform.forward, currentHolder.transform.up, .5f);

        pathDisplay.SetPosition(0, transform.position);
        for (int i = 1; i <= resolution; i++)
        {
            float simulationTime = i / (float)resolution;
            Vector3 displacement = forward *currentForce * simulationTime + Vector3.up * Physics.gravity.y * simulationTime * simulationTime / 2f;
            Vector3 drawPoint = transform.position + displacement;
            pathDisplay.SetPosition(i,drawPoint);
        }
    }

    public void Interact(ControllableEntity activator)
    {
        //ayy lmao
        activator.HoldItem(this);
    }

    public void AddLineRenderer()
    {
        pathDisplay = gameObject.AddComponent<LineRenderer>();
    }
}
