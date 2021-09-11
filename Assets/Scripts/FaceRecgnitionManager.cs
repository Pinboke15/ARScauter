using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class FaceRecgnitionManager : MonoBehaviour {

    readonly Vector3 DELTA = new Vector3(0.15f, 0f, 0f);

    [SerializeField]
    private ARFaceManager arFaceMgr;

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
                // äÁÇÃè≠Çµâ°Ç…Ç∏ÇÁÇµÇƒï\é¶
                placedObj.gameObject.transform.position = placedObj.gameObject.transform.position + DELTA;
            }
        }

        foreach (var placedObj in args.updated) {
            if (placedObj.trackingState == TrackingState.Tracking) {
                placedObj.gameObject.SetActive(true);
                // äÁÇÃè≠Çµâ°Ç…Ç∏ÇÁÇµÇƒï\é¶
                placedObj.gameObject.transform.position = placedObj.gameObject.transform.position + DELTA;
            } else {
                placedObj.gameObject.SetActive(false);
            }
        }

        foreach (var placedObj in args.removed) {
            placedObj.gameObject.SetActive(false);
        }
    }
}
