using UnityEngine;

[CreateAssetMenu]
public class MixingRecipeSO : ScriptableObject
{
    public KitchenObjectOS input1;
    public KitchenObjectOS input2;
    public KitchenObjectOS output;
    public float mixingTime = 3f; 
}