using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    public static MenuController Instance { set; get; }
    public GameObject Client;
    private Client c;
    public Text getUserName;
    private string _userName;
    public void Start()
    {
        Instance = this;
    }

    public void GoToLocal()
    {
        SceneManager.LoadScene("Local");
    }
    public void GoToServer()
    {
        string hostAdres = "127.0.0.1";

        try
        {
            _userName = getUserName.text;
            c = Client.GetComponent<Client>();
            Debug.Log("Connect1" + _userName);
            c.ConnectToServer(_userName);
            Debug.Log("Connect" + c);
        }
        catch(Exception e)
        {
            Debug.Log(e.Message);
        }
    }
    public void SendMessage()
    {
        _userName = getUserName.text;
        c.SendMessage(11);
    }
}
