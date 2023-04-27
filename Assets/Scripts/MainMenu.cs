using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UIElements;

public class MainMenu : MonoBehaviour
{
	public static MainMenu instance;

    string[] playerNames = new string[] { "Alice", "Bob", "Charlie", "Peter", 
        "Phan Cao", "Yến", "Ngoãn", "Thi","Đạt", "Thuận", "Tuệ", "Díu" };

    [SerializeField]
	private GameObject homeScreen;		
	[SerializeField]
	private GameObject joinRoomScreen;
	[SerializeField]
	private GameObject lobbyScreen;
	[SerializeField]
	private GameObject hostButtonJoinGame;
	[SerializeField]
	private TMPro.TextMeshProUGUI RoomId;

	[SerializeField] TMPro.TextMeshProUGUI m_notificationText;

	[SerializeField]
	private TMPro.TMP_InputField inputRoomId;

	public static string deepLinkZaloApp = "https://zalo.me/s/512606311101007876/";
	public string userAppId = "";
	public string userAvatar = "https://h5.zdn.vn/static/images/avatar.png";
	public string playerName = "";
	public string roomId = "";
	public string isHost = "0";
	public string gender = "0";
	public string isSpectator = "0";
	//public Dictionary<string, GameObject> listPlayers;
	public Dictionary<string, Texture2D> listPlayerAvatars;

	//private const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
	private const string CHARS = "0123456789";
	public int length = 6;

	TouchScreenKeyboard myKeyboard;
	private TMPro.TextMeshProUGUI currentInput;
	// end

	public GameObject[] characters;
	public int selectedCharacter = 0;

    private void Awake()
    {
		if (instance == null)
			instance = this;
		else if (instance != this)
			Destroy(gameObject);
	}
    private void Start()
    {
        //SocketClient.instance.OnConnectWebsocket();
        SoundManager.Instance.PlaySound(SoundManager.SoundType.MenuBackground);
		listPlayerAvatars = new Dictionary<string, Texture2D>();
		StartCoroutine(WaitingReceiver());
	}

