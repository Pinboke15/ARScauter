using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class CpuImageManager : MonoBehaviour {
    [SerializeField]
    [Tooltip("The ARCameraManager which will produce frame events.")]
    ARCameraManager m_CameraManager;

    public ARCameraManager cameraManager {
        get => m_CameraManager;
        set => m_CameraManager = value;
    }

    [SerializeField]
    TextMeshProUGUI m_ImageInfo;
    public TextMeshProUGUI imageInfo {
        get => m_ImageInfo;
        set => m_ImageInfo = value;
    }

    private Texture2D m_CameraTexture;

    unsafe public byte[] CaptureLatestImage() {
        // カメラ画像の取得チェック
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage nativeImage)) {
            m_ImageInfo.text = "Failed Acquired Cpu Image.";
            return null;
        }

        // 画像情報の表示
        m_ImageInfo.text =
            $"Image info: \n\t Size:{nativeImage.width}x{nativeImage.height}\nPlaneCount:{nativeImage.planeCount}\nTime;{nativeImage.timestamp}\nFormat:{nativeImage.format}";

        // 画像のテクスチャフォーマットをRGBA32に設定
        var format = TextureFormat.RGBA32;
        if (m_CameraTexture == null || m_CameraTexture.width != nativeImage.width || m_CameraTexture.height != nativeImage.height) {
            m_CameraTexture = new Texture2D(nativeImage.width, nativeImage.height, format, false);
        }

        // 取得画像情報をY軸反転し、テクスチャフォーマットに変換するパラメータオブジェクトを生成
        var convParams = new XRCpuImage.ConversionParams(nativeImage, format, XRCpuImage.Transformation.MirrorY | XRCpuImage.Transformation.MirrorX);

        // テクスチャフォーマットに変換
        var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
        try {
            // XCpuImageの画像情報をTexture2D型に変換
            nativeImage.Convert(convParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        } finally {
            nativeImage.Dispose();
        }

        // 変更を反映
        m_CameraTexture.Apply();
        // 画像のバイナリデータを保持(外部から取得可能にする)
        return m_CameraTexture.EncodeToJPG();
    }
}
