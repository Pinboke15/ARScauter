using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;

public class WebManager : MonoBehaviour {
    [SerializeField]
    private string authrizationKey = "";
    [SerializeField]
    private string endpoint = "";

    private const string OCP_APIM_SUBSCRIPTION_KEY = "Ocp-Apim-Subscription-Key";
    private const string WEB_API = "face/v1.0/detect";

    public async Task<string> AnalyseFaceImage(byte[] image) {
        WWWForm webForm = new WWWForm();

        string requestParameters =
            "returnFaceId=true&returnFaceLandmarks=false" +
            "&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses," +
            "emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";

        using (UnityWebRequest webRequest = UnityWebRequest.Post($"{endpoint}{WEB_API}" + "?" + requestParameters, webForm)) {

            webRequest.SetRequestHeader("Content-Type", "application/octet-stream");
            webRequest.SetRequestHeader(OCP_APIM_SUBSCRIPTION_KEY, authrizationKey);

            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.uploadHandler = new UploadHandlerRaw(image);
            webRequest.uploadHandler.contentType = "application/octet-stream";

            await webRequest.SendWebRequest();

            long responseCode = webRequest.responseCode;
            if (responseCode != 200) {
                return null;
            }
            return webRequest.downloadHandler.text;
        }
    }
}
