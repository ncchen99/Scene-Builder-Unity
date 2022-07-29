using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Linq;

public class Widget : MonoBehaviour
{
	public GameObject[] obj = new GameObject[10];
    public string[] name = {"Cube", "Sphere"};
    public GameObject sphere;

    void Start()
    {
        string url = "http://127.0.0.1:5000/";
        // A correct website page.
        StartCoroutine(GetRequest(url + "data"));
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
                    Assets result = Assets.CreateFromJSON(webRequest.downloadHandler.text);
                    for(int i = 0; i < result.data.Length; i++){
                        Asset asset = result.data[i];
                        if (name.Contains(asset.name))
                        {
                            int index = System.Array.IndexOf(name, asset.name);
                            GameObject objectAsset = Instantiate(obj[index], new Vector3(asset.position.x , asset.position.y, asset.position.z*-1), new Quaternion(0,0,0,0)) as GameObject;
                            Vector3 objectScale = objectAsset.transform.localScale;
                            // Sets the local scale of game object
                            objectAsset.transform.localScale = new Vector3(objectScale.x, objectScale.y ,objectScale.z);
                        }
                    }
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}



[System.Serializable]
public class Assets {  
    public Asset[] data;
    public static Assets CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Assets>(jsonString);
    }
}

[System.Serializable]
public class Asset{
    public string name;
    public Coordinate position;
}

[System.Serializable]
public class Coordinate {
    public int x;
    public int y;
    public int z;
}
