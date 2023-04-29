using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientHouse : MonoBehaviour
{
    [SerializeField] string m_clientID;
    [SerializeField] string m_clientName;
    [SerializeField] Light m_housePointLight;
    [SerializeField] Transform m_fontHousePoint;
    [SerializeField] TMPro.TextMeshPro m_clientHouseName;    
    
    public Transform FontHousePoint => m_fontHousePoint;
    public string ClientHouseName
    {
        get => m_clientName;
        set => m_clientName = value;
    }
    public string ClientID
    {
        get => m_clientID;
        set => m_clientID = value;
    }

    // Start is called before the first frame update
    void Start()
    {        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnMouseGoingToHouse()
    {
        m_housePointLight.gameObject.SetActive(true);
    }
    public void OnMouseNotInHouse()
    {
        m_housePointLight.gameObject.SetActive(false);
    }
    public void SetupClientHouse(Newtonsoft.Json.Linq.JToken clientInfo)
    {
        transform.LookAt(Vector3.zero);
        
        m_clientID = clientInfo["userAppId"].ToString();
        m_clientName = clientInfo["playerName"].ToString();

        m_clientHouseName.text = "NAME: " + m_clientName;
        m_clientHouseName.transform.rotation = Quaternion.LookRotation(m_clientHouseName.transform.position - Camera.main.transform.position);
    }
}
