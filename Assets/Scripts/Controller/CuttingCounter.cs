using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CuttingCounter : KitchenObjectHolder, IHasProgress{


    public static event EventHandler OnAnyCut;

    new public static void ResetStaticData() {
        OnAnyCut= null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler OnCut;

    [SerializeField] private CuttingReciepeSO[] cutKitchenObjectOSArray;

    private int cuttingProgress;

    public override void Interact(PlayerController player) {

        if (player.IsHoldingObject() && player.GetHeldObject().GetHeldObjectOS().objectName == "DirtyDish")
        {
           
            KitchenObjectOS outputKitchenObjectSO = GetOutputForInput(GetHeldObject().GetHeldObjectOS());

            GetHeldObject().DestroySelf();
            KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, player);
        }
        if (!IsHoldingObject()) {
           
            if (player.IsHoldingObject()) {
                

                if (HasRecipeWithInput(player.GetHeldObject().GetHeldObjectOS())){
                   
                    player.GetHeldObject().SetKitchenObjectParent(this);
                    cuttingProgress= 0;
        
                    CuttingReciepeSO cuttingReciepeSO = GetCuttingSOWithInput(GetHeldObject().GetHeldObjectOS());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = (float)cuttingProgress / cuttingReciepeSO.cuttingPogressMax
                    });
                }
            }
            else {
                
            }
        }
        else {
          
            if (player.IsHoldingObject()) {
                
                if (player.GetHeldObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                   
                    if (plateKitchenObject.TryAddIngredient(GetHeldObject().GetHeldObjectOS())) {
                        GetHeldObject().DestroySelf();
                    }
                }
            }
            else {
                
                GetHeldObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void Interact(KitchenAI player)
    {
        if (!GetHeldObject())
        {

        if (player.IsHoldingObject() && player.GetHeldObject().GetHeldObjectOS().objectName == "DirtyDish")
        {
           
            KitchenObjectOS outputKitchenObjectSO = GetOutputForInput(GetHeldObject().GetHeldObjectOS());

            GetHeldObject().DestroySelf();
            KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, player);
        }
        if (!IsHoldingObject())
        {
            
            if (player.IsHoldingObject())
            {
              

                if (HasRecipeWithInput(player.GetHeldObject().GetHeldObjectOS()))
                {
                    
                    player.GetHeldObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;
                        ClearHeldObject();
                        CuttingReciepeSO cuttingReciepeSO = GetCuttingSOWithInput(GetHeldObject().GetHeldObjectOS());

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                    {
                        progressNormalized = (float)cuttingProgress / cuttingReciepeSO.cuttingPogressMax
                    });
                }
            }
            else
            {
                
            }
        }
        else
        {
            
            if (player.IsHoldingObject())
            {
                
                if (player.GetHeldObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    
                    if (plateKitchenObject.TryAddIngredient(GetHeldObject().GetHeldObjectOS()))
                    {
                        GetHeldObject().DestroySelf();
                            ClearHeldObject();
                        }
                }
            }
            else
            {
               
                GetHeldObject().SetKitchenObjectParent(player);
                    ClearHeldObject();
            }
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


    public override void InteractAlternate(KitchenAI player)
    {
        if (IsHoldingObject() && HasRecipeWithInput(GetHeldObject().GetHeldObjectOS()))
        {
            
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);
            OnAnyCut?.Invoke(this, EventArgs.Empty);

            AudioManager.Instance.PlayChopSound(transform.position);
            CuttingReciepeSO cuttingReciepeSO = GetCuttingSOWithInput(GetHeldObject().GetHeldObjectOS());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingReciepeSO.cuttingPogressMax
            });

            if (cuttingProgress >= cuttingReciepeSO.cuttingPogressMax)
            {

                KitchenObjectOS outputKitchenObjectSO = GetOutputForInput(GetHeldObject().GetHeldObjectOS());

                GetHeldObject().DestroySelf();

                KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }


    public override void InteractAlternate(PlayerController player) {
        if (IsHoldingObject() && HasRecipeWithInput(GetHeldObject().GetHeldObjectOS())) {
           
            cuttingProgress++;

            OnCut?.Invoke(this,EventArgs.Empty);
            OnAnyCut?.Invoke(this,EventArgs.Empty);

           
            CuttingReciepeSO cuttingReciepeSO = GetCuttingSOWithInput(GetHeldObject().GetHeldObjectOS());

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = (float)cuttingProgress / cuttingReciepeSO.cuttingPogressMax
            });

            if (cuttingProgress >= cuttingReciepeSO.cuttingPogressMax) {

                KitchenObjectOS outputKitchenObjectSO = GetOutputForInput(GetHeldObject().GetHeldObjectOS());

                GetHeldObject().DestroySelf();

                KitchenObject.SpwanKitchenObject(outputKitchenObjectSO, this);
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectOS inputKitchenObjectOS) {

        CuttingReciepeSO cuttingReciepeSO = GetCuttingSOWithInput(inputKitchenObjectOS);
        return cuttingReciepeSO != null;

    }

    private KitchenObjectOS GetOutputForInput(KitchenObjectOS inputKitchenObjectOS) {
        CuttingReciepeSO cuttingReciepeSO = GetCuttingSOWithInput(inputKitchenObjectOS);
        if(cuttingReciepeSO != null) {
            return cuttingReciepeSO.output;
        }else {
            return null;
        }
    }

    private CuttingReciepeSO GetCuttingSOWithInput(KitchenObjectOS inputKitchenObjectOS) {
        foreach (CuttingReciepeSO cuttingReciepeSO in cutKitchenObjectOSArray) {
            if (cuttingReciepeSO.input == inputKitchenObjectOS) {
                return cuttingReciepeSO;
            }
        }

        return null;
    }
}
