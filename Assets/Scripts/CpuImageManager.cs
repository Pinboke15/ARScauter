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
        // �J�����摜�̎擾�`�F�b�N
        if (!cameraManager.TryAcquireLatestCpuImage(out XRCpuImage nativeImage)) {
            m_ImageInfo.text = "Failed Acquired Cpu Image.";
            return null;
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
        // �摜�̃o�C�i���f�[�^��ێ�(�O������擾�\�ɂ���)
        return m_CameraTexture.EncodeToJPG();
    }
}
