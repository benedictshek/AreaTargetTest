using UnityEngine;

public class MinimapCameraController : MonoBehaviour
{
    public RectTransform test;

    private void LateUpdate()
    {
        Vector3 newCamPosition = Camera.main.transform.position;
        newCamPosition.y = transform.position.y;
        transform.position = newCamPosition;

        test.rotation = Quaternion.Euler(0, 0, -Camera.main.transform.eulerAngles.y);
    }
}
