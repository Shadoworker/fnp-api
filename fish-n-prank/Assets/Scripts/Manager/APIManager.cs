using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Net;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
using TMPro;
using Kayfo;

public class APIManager : MonoBehaviour
{

    private const string BASE_URL = "http://localhost:1337/api/";

    public static APIManager instance { get; private set; }
	public string m_deeplinkURL;
	
    public TextMeshProUGUI m_dataText;

    public GameObject m_tokenBox;

    public TMP_InputField m_tokenField;

    public List<ServerSO> m_serversSO;

    int m_currentServerIndex = 0;

    public Slider m_serverSlider;

    fnpFish choosenFish = null;

    private void Awake()
	{
    	if (instance == null)
    	{
        	instance = this;
        	Application.deepLinkActivated += onDeepLinkActivated;
        	if (!String.IsNullOrEmpty(Application.absoluteURL))
        	{
            	// Cold start and Application.absoluteURL not null so process Deep Link.
                onDeepLinkActivated(Application.absoluteURL);
        	}
        	// Initialize DeepLink Manager global variable.
        	else m_deeplinkURL = "[none]";
        	DontDestroyOnLoad(gameObject);
    	}
    	else
    	{
        	Destroy(gameObject);
    	}
	}

	private void onDeepLinkActivated(string url)
	{
    	// Update DeepLink Manager global variable, so URL can be accessed from anywhere.
    	m_deeplinkURL = url;

        // Decode the URL to determine action.
        // In this example, the app expects a link formatted like this:
        // unitydl://mylink?scene1
        string urlDecoded = WWW.UnEscapeURL(url);

    	string data = urlDecoded.Split("?"[0])[1];
    	
        m_dataText.text = data;

        SavePlayerID(data);

        // Debug.Log(output);

	}

    
    public void SelectSlider()
    {
        m_currentServerIndex = (int)m_serverSlider.value;
    }

    // Start is called before the first frame update
    void Start()
    {
 
        // Test Login to local api
        // StartCoroutine(Post("auth/local" , JsonUtility.ToJson(credentials) ));

        if(m_tokenBox)

            #if UNITY_EDITOR //Check if running a build or in editor
                m_tokenBox.SetActive(true);
            #else
                m_tokenBox.SetActive(false);
            #endif

    }


    public void OpenLoginWindow()
    {
        ServerSO s = m_serversSO[m_currentServerIndex];

        Application.OpenURL(s.m_connectUrl+"/v1/authorize?client_id=0oa7e5jz4w9xy416F5d7&response_type=code&scope=openid&redirect_uri="+s.m_fnpApiUrl+"l3v3l/callback&state=abc123");
    }

    public void LoadPlayerData()
    {
        string id_token = m_tokenField.text;
        if(id_token == "") return;


        string _path = "l3v3l/who?id_token="+id_token;

        StartCoroutine(DisplayUserData(_path));
    }
  
    IEnumerator Post(string _path, string _bodyJsonString)
    {
        var request = new UnityWebRequest(BASE_URL+_path, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(_bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        // Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
        }
    }

    IEnumerator Get(string _path)
    {
        // "https://api.github.com/users?since=0&per_page=2"

        var request = new UnityWebRequest(BASE_URL+_path, "GET");
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        // Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Received: " + request.downloadHandler.text);
        }
    }

    
    IEnumerator DisplayUserData(string _path)
    {

        var request = new UnityWebRequest(BASE_URL+_path, "GET");
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        // Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error: " + request.error);
            m_dataText.text = "VERIFIEZ VOTRE TOKEN";

        }
        else
        {
            m_dataText.text = request.downloadHandler.text;
            
            SavePlayerID(request.downloadHandler.text);
        }
    }


    IEnumerator IGetFish(string _path)
    {

        var request = new UnityWebRequest(BASE_URL+_path, "GET");
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        // Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            string _jsonString = request.downloadHandler.text;

            choosenFish = JsonUtility.FromJson<fnpFish>(_jsonString);

            Debug.Log(_jsonString);

        }
    }

    public void GetFish(string _path)
    {
        StartCoroutine(IGetFish(_path));
    }

    IEnumerator IUpdatePlayerResources(string _path, string _bodyJsonString)
    {
        var request = new UnityWebRequest(BASE_URL+_path, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(_bodyJsonString);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        // Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            Debug.Log("Resources Updated: " + request.downloadHandler.text);
        }
    }

    public void UpdatePlayerResources(string _path, string _bodyJsonString)
    {
        StartCoroutine(IUpdatePlayerResources(_path, _bodyJsonString));
    }


   IEnumerator IGetPlayerResources(string _path)
    {
        var request = new UnityWebRequest(BASE_URL+_path, "GET");
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        // Debug.Log("Status Code: " + request.responseCode);

        if (request.isNetworkError)
        {
            Debug.Log("Error: " + request.error);
        }
        else
        {
            string _jsonString = request.downloadHandler.text;

            StateManager.Instance.m_persistentPlayerResources.Set(_jsonString);

        }
    }


    public void GetPlayerResources()
    {
        string _path = "";
        StartCoroutine(IGetPlayerResources(_path));
    }

    void SavePlayerID(string _jsonString)
    {
        
        fnpPlayerInfo _fnpPlayerInfo = JsonUtility.FromJson<fnpPlayerInfo>(_jsonString);
        
        string playerID = _fnpPlayerInfo.player_id;

        StateManager.Instance.m_persistentPlayerID.Set(playerID);

        GrantAccess();
    }

    void GrantAccess()
    {
        StateManager.Instance.GrantAccess();
    }

 
}


[System.Serializable]
public class Credentials
{ 
    public string identifier;
    public string password;
}

[System.Serializable]
public class fnpPlayerInfo
{
    public string   sub;
    public int      ver;
    public string   iss;
    public string   aud;
    public int      iat;
    public int      exp;
    public string   jti;
    public string[] amr;
    public string   idp;
    public int      auth_time;
    public string   player_id;
    public string   at_bash;
}
 

[System.Serializable]
public class fnpFish
{
    public string  name;
    public string size;
    public float weight;
}

[System.Serializable]
public class fnpUpdateBody
{
    public string  player_id;
    public string game_id;
    public List<fnpResourceBody> resources = new List<fnpResourceBody>();
}
 
[System.Serializable]
public class fnpResourceBody
{
    public string  resource_id;
    public int amount;
}

