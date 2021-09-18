using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;

public class FaceRecgnitionManager : MonoBehaviour {

    readonly Vector3 DELTA = new Vector3(0.15f, 0f, 0f);

    [SerializeField]
    private ARFaceManager arFaceMgr;
    [SerializeField]
    private CpuImageManager cpuImgMgr;
    [SerializeField]
    private WebManager webMgr;

    private FaceData recogFaceData;

    // For Debug
    [SerializeField]
    RawImage capturedRawImg;
    [SerializeField]
    TextMeshProUGUI recogResTxt;

    private void OnEnable() {
        arFaceMgr.facesChanged += OnFaceDetected;
    }

    private void OnDisable() {
        arFaceMgr.facesChanged -= OnFaceDetected;
    }

    private void OnFaceDetected(ARFacesChangedEventArgs args) {
        foreach (var placedObj in args.added) {
            if (placedObj.trackingState == TrackingState.Tracking) {
                placedObj.gameObject.SetActive(true);
                //�F�������X�V
                UpdateFaceInfo(placedObj);
                // ��̏������ɂ��炵�ĕ\��
                placedObj.gameObject.transform.position = placedObj.gameObject.transform.position + DELTA;
            }
        }

        foreach (var placedObj in args.updated) {
            if (placedObj.trackingState == TrackingState.Tracking) {
                placedObj.gameObject.SetActive(true);
                //�F�������X�V
                UpdateFaceInfo(placedObj);
                // ��̏������ɂ��炵�ĕ\��
                placedObj.gameObject.transform.position = placedObj.gameObject.transform.position + DELTA;
            } else {
                placedObj.gameObject.SetActive(false);
            }
        }

        foreach (var placedObj in args.removed) {
            placedObj.gameObject.SetActive(false);
        }
    }

    public async void OnRecogBtn() {
        // �J�����摜�擾
        byte[] imageData = cpuImgMgr.CaptureLatestImage();

        if (imageData != null) {
            // ��F��
            string recogRes = await webMgr.AnalyseFaceImage(imageData);
            // JSON���ʂ����
            recogRes = recogRes.Remove(0, 1);
            recogRes = recogRes.Substring(0, recogRes.Length - 1);
            if (recogRes == null || recogRes.Length == 0) {
                recogResTxt.text = "No Face Recognized...";
                recogFaceData = null;
            } else {
                //recogResTxt.text = recogRes;
                recogFaceData = JsonConvert.DeserializeObject<FaceData>(recogRes);
            }

            // DEBUG: �J�����摜��\��
            Texture2D capturedTexture = new Texture2D(640, 480);
            capturedTexture.LoadImage(imageData);
            capturedRawImg.texture = capturedTexture;
        }
    }

    private void UpdateFaceInfo(ARFace uiObj) {
        if (recogFaceData != null) {
            uiObj.gameObject.transform.Find("Canvas/Age").GetComponent<TextMeshProUGUI>().text =
                "Age: " + recogFaceData.faceAttributes.age.ToString();
            uiObj.gameObject.transform.Find("Canvas/Gender").GetComponent<TextMeshProUGUI>().text =
                "Gender: " + recogFaceData.faceAttributes.gender;
            uiObj.gameObject.transform.Find("Canvas/Glasses").GetComponent<TextMeshProUGUI>().text =
                "Glasses: " + recogFaceData.faceAttributes.glasses;
        }
    }
}
