using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject startGameObject;
    [SerializeField] TMPro.TextMeshProUGUI textWating;
    [SerializeField] GameObject startButton;
    // player running
    [SerializeField] GameObject runningObject;
    [SerializeField] TMPro.TextMeshProUGUI textRunning;
    // end game
    [SerializeField] GameObject endGameObject;
    [SerializeField] UnityEngine.UI.RawImage playerPicture;
    [SerializeField] TMPro.TextMeshProUGUI textPlayerWin;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

    }

    // Start is called before the first frame update
    void Start()
    {        
        runningObject.SetActive(false);
        endGameObject.SetActive(false);
        startGameObject.SetActive(true);
        if (SocketClient.instance.isHost)
        {
            textWating.text = "Ready to start the game!";
            startButton.SetActive(true);
        } 
        else
        {
            textWating.text = "Wating for host start the game!";
            startButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void StartGame()
    {
        SocketClient.instance.OnStartGame();
    }

    public void ReadyPlayGame()
    {
        startGameObject.SetActive(false);
    }

    public void CurrentPlayerRuning(Newtonsoft.Json.Linq.JObject playerRun)
    {
        runningObject.SetActive(true);
        textRunning.text = playerRun["playerName"].ToString() + " is running!!!";
    }

    public void PlayerRuning()
    {
        runningObject.SetActive(false);
    }

    public void ShowEndGameScreen(Newtonsoft.Json.Linq.JObject playerWin)
    {
        runningObject.SetActive(false);        
        textPlayerWin.text = playerWin["playerName"].ToString() + " is winning !!!";
        StartCoroutine(SetPlayerImage(playerWin["avatar"].ToString()));            
        endGameObject.SetActive(true);
    }

    public void BackToMM()
    {
        SoundManager.Instance.StopAllSounds();
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator SetPlayerImage(string imageUrl)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Texture2D textureImageUrl = ((DownloadHandlerTexture)request.downloadHandler).texture;
            playerPicture.texture = textureImageUrl;
        }
        else
        {
            Debug.Log(request.error);
        }
    }
}
