using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{

    [SerializeField] private KitchenObjectHolder KitchenObjectHolder;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start() {
        PlayerController.Instance.OnSelectedCounterChanged += Instance_OnSelectedCounterChanged;
    }

    private void Instance_OnSelectedCounterChanged(object sender, PlayerController.OnSelectedCounterChangedEventArgs e) {
        if(e.selectedCounter == KitchenObjectHolder) {
            Show();
        }
        else {
            Hide();
        }

    }

    private void Show() {

        foreach (GameObject visualGameObject in visualGameObjectArray) {
            //visualGameObject.SetActive(true);
        }
    } 
    private void Hide() {
        foreach (GameObject visualGameObject in visualGameObjectArray) {
            //visualGameObject.SetActive(false);
        }
    }
}
