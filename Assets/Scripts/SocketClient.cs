using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
//using System.Net.WebSockets;
using Newtonsoft.Json.Linq;

// Use plugin namespace
using NativeWebSocket;
using Random = UnityEngine.Random;
using System.Linq;
using UnityEngine.SceneManagement;
//using System.Globalization;

public class SocketClient : MonoBehaviour
{

    public static SocketClient instance;

    public delegate void ReceiveAction(string message);
    public event ReceiveAction OnReceived;

    //public ClientWebSocket webSocket = null;
    public WebSocket webSocket;

    [SerializeField]
    private string url = "";
    static string baseUrl = "ws://192.168.1.39";
    static string HOST = "8083";

    //static string baseUrl = "wss://rlgl2-api.brandgames.vn";
    //static string HOST = "8082";

    public string ROOM = "";
    public string clientId = "";

    public string playerJoinName = "";
    public int currentPlayerJoined = 0;

    public bool isHost = false;
    public bool isSpectator = false;

    public static bool IS_FIRST_JOIN = true;

    [SerializeField]
    private GameObject playerPrefab;
    public GameObject player = null;
    public JArray players;

    [SerializeField]
    private GameObject spectatorPrefab;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        //Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
    }
    void Start()
    {
        //player = GameObject.Find("Hamster");
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (webSocket!=null)
            webSocket.DispatchMessageQueue();
#endif
    }

    async void OnDestroy()
    {
        if (webSocket != null)
        {
            //webSocket.Dispose();
           await webSocket.Close();
        }

        Debug.Log("WebSocket closed.");
    }

    public void OnConnectWebsocket()
    {
        url = baseUrl + ":" + HOST;
        Connect(url);
        //OnReceived = ReceiveSocket;
    }
    async void Connect(string uri)
    {
        try
        {
            webSocket = new WebSocket(uri);

            Debug.Log(" webSocket connect ===========================================  " + webSocket.State);
            webSocket.OnOpen += () =>
            {
                Debug.Log("WS OnOpen  ");
                OnRequestRoom();
            };
            webSocket.OnMessage += (bytes) =>
            {
                // Reading a plain text message
                var message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("Received OnMessage! (" + bytes.Length + " bytes) " + message);
                ReceiveSocket(message);
            };

            webSocket.OnError += (string errMsg) =>
            {
                Debug.Log("WS error: " + errMsg);
            };

            // Add OnClose event listener
            webSocket.OnClose += (WebSocketCloseCode code) =>
            {
                Debug.Log("WS closed with code: " + code.ToString());

                if(code.ToString() != "Normal")
                {
                    OnDisconnect();
                }
            };
            // Keep sending messages at every 0.3s
            //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);
            //Receive();
            await webSocket.Connect();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }

    }

    private async void Send(string message)
    {
        var encoded = Encoding.UTF8.GetBytes(message);
        var buffer = new ArraySegment<Byte>(encoded, 0, encoded.Length);


        //await webSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        if (webSocket.State == WebSocketState.Open)
        {
            // Sending bytes
            await webSocket.Send(encoded);

            // Sending plain text
            //await webSocket.SendText(message);
        }
    }
    private void Receive()
    {
        while (webSocket.State== WebSocketState.Open)
        {
            webSocket.OnMessage += (byte[] msg) =>
            {

                Debug.Log("WS received message:  " + System.Text.Encoding.UTF8.GetString(msg));
                string message = Encoding.UTF8.GetString(msg);

                Debug.Log("session response : " + message);
                if (OnReceived != null) OnReceived(message);

            };

            // Add OnError event listener
            webSocket.OnError += (string errMsg) =>
            {
                Debug.Log("WS error: " + errMsg);
            };

            // Add OnClose event listener
            webSocket.OnClose += (WebSocketCloseCode code) =>
            {
                Debug.Log("WS closed with code: " + code.ToString());
            };

        }
    }

    void ReceiveSocket(string message)
    {
        JObject data = JObject.Parse(message);

        switch (data["event"].ToString())
        {
            case "roomDetected":
                ROOM = data["room"].ToString();
                clientId = data["clientId"].ToString();
                //OnJoinRoom();
                OnJoinLobbyRoom();

                break;
            case "failJoinRoom":

                MainMenu.instance.ShowFailScreen(data["message"].ToString());
                break;

            case "joinLobbyRoom":

                IS_FIRST_JOIN = false;
                MainMenu.instance.ShowLobby();
                players = JArray.Parse(data["players"].ToString());

                currentPlayerJoined = players.Count;
                Debug.Log(" playerName  join room  " + data["playerName"].ToString());

                int countUserPlay = 0;
                int countSpectator = 0;
                // for new player
                if (data["clientId"].ToString() == clientId && player == null)
                {

                    for (int i = 0; i < players.Count; i++)
                    {
                        isHost = data["isHost"].ToString() == "1";

                        if (players[i]["isSpectator"].ToString() == "0")
                        {
                            countUserPlay++;
                            StartCoroutine(LoadAvatarImage(players[i]["avatar"].ToString(), players[i]["id"].ToString()));
                        }
                        else
                        {
                            countSpectator++;
                        }
                    }
                }
                // for old player
                else
                {

                    if (data["isSpectator"].ToString() == "0")
                        StartCoroutine(LoadAvatarImage(data["avatar"].ToString(), data["clientId"].ToString()));
                }

                MainMenu.instance.ShowPlayerJoinRoom(data["playerName"].ToString());
                MainMenu.instance.ShowTotalPlayers(players.Count);

                break;
            case "gotoGame":
                MainMenu.instance.GotoGame();
                break;
            case "joinRoom":

                players = JArray.Parse(data["players"].ToString());
                player = GameObject.Find("Hamster");

                player.GetComponent<PathFollower>().speed = Mathf.Max(700 / (30f / players.Count), 100f); 
                //OnStartGame();

                break;
            case "startGame":
                SoundManager.Instance.StopSound(SoundManager.SoundType.MenuBackground);
                SoundManager.Instance.PlaySound(SoundManager.SoundType.IngameBackground);
                Debug.Log("  startGame =================  " + data);
                GameManager.instance.ReadyPlayGame();
                if (isSpectator)
                {
                    // code spectator screen here

                }
                else
                {
                    if (isHost)
                    {
                        OnRequestNextRun();
                    }

                }
                break;

            case "responseNextRun":
                Debug.Log("  requestNextRun data ==========  " + data);
                JObject playerRun = JObject.Parse(data["playerRun"].ToString());
                Debug.Log("  requestNextRun playerRun  ========== " + playerRun["playerName"].ToString());
                if (clientId == data["playerRunId"].ToString())
                {
                    GameManager.instance.PlayerRuning();
                    bool isFinalRun = data["isFinalRun"].ToString() == "1";
                    player.GetComponent<PathFollower>().ActivePath(isFinalRun);
                    SoundManager.Instance.PlaySound(SoundManager.SoundType.Mouse);
                }
                else
                {
                    SoundManager.Instance.StopSound(SoundManager.SoundType.Mouse);
                    GameManager.instance.CurrentPlayerRuning(playerRun);
                }
                break;

            case "playerDie":
                Debug.Log("  playerDie data ==========  " + data);

                if (clientId == data["clientId"].ToString())
                {

                    OnCloseConnectSocket();
                }

                break;

            case "endGame":
                Debug.Log("  endGame data ==========  " + data);
                players = JArray.Parse(data["players"].ToString());
                JObject playerWin = JObject.Parse(data["playerWin"].ToString());
                GameManager.instance.ShowEndGameScreen(playerWin);

                SoundManager.Instance.StopAllSounds();

                // Always play Win sound when game ended
                SoundManager.Instance.PlaySound(SoundManager.SoundType.Win);

                break;
            
            case "newRuningId":                
                // check new run 
                string checkNewRunning = data["playerRunningId"].ToString();

                if (checkNewRunning != "" && checkNewRunning == clientId)
                {
                    OnRequestNextRun();
                }
                Debug.Log("NEW RUNNING ID" + checkNewRunning);
                break;

            case "playerLeaveRoom":
                string playerLeaveId = data["clientId"].ToString();

                for (int i = 0; i < players.Count; i++)
                {
                    //Debug.Log(" players player leave ==   " + players[i].ToString());
                    if (playerLeaveId == players[i]["id"].ToString())
                    {
                        if (player == null)
                        {
                            if (players[i]["isSpectator"].ToString() == "0")
                            {
                                MainMenu.instance.RemovePlayerJoinRoomByAvatar(playerLeaveId);
                            }                            
                        }
                        players.RemoveAt(i);
                        Debug.Log(" players playerLeaveRoom 222222222222222  " + playerLeaveId);

                    }
                }
                // check new host 
                string checkNewHost = data["newHost"].ToString();
                
                if (checkNewHost != "" && checkNewHost == clientId)
                {
                    isHost = true;

                    if (player != null)
                    {
                        Debug.Log(" client is new host -----------    " );
                        if(data["playerRunningId"].ToString() != "")
                        {
                            OnRequestNextRun();
                        }
                    } 
                    else
                    {
                        Debug.Log(" client is new lobby host ---=====  ");

                        MainMenu.instance.isHost = "1";
                        MainMenu.instance.CheckTheHost();
                    }
                    
                }
                break;

            default:
                break;
        }
    }


    public void OnRequestRoom()
    {
        Debug.Log("  MainMenu.instance.isSpectator OnRequestRoom =================  " + MainMenu.instance.isSpectator);
        string room = MainMenu.instance.roomId;
        JObject jsData = new JObject();
        jsData.Add("meta", "requestRoom");
        jsData.Add("playerLen", 8);
        jsData.Add("room", room);
        jsData.Add("host", MainMenu.instance.isHost);
        jsData.Add("isSpectator", MainMenu.instance.isSpectator);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }

    public void OnJoinLobbyRoom()
    {
        Debug.Log("  MainMenu.instance.isSpectator OnJoinLobbyRoom   " + MainMenu.instance.isSpectator);
        Debug.Log("  MainMenu.instance.gender gender   " + MainMenu.instance.gender);
        string playerName = MainMenu.instance.playerName;

        //if (playerName.Length <= 1)
        //{
        //    playerName = "anonymous";
        //}

        JObject jsData = new JObject();
        jsData.Add("meta", "joinLobby");
        jsData.Add("room", ROOM);
        jsData.Add("isHost", MainMenu.instance.isHost);
        jsData.Add("gender", MainMenu.instance.gender);
        jsData.Add("isSpectator", MainMenu.instance.isSpectator);
        jsData.Add("playerName", playerName);
        jsData.Add("userAppId", MainMenu.instance.userAppId);
        jsData.Add("avatar", MainMenu.instance.userAvatar);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnGotoGame()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "gotoGame");
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
    public void OnJoinRoom()
    {
        Debug.Log(" OnJoinRoom ==================  " );
        string playerName = MainMenu.instance.playerName;

        //if (playerName.Length <= 1 )
        //{
        //    playerName = "anonymous";
        //}

        //clientPosStart = RandomPosition();

        JObject jsData = new JObject();
        jsData.Add("meta", "join");
        jsData.Add("room", ROOM);
        jsData.Add("isHost", MainMenu.instance.isHost);
        jsData.Add("playerName", playerName);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }
    public void OnStartGame()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "startGame");
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
 
    public void OnRequestNextRun()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "requestNextRun");
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData).ToString());
    }
  
    public void OnPlayerDie()
    {
        Debug.Log(" ======================== OnPlayerDie() ======================================");
        JObject jsData = new JObject();
        jsData.Add("meta", "playerDie");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnPlayerWin()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "playerWin");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnEndGame()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "endGame");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public void OnLeaveRoom()
    {
        JObject jsData = new JObject();
        jsData.Add("meta", "leave");
        jsData.Add("clientId", clientId);
        jsData.Add("room", ROOM);
        Send(Newtonsoft.Json.JsonConvert.SerializeObject(jsData));
    }

    public async void OnCloseConnectSocket()
    {
        clientId = "";
        ROOM = "";

        await webSocket.Close();
    }
    public void OnDisconnect()
    {
        clientId = "";
        ROOM = "";
        //if (player)
        //{
            SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
        //}
    }

    IEnumerator LoadAvatarImage(string imageUrl, string playerID)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
            Texture2D textureImageUrl = null;
            MainMenu.instance.AddPlayerJoinRoomByAvatar(textureImageUrl, playerID);
        }
        else
        {
            Texture2D textureImageUrl = ((DownloadHandlerTexture)request.downloadHandler).texture;
            // use the texture here
            MainMenu.instance.AddPlayerJoinRoomByAvatar(textureImageUrl, playerID);
        }
    }

}
