using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using UnityEngine.Networking;
using System;
using System.IO;
 using System.Text;

public class APIManager : MonoBehaviour
{
    private const string BASE_URL = "http://localhost:1337/api/";
    // Start is called before the first frame update
    void Start()
    {

        Credentials credentials = new Credentials();
        credentials.identifier = "shadow"; // Identifier will be set to PLAYER_ID from NFT-API
        credentials.password = "passer123";
        
        // Test Login to local api
        // StartCoroutine(Post("auth/local" , JsonUtility.ToJson(credentials) ));


        // Test getting page content
        StartCoroutine(GetRequest("https://www.example.com"));
        
    }


    public void OpenLoginWindow()
    {
        Application.OpenURL("https://kayfo.games/");
    }

    IEnumerator GetRequest(string uri)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(uri))
        {
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = uri.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
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

    private IEnumerator ProcessRequest(string uri)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(uri))
        { 

            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.isNetworkError)
            {
                Debug.Log("Error: " + request.error);
            }
            else
            {
                Debug.Log("Received: " + request.downloadHandler.text);
            }
        }
    }
 

    // Update is called once per frame
    void Update()
    {
        
    }
}


[System.Serializable]
public class Credentials
{ 
    public string identifier;
    public string password;
}

 

