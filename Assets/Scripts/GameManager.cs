using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject m_hamster;

    [Header("Start panel")]
    [SerializeField] GameObject startGameObject;
    [SerializeField] TMPro.TextMeshProUGUI textWating;
    [SerializeField] GameObject startButton;
    
    [Header("Ingame panel")]
    [SerializeField] GameObject runningObject;
    [SerializeField] TMPro.TextMeshProUGUI textRunning;
    
    [Header("Endgame panel")]
    [SerializeField] GameObject endGameObject;
    [SerializeField] UnityEngine.UI.RawImage playerPicture;
    [SerializeField] TMPro.TextMeshProUGUI textPlayerWin;
    [SerializeField] RectTransform _endGamePanel;
    [SerializeField] RectTransform _replayBtn;

    private Sequence _sequence;

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

    #region Ingame process
    public void StartGame()
    {
        SocketClient.instance.OnStartGame();
    }
    public void ReadyPlayGame()
    {
        startGameObject.SetActive(false);
    }
    public void ShowOtherPlayerRunning(Newtonsoft.Json.Linq.JObject playerRun)
    {
        runningObject.SetActive(true);
        textRunning.text = "Chú chuột đang chạy ở nhà của " + playerRun["playerName"].ToString() + "!!!";
    }
    public void SetPlayerRunning(bool isFinalRun)
    {
        runningObject.SetActive(false);
        m_hamster.GetComponent<PathFollower>().ActivePath(isFinalRun);
    }
    public void SetSpectatorCurrentRunning()
    {
        
    }
    #endregion

    #region UI
    public void ShowEndGameScreen(Newtonsoft.Json.Linq.JObject playerWin)
    {
        runningObject.SetActive(false);
        endGameObject.SetActive(true);
        StartCoroutine(SetPlayerImage(playerWin["avatar"].ToString()));              

        if (playerWin["id"].ToString() == SocketClient.instance.clientId)
        {            
            _endGamePanel.Find("WinnerAura").gameObject.SetActive(true);
            textPlayerWin.text = "Bạn đã được chọn !!!";
        }
        else
        {            
            _endGamePanel.Find("WinnerAura").gameObject.SetActive(false);
            textPlayerWin.text = playerWin["playerName"].ToString() + "là người được chọn !!!";
        }

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
    #endregion
}
