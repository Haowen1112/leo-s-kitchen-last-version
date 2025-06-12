using System;
using UnityEngine;

public class ClearCounter : KitchenObjectHolder
{
    public static new event EventHandler OnAnyObjectPlaced;

    public static new void ResetStaticData()
    {
        OnAnyObjectPlaced = null;
    }

    [SerializeField] private KitchenObjectOS kitchenObjectOS; // This might be unused in current implementation
    private void Start()
    {
        
    }
    // Override base class methods with more specific implementations
    public override void Interact(PlayerController player)
    {
       
        if (!IsHoldingObject())
        {
            Debug.Log(PlayerController.Instance.IsHoldingObject());
            if (PlayerController.Instance.IsHoldingObject())
            {

                // Player is carrying something - transfer to counter
                PlayerController.Instance.GetHeldObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            // There is a KitchenObject on counter
            if (PlayerController.Instance.IsHoldingObject())
            {
                // Player is carrying something
                if (PlayerController.Instance.GetHeldObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    // Player is holding a plate - try to add counter's object to plate
                    if (plateKitchenObject.TryAddIngredient(GetHeldObject().GetHeldObjectOS()))
                    {
                        GetHeldObject().DestroySelf();
                    }
                }
                else if (GetHeldObject().TryGetPlate(out plateKitchenObject))
                {
                    // Counter is holding a plate - try to add player's object to plate
                    if (plateKitchenObject.TryAddIngredient(PlayerController.Instance.GetHeldObject().GetHeldObjectOS()))
                    {
                        PlayerController.Instance.GetHeldObject().DestroySelf();
                    }
                }
            }
            else
            {
                // Player is not carrying anything - give counter's object to player
                GetHeldObject().SetKitchenObjectParent(PlayerController.Instance);
            }
        }
    }

    public override void Interact(KitchenAI kitchenAI)
    {
        // Implement AI interaction (same logic as player interaction)
        if (!IsHoldingObject())
        {
            if (kitchenAI.IsHoldingObject())
            {
                kitchenAI.GetHeldObject().SetKitchenObjectParent(this);
            }
        }
        else
        {
            if (kitchenAI.IsHoldingObject())
            {
                if (kitchenAI.GetHeldObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(GetHeldObject().GetHeldObjectOS()))
                    {
                        GetHeldObject().DestroySelf();
                    }
                }
                else if (GetHeldObject().TryGetPlate(out plateKitchenObject))
                {
                    if (plateKitchenObject.TryAddIngredient(kitchenAI.GetHeldObject().GetHeldObjectOS()))
                    {
                        kitchenAI.GetHeldObject().DestroySelf();
                    }
                }
            }
            else
            {
                GetHeldObject().SetKitchenObjectParent(kitchenAI);
            }
        }
    }

    // Optional alternate interactions
    public override void InteractAlternate(PlayerController player)
    {
        // Implement any alternate interaction logic here
    }

    public override void InteractAlternate(KitchenAI kitchenAI)
    {
        // Implement any alternate AI interaction logic here
    }
}