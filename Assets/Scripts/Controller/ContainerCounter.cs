using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounter : KitchenObjectHolder
{

    public event EventHandler OnPlayerGrabbedObject;

    [SerializeField] private KitchenObjectOS[] kitchenObjectOS;

    int index;
    public override void Interact(PlayerController player) {
       
        //if (!player.IsHoldingObject()) {
        //player os not carring anything
        if (player.IsHoldingObject())
            player.GetHeldObject().DestroySelf();

            KitchenObject.SpwanKitchenObject(kitchenObjectOS[index],player);
        index++;
        if (index > kitchenObjectOS.Length - 1)
        {
            index = 0;
        }
        AudioManager.Instance.PlayTrashSound(transform.position);
        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
      //  }
    }

    public override void Interact(KitchenAI player)
    {

        //if (!player.IsHoldingObject()) {
        //player os not carring anything
        if (player.IsHoldingObject())
            player.GetHeldObject().DestroySelf();

        KitchenObject.SpwanKitchenObject(kitchenObjectOS[index], player);
        index++;
        if (index > kitchenObjectOS.Length - 1)
        {
            index = 0;
        }

        OnPlayerGrabbedObject?.Invoke(this, EventArgs.Empty);
        //  }
    }

}
