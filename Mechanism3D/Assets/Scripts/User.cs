using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User 
{
    public string name;
    public string password;

    public User(string name, string password)
    {
        this.name = name;
        this.password = password;
        PlayerPrefs.SetString("name", name);
        PlayerPrefs.SetString("password", password);
        LoginManager.Instance.users.Add(this);
    }

}
