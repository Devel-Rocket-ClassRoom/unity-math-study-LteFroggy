using UnityEngine;

public class PlayerInputManager : MonoBehaviour {
    private const string HorizontalAxis = "Horizontal";
    private const string VerticalAxis = "Vertical";
    private const string YawAxis = "Yaw";

    public float Horizontal { get; private set; }
    public float Vertical { get; private set; }
    public float Yaw { get; private set; }

    
    private void Update() {
        Horizontal = Input.GetAxis(HorizontalAxis);
        Vertical = Input.GetAxis(VerticalAxis);
        Yaw = Input.GetAxis(YawAxis);
    }
}
