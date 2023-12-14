using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Chat;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Unity.VisualScripting;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using PlayFab;
using PlayFab.ClientModels;

public class Lobby : MonoBehaviourPunCallbacks
{
    public Transform LoginPanel;
    public Transform SelectionPanel;
    public Transform CreateRoomPanel;
    public Transform InsideRoomPanel;
    public Transform ListRoomsPanel;
    public InputField roomNameInput;
    public InputField playerNameInput;
    private string playerName;
    public GameObject textPrefab;
    public Transform insideRoomPlayerList;
    public GameObject roomEntryPrefab;
    public Transform listRoomPanelContent;
    public Transform listRoomPanel;
    public GameObject startGameButton;
    public Transform chatPanel;

    private Dictionary<string, RoomInfo> cachedRoomList;

    public Chat chat;

    private void Start()
    {
        playerNameInput.text = playerName = string.Format("Player {0}", Random.Range(1, 1000000));

        cachedRoomList = new Dictionary<string, RoomInfo>();

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void StartGameClicked()
    {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel("Multiplayer");
    }

    public void OnJoinRandomRoomClicked()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        Debug.Log("Player joined the room");
        var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
        playerListEntry.GetComponent<Text>().text = newPlayer.NickName;
        playerListEntry.name = newPlayer.NickName;
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        Debug.Log("Player left the room");
        foreach (Transform child in insideRoomPlayerList)
        {
            if (child.name == otherPlayer.NickName)
            {
                Destroy(child.gameObject);
                break;
            }
        }
    }

    public void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
        foreach (var room in roomList)
        {
            if (!room.IsOpen || !room.IsVisible || room.RemovedFromList)
            {
                cachedRoomList.Remove(room.Name);
            }
            else
            {
                cachedRoomList[room.Name] = room;
            }
        }
    }

    public void LoginButtonClicked()
    {
        if (playerNameInput.text.Trim() != "")
        {

            PhotonNetwork.LocalPlayer.NickName = playerName = playerNameInput.text;
            PhotonNetwork.ConnectUsingSettings();
            UpdatePlayFabUsername(playerName);
        }
        else
        {
            Debug.Log("Player name invalid");
        }
    }

    void UpdatePlayFabUsername(string name)
    {
        UpdateUserTitleDisplayNameRequest request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = name
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, PlayFabUpdateUserTitleDisplayNameResult,
            PlayFabUpdateUserTitleDisplayNameError);
    }

    void PlayFabUpdateUserTitleDisplayNameResult(UpdateUserTitleDisplayNameResult updateUserTitleDisplayNameResult)
    {
        Debug.Log("Playfab - UserTitleDisplayName update.");
    }

    void PlayFabUpdateUserTitleDisplayNameError(PlayFabError updateUserTitleDisplayNameError)
    {
        Debug.Log("PlayFab - Error uccurred while updating UserTitleDisplayName: " +
                  updateUserTitleDisplayNameError.ErrorMessage);
    }
    

    public override void OnConnectedToMaster()
    {
        Debug.Log("We have connected to the master server!");
        ActivatePanel("Selection");
    }

    public void ActivatePanel(string panelName)
    {
        LoginPanel.gameObject.SetActive(false);
        SelectionPanel.gameObject.SetActive(false);
        CreateRoomPanel.gameObject.SetActive(false);
        InsideRoomPanel.gameObject.SetActive(false);
        ListRoomsPanel.gameObject.SetActive(false);
        chatPanel.gameObject.SetActive(false);

        if (panelName == LoginPanel.gameObject.name)
            LoginPanel.gameObject.SetActive(true);
        else if (panelName == SelectionPanel.gameObject.name)
            SelectionPanel.gameObject.SetActive(true);
        else if (panelName == CreateRoomPanel.gameObject.name)
            CreateRoomPanel.gameObject.SetActive(true);
        else if (panelName == InsideRoomPanel.gameObject.name)
            InsideRoomPanel.gameObject.SetActive(true);
        else if (panelName == ListRoomsPanel.gameObject.name)
            ListRoomsPanel.gameObject.SetActive(true);
        else if (panelName == chatPanel.gameObject.name)
            chatPanel.gameObject.SetActive(true);
    }

    public void DisconnectButtonClicked()
    {
        PhotonNetwork.Disconnect();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconnected from server");
        ActivatePanel("Login");
    }

    public void CreateARoom()
    {
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 4;
        roomOptions.IsVisible = true;
        PhotonNetwork.CreateRoom(roomNameInput.text, roomOptions);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created");
    }

    public override void OnCreatedRoomFailed(short returnCode, string message)
    {
        Debug.Log("Room Creation Failed");
    }

    public override void OnJoinedRoom()
    {
        var authenticationValues = new Photon.Chat.AuthenticationValues(PhotonNetwork.LocalPlayer.NickName);
        chat.userName = PhotonNetwork.LocalPlayer.NickName;
        chat.ChatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", authenticationValues);
        
        Debug.Log("Room joined");
        ActivatePanel("InsideRoom");
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);

        foreach (var player in PhotonNetwork.PlayerList)
        {
            var playerListEntry = Instantiate(textPrefab, insideRoomPlayerList);
            playerListEntry.GetComponent<Text>().text = player.NickName;
            playerListEntry.name = player.NickName;

        }
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        startGameButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        chat.ChatClient.Disconnect();
        
        Debug.Log("Room Left");
        ActivatePanel("CreateRoom");
        DestroyChildren(insideRoomPlayerList);
    }

    public void DestroyChildren(Transform parent)
    {
        foreach (Transform child in parent)
        {
            Destroy(child.gameObject);
        }
    }

    public void ListRoomsClicked()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Lobby");
        ActivatePanel("ListRooms");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        Debug.Log("Room Update: " + roomList.Count);
        
        UpdateCachedRoomList(roomList);

        foreach (var room in cachedRoomList)
        {
            var newRoomEntry = Instantiate(roomEntryPrefab, listRoomPanelContent);
            var newRoomEntryScript = newRoomEntry.GetComponent<RoomEntry>();
            newRoomEntryScript.roomName = room.Key;
            newRoomEntryScript.roomText.text =
                string.Format("[{0} - ({1}/{2})]", room.Key, room.Value.PlayerCount, room.Value.MaxPlayers);
        }
    }

    public void LeaveLobbyClicked()
    {
        PhotonNetwork.LeaveLobby();
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Lobby Left");
        DestroyChildren(listRoomPanelContent);
        DestroyChildren(insideRoomPlayerList);
        cachedRoomList.Clear();
        ActivatePanel("Selection");
    }
}
