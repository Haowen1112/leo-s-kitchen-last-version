using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
public class AuthManager : MonoBehaviour
{
    [Header("Login References")]
    public InputField loginUsernameInput;
    public InputField loginPasswordInput;
    public Button loginButton;
    public Text loginErrorText;

    [Header("Register References")]
    public InputField registerUsernameInput;
    public InputField registerPasswordInput;
    public InputField registerConfirmPasswordInput;
    public Button registerButton;
    public Text registerErrorText;

    [Header("Panels")]
    public GameObject loginPanel;
    public GameObject registerPanel;

    private void Start()
    {
        
        //ShowLoginPanel();

        
        loginButton.onClick.AddListener(OnLoginButtonClicked);
        registerButton.onClick.AddListener(OnRegisterButtonClicked);
    }

    
    public void ShowLoginPanel()
    {
        loginPanel.SetActive(true);
        registerPanel.SetActive(false);
        ClearAllInputs();
        ClearAllErrors();
    }

    
    public void ShowRegisterPanel()
    {
        loginPanel.SetActive(false);
        registerPanel.SetActive(true);
        ClearAllInputs();
        ClearAllErrors();
    }

    
    private void OnLoginButtonClicked()
    {
        string username = loginUsernameInput.text;
        string password = loginPasswordInput.text;

        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            loginErrorText.text = "cannot blank.";
            return;
        }

        
        if (!PlayerPrefs.HasKey(username))
        {
            loginErrorText.text = "not existS";
            return;
        }

        
        string savedPassword = PlayerPrefs.GetString(username);
        if (password != savedPassword)
        {
            loginErrorText.text = "password wrong";
            return;
        }

        
        loginErrorText.text = "login success";
        SceneManager.LoadScene("Level1");
        
    }

    
    private void OnRegisterButtonClicked()
    {
        string username = registerUsernameInput.text;
        string password = registerPasswordInput.text;
        string confirmPassword = registerConfirmPasswordInput.text;

        
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
        {
            registerErrorText.text = "not null";
            return;
        }

        if (password != confirmPassword)
        {
            registerErrorText.text = "not the same";
            return;
        }

        if (password.Length < 6)
        {
            registerErrorText.text = "longer than 6";
            return;
        }

        
        if (PlayerPrefs.HasKey(username))
        {
            registerErrorText.text = "existed";
            return;
        }

        
        PlayerPrefs.SetString(username, password);
        PlayerPrefs.Save();

        registerErrorText.text = "success！";
        Debug.Log($"user {username} regis success");

        
        Invoke("ShowLoginPanel", 1.5f);
    }

   
    private void ClearAllInputs()
    {
        loginUsernameInput.text = "";
        loginPasswordInput.text = "";
        registerUsernameInput.text = "";
        registerPasswordInput.text = "";
        registerConfirmPasswordInput.text = "";
    }

    
    private void ClearAllErrors()
    {
        loginErrorText.text = "";
        registerErrorText.text = "";
    }
}