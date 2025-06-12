using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class KitchenAI : KitchenObjectHolder
{
    [Header("Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float interactionDistance = 1.2f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float actionDelay = 0.3f;

    [Header("Counters")]
    [SerializeField] private List<KitchenObjectHolder> allCounters = new List<KitchenObjectHolder>();
    [SerializeField] private List<KitchenObjectHolder> plateCounters = new List<KitchenObjectHolder>();
    [SerializeField] private List<KitchenObjectHolder> itemCounters = new List<KitchenObjectHolder>();

    [Header("Special Counters")]
    [SerializeField] private ContainerCounter fridgeCounter;
    [SerializeField] private KitchenObjectHolder washingCounter;

    [Header("Object References")]
    [SerializeField] private KitchenObjectOS dirtyDishSO;
    [SerializeField] private KitchenObjectOS cleanPlateSO;
    [SerializeField] private Transform _holdPoint;

    private KitchenObject _heldObject;
    private KitchenObjectHolder currentTarget;
    private float lastActionTime;
    private bool isMoving;

    private enum AIState { Idle, Moving, Interacting }
    private AIState currentState = AIState.Idle;
    public TMP_Dropdown dropdown;

    private void Update()
    {
        switch (currentState)
        {
            case AIState.Idle:
                FindNextTask();
                break;

            case AIState.Moving:
                HandleMovement();
                break;

            case AIState.Interacting:
                if (Time.time > lastActionTime + actionDelay)
                {
                    PerformInteraction();
                    currentState = AIState.Idle;
                }
                break;
        }
    }

    private void FindNextTask()
    {
        
        if (FindDirtyDish(out KitchenObjectHolder dirtyCounter))
        {
            StartMovingToTarget(dirtyCounter);
            return;
        }

        
        if (IsHoldingCleanPlate())
        {
            if (FindEmptyPlateCounter(out KitchenObjectHolder emptyPlateCounter))
            {
                StartMovingToTarget(emptyPlateCounter);
                return;
            }
        }

       
        if (IsHoldingObject() && !IsCleanPlate(GetHeldObject()))
        {
            if (FindEmptyItemCounter(out KitchenObjectHolder emptyItemCounter))
            {
                StartMovingToTarget(emptyItemCounter);
                return;
            }
        }

      
        if (!IsHoldingObject())
        {
            if (FindEmptyItemCounter(out _)) 
            {
                StartMovingToTarget(fridgeCounter);
                return;
            }
        }

        currentState = AIState.Idle;
    }

    private bool FindDirtyDish(out KitchenObjectHolder dirtyCounter)
    {
        dirtyCounter = null;
        foreach (var counter in allCounters)
        {
            if (counter.IsHoldingObject() && IsDirtyDish(counter.GetHeldObject()))
            {
                dirtyCounter = counter;
                return true;
            }
        }
        return false;
    }

    private bool FindEmptyPlateCounter(out KitchenObjectHolder emptyCounter)
    {
        emptyCounter = null;
        foreach (var counter in plateCounters)
        {
            if (!counter.IsHoldingObject())
            {
                emptyCounter = counter;
                return true;
            }
        }
        return false;
    }

    private bool FindEmptyItemCounter(out KitchenObjectHolder emptyCounter)
    {
        emptyCounter = null;
        foreach (var counter in itemCounters)
        {
            if (!counter.IsHoldingObject())
            {
                emptyCounter = counter;
                return true;
            }
        }
        return false;
    }

    private bool IsHoldingCleanPlate()
    {
        return IsHoldingObject() && IsCleanPlate(GetHeldObject());
    }

    private bool IsDirtyDish(KitchenObject ko)
    {
        return ko != null && ko.GetHeldObjectOS() == dirtyDishSO;
    }

    private bool IsCleanPlate(KitchenObject ko)
    {
        return ko != null && ko.GetHeldObjectOS() == cleanPlateSO;
    }

    private void StartMovingToTarget(KitchenObjectHolder target)
    {
        currentTarget = target;
        currentState = AIState.Moving;
        isMoving = true;
    }

    public void ChangeValue()
    {
        actionDelay = dropdown.value == 0 ? 1f : 0.4f;
    }

    private void HandleMovement()
    {
        if (currentTarget == null)
        {
            currentState = AIState.Idle;
            return;
        }

        Vector3 targetPos = currentTarget.transform.position;
        float distance = Vector3.Distance(transform.position, targetPos);

        if (distance <= interactionDistance)
        {
            isMoving = false;
            currentState = AIState.Interacting;
            lastActionTime = Time.time;
            return;
        }

        Vector3 moveDir = (targetPos - transform.position).normalized;
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        if (moveDir != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void PerformInteraction()
    {
        if (currentTarget == null) return;

        
        if (currentTarget is KitchenObjectHolder counter &&
            counter.IsHoldingObject() &&
            IsDirtyDish(counter.GetHeldObject()))
        {
            counter.Interact(this);
            StartMovingToTarget(washingCounter);
            return;
        }

        
        if (currentTarget == washingCounter &&
            IsHoldingObject() &&
            IsDirtyDish(GetHeldObject()))
        {
            
            GetHeldObject().DestroySelf();
            ClearHeldObject();

            
            KitchenObject.SpwanKitchenObject(cleanPlateSO, this);
            FindNextTask(); 
            return;
        }

        
        if (currentTarget is KitchenObjectHolder targetCounter &&
            IsHoldingCleanPlate() &&
            plateCounters.Contains(targetCounter) &&
            !targetCounter.IsHoldingObject())
        {
            targetCounter.Interact(this);
            return;
        }

        
        if (currentTarget == fridgeCounter && !IsHoldingObject())
        {
            fridgeCounter.Interact(this);
            return;
        }

        
        if (currentTarget is KitchenObjectHolder itemCounter &&
            IsHoldingObject() &&
            !IsCleanPlate(GetHeldObject()) &&
            itemCounters.Contains(itemCounter) &&
            !itemCounter.IsHoldingObject())
        {
            itemCounter.Interact(this);
            return;
        }

        currentState = AIState.Idle;
    }

    //#region IKitchenObjectHolder Implementation
    //public Transform GetHoldPoint() => _holdPoint;
    //public void HoldObject(KitchenObject kitchenObject) => _heldObject = kitchenObject;
    //public KitchenObject GetHeldObject() => _heldObject;
    //public void ClearHeldObject() => _heldObject = null;
    //public bool IsHoldingObject() => _heldObject != null;
    //#endregion
}