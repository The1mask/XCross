using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class PhotonConnect : MonoBehaviourPunCallbacks
{
    #region Private Serializable Fields


    #endregion


    #region Private Fields


    /// <summary>
    /// This client's version number. Users are separated from each other by gameVersion (which allows you to make breaking changes).
    /// </summary>
    string gameVersion = "1";


    #endregion


    #region MonoBehaviour CallBacks


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during early initialization phase.
    /// </summary>
    void Awake()
    {
        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.AutomaticallySyncScene = true;
    }


    /// <summary>
    /// MonoBehaviour method called on GameObject by Unity during initialization phase.
    /// </summary>
    void Start()
    {
        OnClickConnectToMaster();
    }


    #endregion


    #region Public Methods


    public void OnClickConnectToMaster()
    {

        //Settings (all optional and only for tutorial purpose)
        PhotonNetwork.OfflineMode = false;           //true would "fake" an online connection
        PhotonNetwork.NickName = "PlayerName" + Random.Range(1,10);       //to set a player name
        PhotonNetwork.AutomaticallySyncScene = true; //to call PhotonNetwork.LoadLevel()
        PhotonNetwork.GameVersion = "v1";            //only people with the same game version can play together

        //PhotonNetwork.ConnectToMaster(ip,port,appid); //manual connection
        if (!PhotonNetwork.OfflineMode)
            PhotonNetwork.ConnectUsingSettings();           //automatic connection based on the config file in Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("Connected to Master!");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log(cause);
    }

    public void OnClickConnectToRoom()
    {
        if (!PhotonNetwork.IsConnected)
            return;

        //PhotonNetwork.CreateRoom("Peter's Game 1"); //Create a specific Room - Error: OnCreateRoomFailed
        //PhotonNetwork.JoinRoom("Peter's Game 1");   //Join a specific Room   - Error: OnJoinRoomFailed  
        PhotonNetwork.JoinRandomRoom();               //Join a random Room     - Error: OnJoinRandomRoomFailed  
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        //no room available
        //create a room (null as a name means "does not matter")
        PhotonNetwork.CreateRoom(null, new RoomOptions { MaxPlayers = 20 });
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        Debug.Log(message);
        base.OnCreateRoomFailed(returnCode, message);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();


        Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " | Players In Room: " + PhotonNetwork.CurrentRoom.PlayerCount + " | RoomName: " + PhotonNetwork.CurrentRoom.Name + " Region: " + PhotonNetwork.CloudRegion);
        //if(PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name != "Network")
        //    PhotonNetwork.LoadLevel("Network");
        if (PhotonNetwork.IsMasterClient && SceneManager.GetActiveScene().name != "Local")
            PhotonNetwork.LoadLevel("Local");
    }
    #endregion


}

