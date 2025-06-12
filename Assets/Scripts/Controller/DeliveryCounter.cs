using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : KitchenObjectHolder
{

    public static DeliveryCounter Instance { get; private set; }
    public KitchenObjectOS outputKitchenObjectSO;
    private void Awake() {
        Instance = this;
    }


    public override void Interact(PlayerController player) {
        if (!GetHeldObject())
        {

        if (player.IsHoldingObject()) {
            if (player.GetHeldObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    
                    AudioManager.Instance.PlayDeliveryFailSound(transform.position);
                DeliveryManager.Instance.DeliverRecipe(plateKitchenObject);
                KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
                player.GetHeldObject().DestroySelf();
            }
        }
        else
        {
            GetHeldObject().SetKitchenObjectParent(player);
                ClearHeldObject();
        }
        }
        else
        {
            if (!player.IsHoldingObject())
            {
                GetHeldObject().SetKitchenObjectParent(player);
                ClearHeldObject();
            }
        }

    }

}