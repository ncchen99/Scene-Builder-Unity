using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Widget : MonoBehaviour
{

    public GameObject plane;
    public string url = "https://ros-api.paia-arena.com";
    public string mapId = "f8cf4994-a7bb-4e8f-a97c-c164a2cb2141_1671513500_1671513500";
    public string login = "/api/v1/auth/login";
    public string map = "/api/v1/map/"; 
    void Start()
    {
        // A correct website page.

        Dictionary<string, string> body = new Dictionary<string, string>();
        body.Add("email", "admin@test.com");
        body.Add("password", "123456");
        

        // Building b = new Building { email="admin@test.com", password="123456"};
        // string postData = JsonConvert.SerializeObject(b);

        StartCoroutine(PostRequest(this.url + this.login, JsonConvert.SerializeObject(body)));
    }

    IEnumerator PostRequest(string url,string param)
    {
        Debug.Log(param);
        using (UnityWebRequest webRequest = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(param));
            // Post raw data
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("content-type", "application/json; charset=UTF-8");

            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
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
                    var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
                    Debug.Log(this.url + this.map + this.mapId);
                    yield return StartCoroutine(GetRequest(this.url + this.map + this.mapId, values["access_token"]));
                    break;
                    // error CS1061: 'UnityWebRequest' does not contain a definition for 'downloadHanlder' and no accessible extension method 'downloadHanlder' accepting a first argument of type 'UnityWebRequest' could be found (are you missing a using directive or an assembly reference?)
                    
                default:
                    Debug.Log("IDK");
                    break;
            }
        }
    }
    IEnumerator GetRequest(string url, string authentication)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            if(authentication != null) 
                webRequest.SetRequestHeader("Authorization", "Bearer " + authentication);
            // Request and wait for the desired page.
            yield return webRequest.SendWebRequest();

            string[] pages = url.Split('/');
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
                    Debug.Log(webRequest.downloadHandler.text);
                    if(url.Contains("s3.amazonaws.com")) {
                        Assets result = Assets.CreateFromJSON(webRequest.downloadHandler.text);
                        int height = result.info.height;
                        int width = result.info.width;
                        Debug.Log(string.Format("Height: {0}, Width:{1}",height , width) );
                        int[] data = result.data;
                        for(int i = 0; i < height; i++) {
                            for(int j = 0; j < width; j++) {
                                if( data[i * width + j] > 0){
                                    int x = (j - height / 2);
                                    int y = (i - width / 2);
                                    GameObject objectAsset = Instantiate(this.plane, new Vector3(x , 0, y), new Quaternion(0,0,0,0)) as GameObject;
                                    Vector3 objectScale = objectAsset.transform.localScale;
                                    objectAsset.transform.localScale = new Vector3(objectScale.x, objectScale.y ,objectScale.z);
                                }
                            }
                        }
                        
                    } else {
                        var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(webRequest.downloadHandler.text);
                        yield return StartCoroutine(GetRequest(values["url"], null));
                    }
                    // {"url":"https://ros-map.s3.amazonaws.com/f8cf4994-a7bb-4e8f-a97c-c164a2cb2141/1676902823/1676902823.json?AWSAccessKeyId=AKIA4KMWMRFBY67EPKPI&Signature=du46ACCE8jEpK3l0UR5%2BbnL7tCo%3D&Expires=1676976769"}
                    // Assets result = Assets.CreateFromJSON(webRequest.downloadHandler.text);
                    // for(int i = 0; i < result.data.Length; i++){
                    //     Asset asset = result.data[i];
                    //     if (name.Contains(asset.name))
                    //     {
                    //         int index = System.Array.IndexOf(name, asset.name);
                    //         
                    //         Vector3 objectScale = objectAsset.transform.localScale;
                    //         // Sets the local scale of game object
                    //         objectAsset.transform.localScale = new Vector3(objectScale.x, objectScale.y ,objectScale.z);
                    //     }
                    // }
                    // Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }
    }
}



[System.Serializable]
public class Assets {  
    public int[] data;
    public Header header;
    public Info info;
    public static Assets CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Assets>(jsonString);
    }
}

[System.Serializable]
public class Info {
    public int height;
    public int width;
    public Dictionary<string, int> map_load_time;
    public Dictionary<string, Dictionary<string, double>> origin;
    public double resolution;
}

[System.Serializable]
public class Header {
    public string frame_id;
    public Dictionary<string, int> stamp;
}
