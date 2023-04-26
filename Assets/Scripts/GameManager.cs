using DG.Tweening;
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
    [SerializeField] RectTransform _endGamePanel;
    [SerializeField] RectTransform _replayBtn;
    Sequence _sequence;
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
            textWating.text = "Sẵn sàng để bắt đầu trò chơi!";
            startButton.SetActive(true);
        } 
        else
        {
            textWating.text = "Chờ chủ phòng bắt đầu trò chơi!";
            startButton.SetActive(false);
        }
        _sequence = DOTween.Sequence();
        _sequence.Append(textWating.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 0.1f).SetEase(Ease.InOutBounce))
            .Append(startButton.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, -537f), 0.1f).SetEase(Ease.InOutBounce));
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
        textRunning.text = "Chú chuột đang chạy ở nhà của " + playerRun["playerName"].ToString() + "!!!";
    }

    public void PlayerRuning()
    {
        runningObject.SetActive(false);
    }

    public void ShowEndGameScreen(Newtonsoft.Json.Linq.JObject playerWin)
    {
        runningObject.SetActive(false);
        endGameObject.SetActive(true);
        StartCoroutine(SetPlayerImage(playerWin["avatar"].ToString()));

        var resultText = (playerWin["id"].ToString() == SocketClient.instance.clientId) ? "Bạn đã thắng !!!" : playerWin["playerName"].ToString() + " đã thắng !!!";
        textPlayerWin.text = resultText;

        _endGamePanel.DOScale(1, 0.2f).SetEase(Ease.InOutBounce);
        _replayBtn.DOAnchorPos(new Vector2(0f, -537f), 0.1f);
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
