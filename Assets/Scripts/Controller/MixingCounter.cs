using System;
using UnityEngine;

public class MixingCounter : KitchenObjectHolder, IHasProgress
{
    [Header("mixing settings")]
    [SerializeField] private float mixingTime = 3f;
    [SerializeField] private float mixingCooldown = 1f;
    private float currentMixingTime = 0f;
    private float currentCooldown = 0f;
    private bool isMixing = false;
    private bool isCooldown = false;

    public static event EventHandler OnAnyMix;
    new public static void ResetStaticData()
    {
        OnAnyMix = null;
    }

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    public event EventHandler OnMix;

    [SerializeField] private MixingRecipeSO[] mixingRecipeSOArray;

    private KitchenObjectOS firstIngredient;
    private KitchenObjectOS secondIngredient;

    [SerializeField] private Transform itemHoldPoint1;
    [SerializeField] private Transform itemHoldPoint2;

    private KitchenObject item1;
    private KitchenObject item2;

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
        Debug.Log(HasItemsOnCounter());
        Debug.Log(player.IsHoldingObject());
        if (!HasItemsOnCounter()&& !player.IsHoldingObject())
        {
            GetHeldObject().SetKitchenObjectParent(player);
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

                if (item1 != null && item2 != null)
                {
                    KitchenObjectOS input1 = item1.GetHeldObjectOS();
                    KitchenObjectOS input2 = item2.GetHeldObjectOS();

                    if (HasRecipeWithInputs(input1, input2))
                    {
                        StartMixing();
                    }
                }
            }
            else
            {
                PickupItemFromCounter(player, 1);
            }
        }
        else
        {
            if (player.IsHoldingObject())
            {
                if (player.GetHeldObject().TryGetPlate(out PlateKitchenObject plateKitchenObject))
                {
                    bool addedBoth = true;

                    if (item1 != null && !plateKitchenObject.TryAddIngredient(item1.GetHeldObjectOS()))
                        addedBoth = false;

                    if (item2 != null && !plateKitchenObject.TryAddIngredient(item2.GetHeldObjectOS()))
                        addedBoth = false;

                    if (addedBoth)
                    {
                        if (item1 != null) item1.DestroySelf();
                        if (item2 != null) item2.DestroySelf();
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
            item2.GetHeldObjectOS());

        item1.DestroySelf();
        item2.DestroySelf();

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

        if (position == 1)
        {
            item1 = kitchenObject;
            firstIngredient = kitchenObject.GetHeldObjectOS();
            kitchenObject.transform.position = itemHoldPoint1.position;
            kitchenObject.transform.SetParent(itemHoldPoint1);
        }
        else if (position == 2)
        {
            item2 = kitchenObject;
            secondIngredient = kitchenObject.GetHeldObjectOS();
            kitchenObject.transform.position = itemHoldPoint2.position;
            kitchenObject.transform.SetParent(itemHoldPoint2);
        }

        kitchenObject.SetKitchenObjectParent(this);
    }

    private void PickupItemFromCounter(PlayerController player, int position)
    {
        if (position == 1 && item1 != null)
        {
            item1.SetKitchenObjectParent(player);
            item1 = null;
            firstIngredient = null;
        }
        else if (position == 2 && item2 != null)
        {
            item2.SetKitchenObjectParent(player);
            item2 = null;
            secondIngredient = null;
        }
    }

    private bool HasItemsOnCounter()
    {
        return item1 != null || item2 != null;
    }

    private bool HasOneItemOnCounter()
    {
        return (item1 != null && item2 == null) || (item1 == null && item2 != null);
    }

    private void ClearItems()
    {
        item1 = null;
        item2 = null;
        firstIngredient = null;
        secondIngredient = null;
    }

    private bool HasRecipeWithInputs(KitchenObjectOS input1, KitchenObjectOS input2)
    {
        MixingRecipeSO mixingRecipeSO = GetMixingRecipeWithInputs(input1, input2);
        return mixingRecipeSO != null;
    }

    private KitchenObjectOS GetOutputForInputs(KitchenObjectOS input1, KitchenObjectOS input2)
    {
        MixingRecipeSO mixingRecipeSO = GetMixingRecipeWithInputs(input1, input2);
        if (mixingRecipeSO != null)
        {
            return mixingRecipeSO.output;
        }
        return null;
    }

    private MixingRecipeSO GetMixingRecipeWithInputs(KitchenObjectOS input1, KitchenObjectOS input2)
    {
        foreach (MixingRecipeSO mixingRecipeSO in mixingRecipeSOArray)
        {
            if ((mixingRecipeSO.input1 == input1 && mixingRecipeSO.input2 == input2) ||
                (mixingRecipeSO.input1 == input2 && mixingRecipeSO.input2 == input1))
            {
                return mixingRecipeSO;
            }
        }
        return null;
    }
}