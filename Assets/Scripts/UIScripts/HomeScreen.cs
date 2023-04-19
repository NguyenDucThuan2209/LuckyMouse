using TMPro;
using UnityEngine;
using DG.Tweening;

namespace UIElements
{
    public class HomeScreen : Screen
    {
        [SerializeField] RectTransform _gameTitle, _joinBtn, _createBtn;
        Sequence _sequence;
        // Start is called before the first frame update
        void Start()
        {
            _sequence = DOTween.Sequence();
            _sequence.SetAutoKill(false);
            _sequence.Append(_gameTitle.DOAnchorPos(new Vector2(0, -500f), 0.1f).SetEase(Ease.InOutBounce))
                .Append(_joinBtn.DOAnchorPos(new Vector2(275f, 200f), 0.1f))
                .Append(_createBtn.DOAnchorPos(new Vector2(-275f, 200f), 0.1f));

            /*_sequence.Append(_gameTitle.DOAnchorPos(new Vector2(0, -500f), 0.1f).SetEase(Ease.InOutBounce).OnComplete(() =>
            {
                Debug.Log("hnhp----- OnEnable()");
                _joinBtn.DOAnchorPos(new Vector2(275f, 200f), 0.1f);
                _createBtn.DOAnchorPos(new Vector2(-275f, 200f), 0.1f);

            }));*/
        }

        void OnDestroy()
        {
            _sequence.Kill();
        }

        void OnEnable()
        {
            _sequence.Restart();
            
        }

        void Update()
        {

        }

        public void CreateRoom()
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);
            MainMenu.instance.HostCreateNewRoom();
        }
        public void JoinRoom()
        {
            SoundManager.Instance.PlaySound(SoundManager.SoundType.Click);
            MainMenu.instance.UserJoinRoom();
        }
        public void SpectatorJoinRoom()
        {
            MainMenu.instance.SpectatorJoinRoom();
        }
    }
}