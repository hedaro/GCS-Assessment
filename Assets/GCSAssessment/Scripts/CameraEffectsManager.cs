using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffectsManager : MonoBehaviour
{
    [SerializeField]
    float ShakeStrength = 0.1f;

    public void CameraZoomIn(Vector3 target, float ZoomScale, float duration, float timeScale)
    {
        StartCoroutine(HandleCameraZoomIn(target, ZoomScale, duration, timeScale));
    }

    IEnumerator HandleCameraZoomIn(Vector3 target, float ZoomScale,  float duration, float timeScale)
    {
        float StartZoom = Camera.main.orthographicSize;
        float TargetZoom = StartZoom * ZoomScale;
        float Counter = 0f;
        Vector3 Target = AdjustCameraPosition(target, TargetZoom);
        Time.timeScale = timeScale;
        while (Counter < duration)
        {
            float t = Counter / duration;
            Vector3 CameraPos = Vector3.Lerp(Camera.main.transform.position, Target, t);

            Camera.main.transform.position = CameraPos;
            Camera.main.orthographicSize = Mathf.Lerp(StartZoom, TargetZoom, t);
            Counter += Time.deltaTime;

            yield return null;
        }

        TargetZoom = StartZoom;
        StartZoom = Camera.main.orthographicSize;
        Counter = 0f;
        Target = Vector3.zero;
        Target.z = Camera.main.transform.position.z;
        Time.timeScale = 1f;
        while (Counter < duration)
        {
            float t = Counter / duration;
            Vector3 CameraPos = Vector3.Lerp(Camera.main.transform.position, Target, t);
            Camera.main.transform.position = CameraPos;
            Camera.main.orthographicSize = Mathf.Lerp(StartZoom, TargetZoom, t);
            Counter += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.position = Target;
        Camera.main.orthographicSize = TargetZoom;
    }

    Vector3 AdjustCameraPosition(Vector3 position, float Zoom)
    {
        Vector3 AdjustedPosition = position;
        AdjustedPosition.z = Camera.main.transform.position.z;

        float ScreenAspect = (float)Screen.width / Screen.height;
        float ZoomedCameraWidth = Zoom * ScreenAspect;

        float CameraHeightDiff = (GameManager.Instance.CameraBounds.size.y / 2) - Zoom;
        float CameraWidthDiff = (GameManager.Instance.CameraBounds.size.x / 2) - ZoomedCameraWidth;
        
        if (AdjustedPosition.y > CameraHeightDiff)
        {
            AdjustedPosition.y = CameraHeightDiff/2f;
        }
        else if (AdjustedPosition.y < -CameraHeightDiff)
        {
            AdjustedPosition.y = -CameraHeightDiff/2f;
        }

        if (AdjustedPosition.x > CameraWidthDiff)
        {
            AdjustedPosition.x = CameraWidthDiff/2f;
        }
        else if (AdjustedPosition.x < -CameraWidthDiff)
        {
            AdjustedPosition.x = -CameraWidthDiff/2f;
        }

        return AdjustedPosition;
    }

    public void ShakeCamera()
    {
        StartCoroutine(HandleShakeCamera());
    }

    IEnumerator HandleShakeCamera()
    {
        Vector2 ShakeDirection = Random.insideUnitCircle.normalized;
        Vector3 Shake = ShakeDirection * ShakeStrength;

        Camera.main.transform.position += Shake;
        yield return null;
        Camera.main.transform.position -= Shake;
    }
}
