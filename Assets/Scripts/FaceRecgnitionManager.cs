using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

public class FaceRecgnitionManager : MonoBehaviour {
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
                Vector3 pos = placedObj.gameObject.transform.position;
                Vector3 delta = new Vector3(0.2f, 0, 0);
                placedObj.gameObject.transform.position = pos + delta;
            }
        }

        foreach (var placedObj in args.updated) {
            if (placedObj.trackingState == TrackingState.Tracking) {
                placedObj.gameObject.SetActive(true);
                // äÁÇÃè≠Çµâ°Ç…Ç∏ÇÁÇµÇƒï\é¶
                Vector3 pos = placedObj.gameObject.transform.position;
                Vector3 delta = new Vector3(0.2f, 0, 0);
                placedObj.gameObject.transform.position = pos + delta;
            } else {
                placedObj.gameObject.SetActive(false);
            }
        }

        foreach (var placedObj in args.removed) {
            placedObj.gameObject.SetActive(false);
        }
    }
}
