using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    private static SoundManager m_instance;
    public static SoundManager Instance => m_instance;
    public enum SoundType
    {
        // Menu Sounds
        MenuBackground,
        JoinRoom,
        Click,
        // Ingame Sounds
        IngameBackground,
        Mouse,
        Win,
        Lose,                
    }
    [SerializeField] AudioSource[] m_audioSources;    

    // Start is called before the first frame update
    void Start()
    {
        if (m_instance == null)
        {
            m_instance = this;
        }
        DontDestroyOnLoad(m_instance);
    }

    public void PlaySound(SoundType sound)
    {
        m_audioSources[(int)sound].Play();
    }
    public void StopSound(SoundType sound)
    {
        m_audioSources[(int)sound].Stop();
    }
    public void StopAllSounds()
    {
        for (int i = 0; i < m_audioSources.Length; i++)
        {
            m_audioSources[i].Stop();
        }
    }
}
