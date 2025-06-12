using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class RecipeSO : ScriptableObject{

    public List<KitchenObjectOS> kitchenObjectsOSList;
    public string recipeName;


    public bool isRecipe;
    public List<KitchenObjectOS> RecipekitchenObjectsOSList;

}
