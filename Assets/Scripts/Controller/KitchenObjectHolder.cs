using System;
using UnityEngine;

public abstract class KitchenObjectHolder : MonoBehaviour
{
    public static event EventHandler OnAnyObjectPlaced;

    public static void ResetStaticData()
    {
        OnAnyObjectPlaced = null;
    }

    [SerializeField] private Transform _holdPoint;

    public KitchenObject _heldKitchenObject;
    private void Start()
    {
    }
    public virtual void Interact(PlayerController player)
    {
        Debug.LogError("KitchenObjectHolder.Interact() - Base method called!");
    }

    public virtual void InteractAlternate(PlayerController player)
    {
       
    }

    public virtual void Interact(KitchenAI kitchenAI)
    {
        Debug.LogError("KitchenObjectHolder.Interact() - Base method called for AI!");
    }

    public virtual void InteractAlternate(KitchenAI kitchenAI)
    {
      
    }

    
    public Transform GetHoldPoint()
    {
        return _holdPoint;
    }

    
    public Transform GetHeldObjectFollowTransform()
    {
        return _holdPoint;
    }

    public void HoldObject(KitchenObject kitchenObject)
    {
        _heldKitchenObject = kitchenObject;
        Debug.Log("321322222222222");
        if (kitchenObject != null)
        {
            OnAnyObjectPlaced?.Invoke(this, EventArgs.Empty);
        }
    }

    
    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        HoldObject(kitchenObject);
    }

    public KitchenObject GetHeldObject()
    {
        return _heldKitchenObject;
    }

    public void ClearHeldObject()
    {
        _heldKitchenObject = null;
    }

    public bool IsHoldingObject()
    {
        return _heldKitchenObject != null;
    }
}