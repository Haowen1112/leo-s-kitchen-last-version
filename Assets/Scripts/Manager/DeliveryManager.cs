using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class DeliveryManager : MonoBehaviour {

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeComplete;
    public event EventHandler OnRecipeSucess;
    public event EventHandler OnRecipeFailed;

    public static DeliveryManager Instance { get; private set; }



    [SerializeField] private RecipeListSO recipeListSO;

    private List<RecipeSO> waitingRecipeSOList;
    private float spawnRecipeTimer;
    private float spawnRecipeTimerMax = 4f;
    private int waitingRecipeMax = 4;
    private int successfulRecipesAmount;
    private int gold;

    public TextMeshProUGUI goldText;
    public int GameOverIndex;

    int detectionGold;

    private void Awake()
    {

        Instance = this;

        waitingRecipeSOList = new List<RecipeSO>();
    }

    private void Start()
    {
        gold = 0;
        goldText.text = gold.ToString();
    }

    private void Update() {
        spawnRecipeTimer -= Time.deltaTime;
        goldText.text = gold.ToString();
       
        if (spawnRecipeTimer < 0f)
        {
            spawnRecipeTimer = spawnRecipeTimerMax;

            if (KitchenGameManager.Instance.IsGamePlaying() && waitingRecipeSOList.Count < waitingRecipeMax)
            {
                RecipeSO waitingRecipeSO = recipeListSO.recipeSOList[UnityEngine.Random.Range(0, recipeListSO.recipeSOList.Count)];
                waitingRecipeSOList.Add(waitingRecipeSO);

                OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public void DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for(int i = 0; i < waitingRecipeSOList.Count; i++) {
            RecipeSO waitingRecipeSO = waitingRecipeSOList[i];

            if(waitingRecipeSO.kitchenObjectsOSList.Count == plateKitchenObject.GetHeldObjectOSList().Count ) {
                
                bool plateContentMatchesRecipe = true;

                foreach(KitchenObjectOS recipeKitchenObjectSO in waitingRecipeSO.kitchenObjectsOSList) {
                  
                    bool ingredientsFound = false;

                    foreach (KitchenObjectOS plateKitchenObjectSO in plateKitchenObject.GetHeldObjectOSList()) {
                    
                        if(plateKitchenObjectSO == recipeKitchenObjectSO) {
                           
                            ingredientsFound = true;
                            break;
                        }
                    }
                    if(!ingredientsFound ) {
                        
                        plateContentMatchesRecipe = false;
                    }
                }

                if(plateContentMatchesRecipe) {
                 

                    successfulRecipesAmount++;

                    waitingRecipeSOList.RemoveAt(i);

                    OnRecipeComplete?.Invoke(this, EventArgs.Empty);
                    OnRecipeSucess?.Invoke(this, EventArgs.Empty);
                   
                    return;
                }
            }

        }

        
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);

    }

    public void AddGold(int num)
    {
        gold += num;
         detectionGold = gold;
    }

    public List<RecipeSO> GetWaitingRecipeSOList(){
        return waitingRecipeSOList;
    }

    public int GetSuccessfulRecipesAmount() {
        return successfulRecipesAmount;
    }

public int GetDetectionGold()
    {
        return detectionGold;
    }

    public int GetGoldSuccessfulRecipesAmount()
    {
        return gold;
    }
}

