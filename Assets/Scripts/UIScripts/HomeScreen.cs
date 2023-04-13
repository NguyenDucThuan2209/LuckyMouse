using TMPro;
using UnityEngine;

namespace UIElements
{
    public class HomeScreen : Screen
    {
        
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
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