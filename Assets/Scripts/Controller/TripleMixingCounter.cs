using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TripleMixingCounter : KitchenObjectHolder, IHasProgress
{
    [Header("mixing settings")]
    [SerializeField] private float mixingTime = 3f;
    [SerializeField] private float mixingCooldown = 1f;
    private float currentMixingTime = 0f;
    private float currentCooldown = 0f;
    private bool isMixing = false;
    private bool isCooldown = false;

    public static event EventHandler OnAnyMix;
    public new static void ResetStaticData()
    {
        OnAnyMix = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnMix;

    [SerializeField] private TripleMixingRecipeSO[] mixingRecipeSOArray;

    [SerializeField] private Transform itemHoldPoint1;
    [SerializeField] private Transform itemHoldPoint2;
    [SerializeField] private Transform itemHoldPoint3;

    private KitchenObject item1;
    private KitchenObject item2;
    private KitchenObject item3;

    private void Update()
    {
        if (isMixing)
        {
            currentMixingTime += Time.deltaTime;

            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
            {
                progressNormalized = currentMixingTime / mixingTime
            });

            if (currentMixingTime >= mixingTime)
            {
                isMixing = false;
                CompleteMixing();
            }
        }
        else if (isCooldown)
        {
            currentCooldown -= Time.deltaTime;

            if (currentCooldown <= 0)
            {
                isCooldown = false;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs
                {
                    progressNormalized = 0f
                });
            }
        }
    }

    public override void Interact(PlayerController player)
    {
        if (!HasItemsOnCounter() && !player.IsHoldingObject())
        {
            
            GetHeldObject()?.SetKitchenObjectParent(player);
            return;
        }

        if (!HasItemsOnCounter())
        {
           
            if (player.IsHoldingObject())
            {
                PlaceItemOnCounter(player, 1);
            }
        }
        else if (HasOneItemOnCounter())
        {
           
            if (player.IsHoldingObject())
            {
                PlaceItemOnCounter(player, 2);
            }
            else
            {
                PickupItemFromCounter(player, 1);
            }
        }
        else if (HasTwoItemsOnCounter())
        {
            
            if (player.IsHoldingObject())
            {
                PlaceItemOnCounter(player, 3);

              
                if (item1 != null && item2 != null && item3 != null)
                {
                    KitchenObjectOS input1 = item1.GetHeldObjectOS();
                    KitchenObjectOS input2 = item2.GetHeldObjectOS();
                    KitchenObjectOS input3 = item3.GetHeldObjectOS();

                    if (HasRecipeWithInputs(input1, input2, input3))
                    {
                        StartMixing();
                    }
                }
            }
            else
            {
                
                if (item1 != null)
                {
                    PickupItemFromCounter(player, 1);
                }
                else if (item2 != null)
                {
                    PickupItemFromCounter(player, 2);
                }
            }
        }
        else
        {
            
            if (player.IsHoldingObject())
            {
                
                if (player.GetHeldObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    bool addedAll = true;

                    if (item1 != null && !plateKitchenObject.TryAddIngredient(item1.GetHeldObjectOS()))
                        addedAll = false;
                    if (item2 != null && !plateKitchenObject.TryAddIngredient(item2.GetHeldObjectOS()))
                        addedAll = false;
                    if (item3 != null && !plateKitchenObject.TryAddIngredient(item3.GetHeldObjectOS()))
                        addedAll = false;

                    if (addedAll)
                    {
                        if (item1 != null) item1.DestroySelf();
                        if (item2 != null) item2.DestroySelf();
                        if (item3 != null) item3.DestroySelf();
                        ClearItems();
                    }
                }
            }
            else
            {
               
                if (item1 != null)
                {
                    PickupItemFromCounter(player, 1);
                }
                else if (item2 != null)
                {
                    PickupItemFromCounter(player, 2);
                }
                else if (item3 != null)
                {
                    PickupItemFromCounter(player, 3);
                }
            }
        }
    }

    private void StartMixing()
    {
        if (isMixing || isCooldown) return;

        isMixing = true;
        currentMixingTime = 0f;

        OnMix?.Invoke(this, EventArgs.Empty);
        OnAnyMix?.Invoke(this, EventArgs.Empty);
    }

    private void CompleteMixing()
    {
        KitchenObjectOS outputKitchenObjectOS = GetOutputForInputs(
            item1.GetHeldObjectOS(),
            item2.GetHeldObjectOS(),
            item3.GetHeldObjectOS());

        item1.DestroySelf();
        item2.DestroySelf();
        item3.DestroySelf();

        KitchenObject.SpwanKitchenObject(outputKitchenObjectOS, this);

        ClearItems();
        StartCooldown();
    }

    private void StartCooldown()
    {
        isCooldown = true;
        currentCooldown = mixingCooldown;
    }

    private void PlaceItemOnCounter(PlayerController player, int position)
    {
        KitchenObject kitchenObject = player.GetHeldObject();

        switch (position)
        {
            case 1:
                item1 = kitchenObject;
                kitchenObject.transform.position = itemHoldPoint1.position;
                kitchenObject.transform.SetParent(itemHoldPoint1);
                break;
            case 2:
                item2 = kitchenObject;
                kitchenObject.transform.position = itemHoldPoint2.position;
                kitchenObject.transform.SetParent(itemHoldPoint2);
                break;
            case 3:
                item3 = kitchenObject;
                kitchenObject.transform.position = itemHoldPoint3.position;
                kitchenObject.transform.SetParent(itemHoldPoint3);
                break;
        }

        kitchenObject.SetKitchenObjectParent(this);
    }

    private void PickupItemFromCounter(PlayerController player, int position)
    {
        KitchenObject itemToPickup = null;

        switch (position)
        {
            case 1:
                itemToPickup = item1;
                item1 = null;
                break;
            case 2:
                itemToPickup = item2;
                item2 = null;
                break;
            case 3:
                itemToPickup = item3;
                item3 = null;
                break;
        }

        if (itemToPickup != null)
        {
            itemToPickup.SetKitchenObjectParent(player);
        }
    }

    private bool HasItemsOnCounter()
    {
        return item1 != null || item2 != null || item3 != null;
    }

    private bool HasOneItemOnCounter()
    {
        return (item1 != null && item2 == null && item3 == null) ||
               (item1 == null && item2 != null && item3 == null) ||
               (item1 == null && item2 == null && item3 != null);
    }

    private bool HasTwoItemsOnCounter()
    {
        return (item1 != null && item2 != null && item3 == null) ||
               (item1 != null && item2 == null && item3 != null) ||
               (item1 == null && item2 != null && item3 != null);
    }

    private void ClearItems()
    {
        item1 = null;
        item2 = null;
        item3 = null;
    }

    private bool HasRecipeWithInputs(KitchenObjectOS input1, KitchenObjectOS input2, KitchenObjectOS input3)
    {
        TripleMixingRecipeSO recipe = GetMixingRecipeWithInputs(input1, input2, input3);
        return recipe != null;
    }

    private KitchenObjectOS GetOutputForInputs(KitchenObjectOS input1, KitchenObjectOS input2, KitchenObjectOS input3)
    {
        TripleMixingRecipeSO recipe = GetMixingRecipeWithInputs(input1, input2, input3);
        return recipe?.output;
    }

    private TripleMixingRecipeSO GetMixingRecipeWithInputs(KitchenObjectOS input1, KitchenObjectOS input2, KitchenObjectOS input3)
    {
        foreach (TripleMixingRecipeSO recipe in mixingRecipeSOArray)
        {
            if (IsRecipeMatch(recipe, input1, input2, input3))
            {
                return recipe;
            }
        }
        return null;
    }

    private bool IsRecipeMatch(TripleMixingRecipeSO recipe, KitchenObjectOS input1, KitchenObjectOS input2, KitchenObjectOS input3)
    {
        
        List<KitchenObjectOS> inputs = new List<KitchenObjectOS> { input1, input2, input3 };
        List<KitchenObjectOS> recipeInputs = new List<KitchenObjectOS> { recipe.input1, recipe.input2, recipe.input3 };

        return inputs.Count == recipeInputs.Count &&
               inputs.All(i => recipeInputs.Contains(i)) &&
               recipeInputs.All(ri => inputs.Contains(ri));
    }
}