    public string Generate()
    {
        string result = "";
        System.Random rand = new System.Random();
        while (result.Length < 6)
        {
            result += CHARS[rand.Next(0, CHARS.Length)];
        }
        return result;
        //if (IsUnique(result))
        //{
        //    return result;
        //}
        //else
        //{
        //    return Generate();
        //}
    }
    IEnumerator WaitingReceiver()
    {
        //roomId = "roomid";// test already have room id
        Debug.Log("  SocketClient.IS_FIRST_JOIN =================  " + SocketClient.IS_FIRST_JOIN);
        yield return new WaitForSeconds(0.5f);
        if (roomId != "" && SocketClient.IS_FIRST_JOIN)
        {
            homeScreen.SetActive(false);       
            joinRoomScreen.SetActive(true);
            lobbyScreen.SetActive(false);

            inputRoomId.text = roomId;
            JoinRoom();
            gameObject.GetComponent<JoinGameScreen>().SetTextInputRoomId(roomId);
            UserJoinRoom();
        }
        else
        {
            homeScreen.SetActive(true);
            joinRoomScreen.SetActive(false);
            lobbyScreen.SetActive(false);
        }        
    }
    private void Update()
    {
        if (myKeyboard != null && myKeyboard.status == TouchScreenKeyboard.Status.Done)
        {
            currentInput.text = myKeyboard.text;

            if (joinRoomScreen.activeSelf)
            {
                inputRoomId.text = myKeyboard.text;
                roomId = myKeyboard.text;
                Debug.Log("Input roomId: " + roomId);
            }

            myKeyboard = null;

        }
       
    }
    public void PlayerNameChange(TMPro.TextMeshProUGUI inputPlayerName)
    {
        playerName = inputPlayerName.text;
    }
    public void InputRoomId(TMPro.TextMeshProUGUI inputRoomId)
    {
        roomId = inputRoomId.text;
    }
    public void OnSelectedInput(TMPro.TextMeshProUGUI _currentInput)
    {
        currentInput = _currentInput;
        //TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        if (myKeyboard == null)
        {
            myKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
        }


    }
    public void CheckTheHost()
    {
        if (isHost == "1")
        {
            hostButtonJoinGame.SetActive(true);
        }
        else
        {
            hostButtonJoinGame.SetActive(false);
        }
    }
    public void JoinRoom()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);

        if (joinRoomScreen.activeSelf)
        {
            roomId = inputRoomId.text;
        }
        RoomId.text = roomId;

        if (playerName.Length <= 1)
        {
            playerName = "anonymous";
            //int rand = UnityEngine.Random.Range(0, playerNames.Length);
            //playerName = playerNames[rand];
        }

        SocketClient.instance.OnConnectWebsocket();
        //createRoomScreen.SetActive(true);
        //homeScreen.SetActive(false);
        //joinRoomScreen.SetActive(false);
        //CheckTheHost();
    }
    public void ShowPlayerJoinRoom(string playerName)
    {
        lobbyScreen.GetComponent<LobbyScreen>().ShowPlayerJoinRoom(playerName);
    }
    public void ShowTotalPlayers(int player)
    {
        lobbyScreen.GetComponent<LobbyScreen>().SetTotalPlayer(player.ToString());
    }
    public void ShowLobby()
    {    
        homeScreen.SetActive(false);
        joinRoomScreen.SetActive(false);
        lobbyScreen.SetActive(true);
        CheckTheHost();
    }
    public void JoinTheGame()
    {
        //SceneManager.LoadScene("Game");
        SocketClient.instance.OnGotoGame();
    }
    public void GotoGame()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);

        if (isSpectator == "0")
        {
            SceneManager.LoadScene("Game");
        }
        else
        {
            SceneManager.LoadScene("Spectator");
        }        
    }
    public void HostCreateNewRoom()
    {        
        roomId = Generate();
        //RoomId.text = "Room ID : " +  roomId;
        isHost = "1";
        JoinRoom();
        lobbyScreen.SetActive(true);
        homeScreen.SetActive(false);
    }
    public void UserJoinRoom()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.JoinRoom);

        roomId = inputRoomId.text;
        joinRoomScreen.SetActive(true);
        homeScreen.SetActive(false);
        isHost = "0";
    }
    public void SpectatorJoinRoom()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.JoinRoom);
        Debug.Log(" ===== SpectatorJoinRoom==== ");
        roomId = inputRoomId.text;
        joinRoomScreen.SetActive(true);
        homeScreen.SetActive(false);
        isHost = "0";
        isSpectator = "1";
    }
    public void ShowFailScreen(string message)
    {
        //failMessage.text = message;
        //homeScreen.SetActive(false);
        //joinRoomScreen.SetActive(false);
        //createRoomScreen.SetActive(false);
        //failJoinRoomScreen.SetActive(true);

        //m_notificationText.text = message;
        m_notificationText.text = "Mã phòng " + RoomId.text + " không tồn tại";
        m_notificationText.gameObject.SetActive(true);

    }

    public void FailToJoinRoom()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);

        homeScreen.SetActive(true);
        joinRoomScreen.SetActive(false);        
        lobbyScreen.SetActive(false);
    }


    public void AddPlayerJoinRoomByAvatar(Texture2D avatar, string playerID)
    {
        if (avatar != null)
        {
            listPlayerAvatars.Add(playerID, avatar);
            lobbyScreen.GetComponent<LobbyScreen>().SetAvatarForPlayer(avatar, playerID);
        }
    }
    public void ResetAvatarList()
    {
        listPlayerAvatars = new Dictionary<string, Texture2D>();

    }
    public void RemovePlayerJoinRoomByAvatar(string playerID)
    {
        lobbyScreen.GetComponent<LobbyScreen>().RemoveAvatarForPlayer(playerID);
    }

    public void CopyToClipboard()
    {
        GUIUtility.systemCopyBuffer = roomId;
    }
    public void BackToMainMenu()
    {
        homeScreen.SetActive(true);
        joinRoomScreen.SetActive(false);        
        lobbyScreen.SetActive(false);
        lobbyScreen.GetComponent<LobbyScreen>().ResetAvatarList();
        SocketClient.instance.OnCloseConnectSocket();
    }

    public void ShareLinkToInvite()
    {
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);
        JavaScriptInjected.instance.SendRequestShareRoom();
    }
    public void NextCharacter()
	{
		characters[selectedCharacter].SetActive(false);
		selectedCharacter = (selectedCharacter + 1) % characters.Length;
		characters[selectedCharacter].SetActive(true);
        //if(selectedCharacter!=0)
        //      {
        //	Starts.SetActive(false);
        //	Unlock.SetActive(true);

        //}else
        //      {
        //	Starts.SetActive(true);
        //	Unlock.SetActive(false);
        //}
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);
    }

    public void PreviousCharacter()
	{
        SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);

        characters[selectedCharacter].SetActive(false);
		selectedCharacter--;
		if (selectedCharacter < 0)
		{
			selectedCharacter += characters.Length;
		}
		characters[selectedCharacter].SetActive(true);
		//if (selectedCharacter != 0)
		//{
		//	Starts.SetActive(false);
		//	Unlock.SetActive(true);

		//}
		//else
		//{
		//	Starts.SetActive(true);
		//	Unlock.SetActive(false);
		//}
	}
	public void Unlocking()
    {

#if UNITY_EDITOR
        SoundManager.Instance.PlaySound(SoundManager.SoundType.JoinRoom);

        //PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene("Game");
#endif
		//SocketClient.instance.OnGotoGame();
	}
		
	public void StartGame()
	{
		SocketClient.instance.OnConnectWebsocket();
		//SocketClient.instance.OnGotoGame();
        SoundManager.Instance.PlaySound(SoundManager.SoundType.JoinRoom);

        //PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene("Game");
	}
}
