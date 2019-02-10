using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseDoor : MonoBehaviour, IInteractable
{
    public enum DoorState
    {
        Open,
        Opening,
        Closed,
        Closing,
        Locked,
    }

    protected event Action<BaseDoor> OnOpen;
    protected event Action<BaseDoor> OnOpening;
    protected event Action<BaseDoor> OnClosed;
    protected event Action<BaseDoor> OnClosing;
    protected event Action<BaseDoor> OnLocked;
    protected event Action<BaseDoor> OnUnlocked;
    protected event Action<ControllableEntity> OnInteract;

    [Header("Interactable related")]
    [SerializeField] string spirteResourceLocator = "";
    public string SpriteResourceLocator
    {
        get
        {
            return spirteResourceLocator;
        }
    }

    protected DoorState previousState;
    [Header("Door related")]
    [SerializeField] DoorState currentState = DoorState.Closed;
    public DoorState CurrentDoorState
    {
        get { return currentState; }
        private set
        {
            previousState = currentState;
            currentState = value;
            switch (currentState)
            {
                case DoorState.Closed:
                    InvokeOnClosed();
                    break;
                case DoorState.Closing:
                    InvokeOnClosing();
                    break;
                case DoorState.Open:
                    InvokeOnOpen();
                    break;
                case DoorState.Opening:
                    InvokeOnOpening();
                    break;
                case DoorState.Locked:
                    InvokeOnLocked();
                    break;
            }
        }
    }

    [Header("Collider Related")]
    [SerializeField] Collider doorCollider = null;
    [SerializeField] bool disableColliderOnMovement = false;

    public GameObject GameObject { get { return gameObject; } }

    Coroutine openRoutine, closeRoutine;

    public void Interact(ControllableEntity activator)
    {
        OnInteract?.Invoke(activator);
        if (IsOpenOrOpening)
            Close();
        else if (IsClosedOrClosing)
            Open();
        //do some feedback
    }

    public void Open()
    {
        if (IsLocked || IsOpenOrOpening)
            return;
        if (IsClosing)
            StopCoroutine(closeRoutine);
        CurrentDoorState = DoorState.Opening;
       StartCoroutine(BaseOpenCoroutine());
    }

    public void Close()
    {
        if (IsClosedOrClosing || IsLocked)
            return;
        if (IsOpening)
            StopCoroutine(openRoutine);
        CurrentDoorState = DoorState.Closing;
       StartCoroutine(BaseCloseCoroutine());
    }

    public void Lock()
    {
        if (!IsClosed)
            return; //do some feedback
        CurrentDoorState = DoorState.Locked;
    }

    public void Unlock()
    {
        if (!IsLocked)
            return;
        InvokeOnUnlocked();
        CurrentDoorState = DoorState.Closed;
    }

    public bool IsClosing
    {
        get { return InState(DoorState.Closing); }
    }

    public bool IsClosed
    {
        get { return InState(DoorState.Closed); }
    }

    public bool IsClosedOrClosing
    {
        get{ return IsClosed || IsClosing; }
    }

    public bool IsOpening
    {
        get { return InState(DoorState.Opening); }
    }

    public bool IsOpen
    {
        get { return InState(DoorState.Open); }
    }

    public bool IsOpenOrOpening
    {
        get { return IsOpen || IsOpening; }
    }

    public bool IsLocked
    {
        get { return InState(DoorState.Locked); }
    }

    public bool IsUnlocked
    {
        get { return !IsLocked; }
    }

    bool InState(DoorState state)
    {
        return CurrentDoorState == state;
    }
    
    /// <summary>
    /// just do movement states handled by base
    /// </summary>
    protected abstract IEnumerator OpenCoroutine();
    /// <summary>
    /// just do movement states handled by base
    /// </summary>
    protected abstract IEnumerator CloseCoroutine();

    IEnumerator BaseOpenCoroutine()
    {
        if (disableColliderOnMovement)
            doorCollider.enabled = false;

        openRoutine = StartCoroutine(OpenCoroutine());
        yield return openRoutine;

        if (disableColliderOnMovement)
            doorCollider.enabled = true;

        CurrentDoorState = DoorState.Open;
    }

    IEnumerator BaseCloseCoroutine()
    {
        if (disableColliderOnMovement)
            doorCollider.enabled = false;

        closeRoutine = StartCoroutine(CloseCoroutine());
        yield return closeRoutine;

        if (disableColliderOnMovement)
            doorCollider.enabled = true;
        CurrentDoorState = DoorState.Closed;
    }

    #region event register invoke functions
    //open
    public void RegisterToOnOpen(Action<BaseDoor> callback)
    {
        OnOpen += callback;
    }

    public void UnregisterFromOnOpen(Action<BaseDoor> callback)
    {
        OnOpen -= callback;
    }

    protected void InvokeOnOpen()
    {
        OnOpen?.Invoke(this);
    }

    //opening
    public void RegisterToOnOpening(Action<BaseDoor> callback)
    {
        OnOpening += callback;
    }

    public void UnregisterFromOnOpening(Action<BaseDoor> callback)
    {
        OnOpening -= callback;
    }

    protected void InvokeOnOpening()
    {
        OnOpening?.Invoke(this);
    }

    //closed
    public void RegisterToOnClosed(Action<BaseDoor> callback)
    {
        OnClosed += callback;
    }

    public void UnregisterFromOnClosed(Action<BaseDoor> callback)
    {
        OnClosed -= callback;
    }

    protected void InvokeOnClosed()
    {
        OnClosed?.Invoke(this);
    }

    //closing
    public void RegisterToOnClosing(Action<BaseDoor> callback)
    {
        OnClosing += callback;
    }

    public void UnregisterFromOnClosing(Action<BaseDoor> callback)
    {
        OnClosing -= callback;
    }

    protected void InvokeOnClosing()
    {
        OnClosing?.Invoke(this);
    }

    //locked
    public void RegisterToOnLocked(Action<BaseDoor> callback)
    {
        OnLocked += callback;
    }

    public void UnregisterFromOnLocked(Action<BaseDoor> callback)
    {
        OnLocked -= callback;
    }

    protected void InvokeOnLocked()
    {
        OnLocked?.Invoke(this);
    }

    //unlocked
    public void RegisterToOnUnlocked(Action<BaseDoor> callback)
    {
        OnUnlocked += callback;
    }

    public void UnregisterFromOnUnlocked(Action<BaseDoor> callback)
    {
        OnUnlocked -= callback;
    }

    protected void InvokeOnUnlocked()
    {
        OnUnlocked?.Invoke(this);
    }

    #endregion
}
