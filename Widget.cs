using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;


public class Widget : MonoBehaviour
{
	public GameObject cube;

    void Start()
    {
        // A correct website page.
        StartCoroutine(GetRequest("http://127.0.0.1:5000/data"));
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
                        if (asset.name.Equals("Cube")){
                            GameObject objectAsset = Instantiate(cube, new Vector3(asset.position.x /25 ,asset.position.y/25,asset.position.z/-25), new Quaternion(0,0,0,0)) as GameObject;
                            Vector3 objectScale = objectAsset.transform.localScale;
                            // Sets the local scale of game object
                            objectAsset.transform.localScale = new Vector3(objectScale.x*2, objectScale.y*2 ,objectScale.z*2);
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
