using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Threading.Tasks;
using System.IO;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;

public class WebManager : MonoBehaviour {
    [SerializeField]
    private string authrizationKey = "";
    [SerializeField]
    CpuImageManager cpuImageManager;

    // FOR DEBUG
    [SerializeField]
    TextMeshProUGUI recogText;
    [SerializeField]
    RawImage inputImage;

    private const string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
    private const string ENDPOINT = "<endpoint>/face/v1.0/detect";

    public async void OnClick() {
        await AnalyseLastImageCaptured();
    }

    async Task AnalyseLastImageCaptured() {
        WWWForm webForm = new WWWForm();

        string requestParameters =
            "returnFaceId=true&returnFaceLandmarks=false" +
            "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
            "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

        using (UnityWebRequest webRequest = UnityWebRequest.Post(ENDPOINT + "?" + requestParameters, webForm)) {

            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            webRequest.SetRequestHeader(OCP_APIM_SUBSCRIPTION_KEY, authrizationKey);

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            byte[] imageData = cpuImageManager.imageBinary;
            // DEBUG Image
            Texture2D capturedTexture = new Texture2D(640, 480);
            capturedTexture.LoadImage(imageData);
            inputImage.texture = capturedTexture;
            //
            webRequest.uploadHandler = new UploadHandlerRaw(imageData);
            webRequest.uploadHandler.contentType = "application/octet-stream";

            await webRequest.SendWebRequest();

            long responseCode = webRequest.responseCode;
            if (responseCode != 200) {
                recogText.text = $"HTTP Status: {responseCode}";
                return;
            }

            try {
                if (webRequest.downloadHandler != null) {
                    string resData = webRequest.downloadHandler.text;
                    resData = resData.Remove(0, 1);
                    resData = resData.Substring(0, resData.Length - 1);
                    var faceData = JsonConvert.DeserializeObject<FaceData>(resData);
                    recogText.text = "GENDER: " + faceData.faceAttributes.gender;
                } else {
                    recogText.text = "Recognition is failed.";
                }
            } catch (Exception e) {
                recogText.text = "Exception: " + e.Message;
            }
        }
    }

    private static byte[] GetImageAsByteArray(string path) {
        FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read);
        BinaryReader binaryReader = new BinaryReader(fileStream);
        return binaryReader.ReadBytes((int)fileStream.Length);
    }

}
