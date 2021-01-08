using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class CameraParallax : MonoBehaviour
{
    public Camera mainCamera;
    public Camera nearOthorgraphicCamera;
    public Camera farCamera;
    public Camera nearCamera;
    public Camera backGroundCamera;
    public float furthestClipPlane;
    public GameObject staticBackgrounds;

    void OnEnable()
    {
        InitCameras();
    }

    void LateUpdate()
    {
        UpdateCameras();
        UpdateStaticBGs();
    }


    public void InitCameras()
    {
        if (backGroundCamera != null)
        {
            backGroundCamera.transform.localPosition = Vector3.zero;
            backGroundCamera.transform.rotation = Quaternion.identity;
            backGroundCamera.transform.localScale = Vector3.one;
            backGroundCamera.orthographic = true;
            backGroundCamera.depth = -3;
        }

        if (farCamera != null)
        {
            farCamera.transform.localPosition = Vector3.zero;
            farCamera.transform.rotation = Quaternion.identity;
            farCamera.transform.localScale = Vector3.one;
            farCamera.orthographic = false;
            farCamera.depth = -2;
            farCamera.transparencySortMode = TransparencySortMode.Orthographic;
        }

        if (mainCamera != null)
        {
            mainCamera.orthographic = true;
            mainCamera.clearFlags = CameraClearFlags.Nothing;
            mainCamera.depth = -1;
        }

        if (nearOthorgraphicCamera != null)
        {
            nearOthorgraphicCamera.transform.localPosition = Vector3.zero;
            nearOthorgraphicCamera.transform.rotation = Quaternion.identity;
            nearOthorgraphicCamera.transform.localScale = Vector3.one;
            mainCamera.orthographic = true;
            mainCamera.clearFlags = CameraClearFlags.Nothing;
            mainCamera.depth = -1;
        }

        if (nearCamera != null)
        {
            nearCamera.transform.localPosition = Vector3.zero;
            nearCamera.transform.rotation = Quaternion.identity;
            nearCamera.transform.localScale = Vector3.one;
            nearCamera.orthographic = false;
            nearCamera.clearFlags = CameraClearFlags.Depth;
            nearCamera.depth = 0;
            nearCamera.transparencySortMode = TransparencySortMode.Orthographic;
        }
    }

    public void UpdateCameras()
    {
        if (mainCamera == null || farCamera == null || nearCamera == null || nearOthorgraphicCamera == null || backGroundCamera == null) return;

        // orthoSize
        float a = mainCamera.orthographicSize;
        // distanceFromOrigin
        float b = Mathf.Abs(mainCamera.transform.position.z);

        //change clipping planes based on main camera z-position
        mainCamera.farClipPlane = b;
        nearOthorgraphicCamera.farClipPlane = b;
        backGroundCamera.nearClipPlane = b;
        backGroundCamera.farClipPlane = furthestClipPlane;
        farCamera.nearClipPlane = b;
        farCamera.farClipPlane = furthestClipPlane;
        nearCamera.farClipPlane = b;
        nearCamera.nearClipPlane = mainCamera.nearClipPlane;

        //update field fo view for parallax cameras and size for background camera
        float fieldOfView = Mathf.Atan(a / b) * Mathf.Rad2Deg * 2f;
        nearCamera.fieldOfView = farCamera.fieldOfView = fieldOfView;
        backGroundCamera.orthographicSize = a;
        nearOthorgraphicCamera.orthographicSize = a;
    }

    //update vị trí của static background theo vị trí của maincamera
    public void UpdateStaticBGs()
    {
        Vector3 mainCamPos= transform.position;
        Vector3 newPos = new Vector3(mainCamPos.x, mainCamPos.y, staticBackgrounds.transform.position.z);
        staticBackgrounds.transform.position = newPos;
    }

}
