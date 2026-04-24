using UnityEngine;

[CreateAssetMenu(menuName = "Camera/CarCameraSettings")]
public class CarCameraSettings : ScriptableObject
{
    public Vector3 moveOffset = new Vector3(0f, 5f, -10f);
    public Vector3 rotOffset = Vector3.zero;
    public float moveSmoothness = 5f;
    public float rotSmoothness = 5f;
    public float maxDistanceBehind = 15f;
}