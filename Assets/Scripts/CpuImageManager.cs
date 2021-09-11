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
        // �J�����摜�̎擾�`�F�b�N
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage nativeImage)) {
            return;
        }

        // �摜���̕\��
        m_ImageInfo.text =
            $"Image info: \n\t Size:{nativeImage.width}x{nativeImage.height}\nPlaneCount:{nativeImage.planeCount}\nTime;{nativeImage.timestamp}\nFormat:{nativeImage.format}";

        // �摜�̃e�N�X�`���t�H�[�}�b�g��RGBA32�ɐݒ�
        var format = TextureFormat.RGBA32;
        if (m_CameraTexture == null || m_CameraTexture.width != nativeImage.width || m_CameraTexture.height != nativeImage.height) {
            m_CameraTexture = new Texture2D(nativeImage.width, nativeImage.height, format, false);
        }

        // �擾�摜����Y�����]���A�e�N�X�`���t�H�[�}�b�g�ɕϊ�����p�����[�^�I�u�W�F�N�g�𐶐�
        var convParams = new XRCpuImage.ConversionParams(nativeImage, format, XRCpuImage.Transformation.MirrorY | XRCpuImage.Transformation.MirrorX);
        
        // �e�N�X�`���t�H�[�}�b�g�ɕϊ�
        var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
        try {
            // XCpuImage�̉摜����Texture2D�^�ɕϊ�
            nativeImage.Convert(convParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
        } finally {
            nativeImage.Dispose();
        }

        // �ύX�𔽉f
        m_CameraTexture.Apply();
        // �L���v�`�������摜��\��
        m_RawCameraImage.texture = m_CameraTexture;
        // �摜�̃o�C�i���f�[�^��ێ�(�O������擾�\�ɂ���)
        imageBinary = m_CameraTexture.EncodeToJPG();
    }

}
