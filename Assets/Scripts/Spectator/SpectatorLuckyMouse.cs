using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorLuckyMouse : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI m_currentRunner;
    [SerializeField] GameObject m_clientHousePrefab;
    [SerializeField] int m_totalPlayer;

    private List<ClientHouse> m_clientList = new List<ClientHouse>();
    private PathFollower m_pathfollower;
    private const float RADIUS = 100f;
    private bool m_isSetupDone = false;

    // Start is called before the first frame update
    void Start()
    {
        m_pathfollower = GetComponent<PathFollower>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    private Vector3 GetCirclePoint(int index)
    {
        float angle = index * (2f * Mathf.PI / m_totalPlayer);
        float x = RADIUS * Mathf.Cos(angle);
        float z = RADIUS * Mathf.Sin(angle);
        return new Vector3(x, 0, z);
    }

    public void SpawnClientHouse(Newtonsoft.Json.Linq.JArray listplayers)
    {
        if (m_isSetupDone) return;

        m_isSetupDone = true;
        int index = listplayers.Count - 1;
        
        while (index > 0)
        {
            if (listplayers[index] == null)
            {
                listplayers.RemoveAt(index);
            }
            else if (listplayers[index]["isSpectator"].ToString() == "1")
            {
                listplayers.RemoveAt(index);
            }
            index--;
        }

        m_totalPlayer = listplayers.Count;

        for (int i = 0; i < m_totalPlayer; i++)
        {
            var clientHouse = Instantiate(m_clientHousePrefab, GetCirclePoint(i), Quaternion.identity).GetComponent<ClientHouse>();
            clientHouse.SetupClientHouse(listplayers[i]);
            m_clientList.Add(clientHouse);
        }
    }
    public void SetNewRunner(Newtonsoft.Json.Linq.JObject currentRunner)
    {
        Debug.LogWarning(currentRunner["playerName"].ToString() + " is the target!");
        for (int i = 0; i < m_clientList.Count; i++)
        {
            if (currentRunner["playerName"].ToString() == m_clientList[i].ClientHouseName)
            {
                Debug.LogWarning(m_clientList[i].ClientHouseName + " has a mouse!");
                m_clientList[i].OnMouseGoingToHouse();
            }
            else
            {
                Debug.LogWarning(m_clientList[i].ClientHouseName + " wait for mouse!");
                m_clientList[i].OnMouseNotInHouse();
            }
            Debug.LogWarning(currentRunner["playerName"].ToString() + "|" + m_clientList[i].ClientHouseName + "|" + (m_clientList[i].ClientHouseName == currentRunner["playerName"].ToString()));
        }
        Debug.LogWarning("===========END OF ROUND==========");
    }
}
