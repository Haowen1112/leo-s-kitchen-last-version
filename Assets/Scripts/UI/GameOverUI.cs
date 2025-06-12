using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverUI : MonoBehaviour {


    [SerializeField] private TextMeshProUGUI recipesDeliveredText;

    [SerializeField] private TextMeshProUGUI GoldText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private Button nextButton;


    private void Awake() {
        restartButton.onClick.AddListener(() => {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
        returnButton.onClick.AddListener(() => {

            SceneManager.LoadScene(0);
        });
        nextButton.onClick.AddListener(() => {

            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        });
    }


    private void Start() {
        KitchenGameManager.Instance.OnStateChanged += KitchenGameManager_OnStateChanged;

        Hide();
    }

    private void KitchenGameManager_OnStateChanged(object sender, System.EventArgs e) {
        if (KitchenGameManager.Instance.IsGameOver()) {
            Show();
            GoldText.text = DeliveryManager.Instance.GetGoldSuccessfulRecipesAmount().ToString();
            recipesDeliveredText.text = DeliveryManager.Instance.GetSuccessfulRecipesAmount().ToString();
        }
        else {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
        if (DeliveryManager.Instance.GetGoldSuccessfulRecipesAmount()!=DeliveryManager.Instance.GetDetectionGold())
        {
             #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }


}