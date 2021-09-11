using System;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    RawImage m_RawCameraImage;
    public RawImage rawCameraImage {
        get => m_RawCameraImage;
        set => m_RawCameraImage = value;
    }

    [SerializeField]
    TextMeshProUGUI m_ImageInfo;
    public TextMeshProUGUI imageInfo {
        get => m_ImageInfo;
        set => m_ImageInfo = value;
    }

    public byte[] imageBinary;

    void OnEnable() {
        if (m_CameraManager != null) {
            m_CameraManager.frameReceived += OnCameraFrameReceived;
        }
    }

    void OnDisable() {
        if (m_CameraManager != null) {
            m_CameraManager.frameReceived -= OnCameraFrameReceived;
        }
    }

    void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs) {
        UpdateCameraImage();
    }

    Texture2D m_CameraTexture;

    unsafe void UpdateCameraImage() {
        // カメラ画像の取得チェック
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage nativeImage)) {
            return;
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
        // キャプチャした画像を表示
        m_RawCameraImage.texture = m_CameraTexture;
        // 画像のバイナリデータを保持(外部から取得可能にする)
        imageBinary = m_CameraTexture.EncodeToJPG();
    }

}
