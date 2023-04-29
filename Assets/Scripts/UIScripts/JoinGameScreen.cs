using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UIElements
{
    public class JoinGameScreen : Screen
    {
        [SerializeField] Slider m_spectatorSlider;
        [SerializeField] TMP_InputField m_inputField;        
        [SerializeField] TextMeshProUGUI m_notificationText;   

        private string m_roomIDEntered;
        public string RoomIDEntered => m_roomIDEntered;
        private TouchScreenKeyboard m_touchScreenKeyboard;

        [SerializeField] RectTransform _gameTitle, _inputCanvas, _exitBtn, _acceptBtn;

        Sequence _sequence;
        // Start is called before the first frame update
        void Start()
        {
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            _sequence.Append(_gameTitle.DOAnchorPos(new Vector2(0, -300f), 0.1f).SetEase(Ease.InOutBounce))
                .Append(_inputCanvas.DOScale(1, 0.2f).SetEase(Ease.InOutBounce))
                .Append(_exitBtn.DOAnchorPos(new Vector2(-275f, 200f), 0.1f))
                .Append(_acceptBtn.DOAnchorPos(new Vector2(275f, 200f), 0.1f));

        }
        void OnDestroy()
        {
            _sequence.Kill();
        }

        void OnEnable()
        {
            _sequence.Restart();

        }
        // Update is called once per frame
        void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (m_touchScreenKeyboard != null)
                {

                    if (m_touchScreenKeyboard.status == TouchScreenKeyboard.Status.Done ||
                        m_touchScreenKeyboard.status == TouchScreenKeyboard.Status.Canceled)
                    {
                        Debug.Log(" m_touchScreenKeyboard.text nullllllllllllllllll  ");
                        m_touchScreenKeyboard = null;
                    }
                    else if (m_touchScreenKeyboard.status == TouchScreenKeyboard.Status.Visible)
                    {

                        m_inputField.text = m_touchScreenKeyboard.text;
                        Debug.Log("m_inputField ================  " + m_inputField.text);
                    }
                }
            }
        }

        public void OnShowNotificationMessage(string notificationText = "Mã phòng không tồn tại*")
        {
            m_notificationText.text = notificationText;
            m_notificationText.gameObject.SetActive(true);
        }
        //public void OnEnteredRoomID(TextMeshProUGUI inputRoomId)
        //{
        //    if (Application.isMobilePlatform)
        //        m_inputField.text = inputRoomId.text;
        //}
        public void OnInputFieldSelected(TextMeshProUGUI _currentInput)
        {

            if (Application.isMobilePlatform)
            {
                if (m_touchScreenKeyboard == null)
                {

                    m_touchScreenKeyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
                }
            }

        }

        public void OnInputValueChanged(TextMeshProUGUI _currentInput)
        {
            m_inputField.caretPosition = _currentInput.text.Length;
        }
        public void SetTextInputRoomId(string _roomId)
        {
            m_inputField.text = _roomId;
        }
        public void OnJoinRoom()
        {
            MainMenu.instance.isSpectator = m_spectatorSlider.value.ToString();
            MainMenu.instance.JoinRoom();
        }
        public void OnUseQRScan()
        {
            
        }

        public void OnExitScreen()
        {
            MainMenu.instance.FailToJoinRoom();
        }
    }
}