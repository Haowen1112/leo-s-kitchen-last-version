using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KitchenObject : MonoBehaviour {

    [SerializeField] private KitchenObjectOS kitchenObjectOS;

    private KitchenObjectHolder kitchenObjectParent; 

    public KitchenObjectOS GetHeldObjectOS() {
        return kitchenObjectOS;
    }

    public void SetKitchenObjectParent(KitchenObjectHolder kitchenObjectParent) {
        if (this.kitchenObjectParent != null) {
            this.kitchenObjectParent.ClearHeldObject();
        }

        this.kitchenObjectParent = kitchenObjectParent;
        Debug.Log(kitchenObjectParent.IsHoldingObject());
        if (kitchenObjectParent.IsHoldingObject()) {
            Debug.LogError("Counter already has a KitchenObject!!");
        }

        kitchenObjectParent.SetKitchenObject(this);

        transform.parent = kitchenObjectParent.GetHeldObjectFollowTransform();
        transform.localPosition = Vector3.zero;
    }

    public KitchenObjectHolder GetHeldObjectParent() {
        return kitchenObjectParent;
    }

    public void DestroySelf() {
        kitchenObjectParent.ClearHeldObject();

        Destroy(gameObject);
    }

    public bool TryGetPlate(out PlateKitchenObject plateKitchenObject) {
        if(this is PlateKitchenObject) {
            plateKitchenObject = this as PlateKitchenObject;
            return true;
        }
        else {
            plateKitchenObject = null;
            return false;
        }
    } 

    public static KitchenObject SpwanKitchenObject(KitchenObjectOS kitchenObjectOS, KitchenObjectHolder kithenObjectParent) {
        Transform kitchenObjectTransform = Instantiate(kitchenObjectOS.prefab);

        KitchenObject kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
        
        kitchenObject.SetKitchenObjectParent(kithenObjectParent);

        return kitchenObject;
    }

}