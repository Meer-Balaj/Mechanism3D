using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance { get; private set; }
    private const string SAVE_SEPARATOR = "-";

    public List<User> users = new List<User>();
    
    [SerializeField] private string username;
    [SerializeField] private string password;

    [SerializeField] private TMP_InputField usernameInputField;
    [SerializeField] private TMP_InputField passwordInputField;

    [SerializeField] private Button loginButton;
    [SerializeField] private Button registerButton;
    [SerializeField] private Button quitButton;
    
    private void Awake()
    {
        Instance = this;
        User admin = new User("admin", "admin");
        users.Add(admin);

        loginButton.onClick.AddListener(() => {
            Login();

        });
        
        registerButton.onClick.AddListener(() => {
            Register();

        });

        quitButton.onClick.AddListener(() => {
            Application.Quit();
        });
    }


    private void Login()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;

        

        if (users.Count == 0)
        {
            Debug.LogError("There are no users");
            return;
        }

        if (password == "")
        {
            Debug.LogError("Enter Password");
            return;
        }
        if (username == "")
        {
            Debug.LogError("Enter Username");
            return;
        }

        foreach(User user in users)
        {
            string loginName = PlayerPrefs.GetString("name");
            string loginPassword = PlayerPrefs.GetString("password");
            if(user.name == username && user.password == password)
            {
                if (loginName == username && loginPassword == password)
                {
                    Loader.Load(Loader.Scene.GameScene);
                    return;
                }
            }
        }
        
        
        /*foreach (User entry in users)
        {
            if (entry.name == username.ToLower() && entry.password == password)
            {
                Loader.Load(Loader.Scene.GameScene);
                return;
            }
        }*/

        Debug.LogError("wrong credentials");
    }
    
    private void Register()
    {
        username = usernameInputField.text;
        password = passwordInputField.text;
        
        foreach (User user in users)
        {
            string loginName = PlayerPrefs.GetString("name");
            string loginPassword = PlayerPrefs.GetString("password");
            if (user.name == username && user.password == password)
            {
                if (loginName == username && loginPassword == password)
                {
                    // user already exists
                    Debug.LogError("user already registered");
                    return;
                }
            }
        }
        
        User newUser = new User(username, password);
        
    }


    private void Save()
    {
        string[] contents = new string[] { "" };

    }
}
