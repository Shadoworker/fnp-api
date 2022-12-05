using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Text;
using TMPro;

public class APIManager : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:1337/api/";

    public static APIManager instance { get; private set; }
	public string m_deeplinkURL;
	
    public TextMeshProUGUI m_dataText;

    public GameObject m_tokenBox;

    public TMP_InputField m_tokenField;

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

        // Debug.Log(output);

	}


    // Start is called before the first frame update
    void Start()
    {

        Credentials credentials = new Credentials();
        credentials.identifier = "shadow"; // Identifier will be set to PLAYER_ID from NFT-API
        credentials.password = "passer123";
        
        // Test Login to local api
        // StartCoroutine(Post("auth/local" , JsonUtility.ToJson(credentials) ));

        #if UNITY_EDITOR //Check if running a build or in editor
            m_tokenBox.SetActive(true);
        #else
            m_tokenBox.SetActive(false);
        #endif

    }


    public void OpenLoginWindow()
    {
        Application.OpenURL("https://connect.playtix.team/oauth2/aus7e5j3kfGHKetdl5d7/v1/authorize?client_id=0oa7e5jz4w9xy416F5d7&response_type=code&scope=openid&redirect_uri=http%3A%2F%2F192.168.1.12:1337%2Fapi%2Fl3v3l%2Fcallback&state=abc123");
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
        // "https://api.github.com/users?since=0&per_page=2"

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
        }
    }


 
}


[System.Serializable]
public class Credentials
{ 
    public string identifier;
    public string password;
}

 

