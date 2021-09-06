using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using System.IO;

public class WebManager : MonoBehaviour {
    [SerializeField]
    private string authrizationKey = "";
    [SerializeField]
    string filePath = null;

    private const string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
    private const string ENDPOINT = "XXXX/face/v1.0/detect";

    byte[] imageData;

    async void Start() {
        await AnalyseLastImageCaptured();
    }

    async Task AnalyseLastImageCaptured() {
        WWWForm webForm = new WWWForm();

        using (UnityWebRequest webRequest = UnityWebRequest.Post(ENDPOINT, webForm)) {

            imageData = GetImageAsByteArray(filePath);

            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            webRequest.SetRequestHeader(OCP_APIM_SUBSCRIPTION_KEY, authrizationKey);

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.uploadHandler = new UploadHandlerRaw(imageData);
            webRequest.uploadHandler.contentType = "application/octet-stream";

            await webRequest.SendWebRequest();

            long responseCode = webRequest.responseCode;

            try {
                string resData = webRequest.downloadHandler.text;
                Debug.Log(resData);
            } catch (Exception e) {
                Debug.Log(e.Message);
            }
        }
    }

    private static byte[] GetImageAsByteArray(string path) {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
    }

}
