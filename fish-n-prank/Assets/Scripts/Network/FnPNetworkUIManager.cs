using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.Events;
using System;

[DisallowMultipleComponent]
[RequireComponent(typeof(NetworkManager))]
public class FnPNetworkUIManager : MonoBehaviour
{

    FnPNetworkManager m_networkManager;

    [SerializeField]
    int m_offsetX;
    [SerializeField]
    int m_offsetY;

    [SerializeField]
    GameObject m_connectionUI = null;
    [SerializeField]
    GameObject m_ingameUI = null;

    [SerializeField]
    Button m_joinLocalButton = null;
    [SerializeField]
    Button m_joinRemoteButton = null;
    [SerializeField]
    Button m_hostGameButton = null;
    [SerializeField]
    Button m_hostAndClientButton = null;
    [SerializeField]
    Button m_disconnectButton = null;

    void Awake()
    {
        m_networkManager = GetComponent<FnPNetworkManager>();
    }

    void Start()
    {
        if (!m_networkManager)
        {
            Debug.LogError("FnPNetworkConnectionUI: m_networkManager must be referenced");
            return;
        }

        // disable host option on mobile devices
        if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
        {
            m_hostGameButton.gameObject.SetActive(false);
            m_hostAndClientButton.gameObject.SetActive(false);
        }
        else
        {
            m_hostGameButton.onClick.AddListener(StartServer);
            m_hostAndClientButton.onClick.AddListener(StartHost);
        }

        m_joinLocalButton.onClick.AddListener(StartLocalClient);
        m_joinRemoteButton.onClick.AddListener(StartRemoteClient);
    }

    private void StartLocalClient()
    {
        m_networkManager.networkAddress = "localhost";
        m_networkManager.StartClient();
    }

    private void StartRemoteClient()
    {
        m_networkManager.networkAddress = "mango.confiote.com";
        m_networkManager.StartClient();
    }

    private void StartHost()
    {
        m_networkManager.StartHost();
    }

    private void StartServer()
    {
        m_networkManager.StartServer();
    }

    private void OnGUI()
    {
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        StopButtons();
    }

    void StartButtons()
    {
        m_connectionUI.SetActive(true);
        m_ingameUI.SetActive(false);
        m_disconnectButton.gameObject.SetActive(false);

        if (!NetworkClient.active)
        {
            m_joinLocalButton.enabled = true;
            m_joinRemoteButton.enabled = true;
            m_hostGameButton.enabled = true;
            m_hostAndClientButton.enabled = true;
        }
        else
        {
            // TODO: connecting
            m_joinLocalButton.enabled = false;
            m_joinRemoteButton.enabled = false;
            m_hostGameButton.enabled = false;
            m_hostAndClientButton.enabled = false;
        }
    }

    void StatusLabels()
    {
        m_connectionUI.SetActive(false);
        m_ingameUI.SetActive(true);
        m_disconnectButton.gameObject.SetActive(true);

        GUILayout.BeginArea(new Rect(10 + m_offsetX, 40 + m_offsetY, 215, 9999));

        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            GUILayout.Label($"<b>Host</b>: running via {Transport.active}");
        }
        // server only
        else if (NetworkServer.active)
        {
            GUILayout.Label($"<b>Server</b>: running via {Transport.active}");
        }
        // client only
        else if (NetworkClient.isConnected)
        {
            GUILayout.Label($"<b>Client</b>: connected to {m_networkManager.networkAddress} via {Transport.active}");
        }
        GUILayout.EndArea();

    }

    void StopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host"))
            {
                m_networkManager.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client"))
            {
                m_networkManager.StopClient();
            }
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            if (GUILayout.Button("Stop Server"))
            {
                m_networkManager.StopServer();
            }
        }
    }
}
