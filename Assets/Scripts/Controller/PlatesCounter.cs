using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : KitchenObjectHolder {


    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;


    [SerializeField] private KitchenObjectOS plateKitchenObjectSO;


    private float spawnPlateTimer;
    private float spawnPlateTimerMax = 4f;
    private int platesSpawnedAmount;
    private int platesSpawnedAmountMax = 4;
    public int maxPlates;
    private void Start()
    {
        for (int i = 0; i < maxPlates; i++)
        {
            platesSpawnedAmount++;

            OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        }
    }


    private void Update() {
        //spawnPlateTimer += Time.deltaTime;
        //if (spawnPlateTimer > spawnPlateTimerMax) {
        //    spawnPlateTimer = 0f;

        //    if (KitchenGameManager.Instance.IsGamePlaying() && platesSpawnedAmount < platesSpawnedAmountMax) {
        //        platesSpawnedAmount++;

        //        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
        //    }
        //}
    }

    public override void Interact(PlayerController player) {
        if (!player.IsHoldingObject()) {
            
            if (platesSpawnedAmount > 0) {
                
                platesSpawnedAmount--;

                KitchenObject.SpwanKitchenObject(plateKitchenObjectSO,player);

                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
            }
        }
    }

}
