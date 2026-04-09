using TMPro;
using UnityEngine;

public class PlayerMove : MonoBehaviour {
    
    [Header("=== 플레이어 이동 관련 값 ===")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _yawSpeed = 180f;
    
    private PlayerInputManager _inputManager;
    private Rigidbody _rigidbody;

    private void Awake() {
        _inputManager =  GetComponent<PlayerInputManager>();
        _rigidbody = GetComponent<Rigidbody>();
    }

    private void Update() {
        
    }

    private void FixedUpdate() {
        // 플레이어 이동
        Vector3 moveLocation = _inputManager.Vertical * transform.forward + _inputManager.Horizontal * transform.right;
        moveLocation = Vector3.ClampMagnitude(moveLocation, 1f);
        _rigidbody.MovePosition(_rigidbody.position + moveLocation * _moveSpeed * Time.fixedDeltaTime);
        
        // 플레이어 회전
        Quaternion rotation = Quaternion.AngleAxis(-_inputManager.Yaw * _yawSpeed * Time.fixedDeltaTime, transform.up);
        _rigidbody.MoveRotation(_rigidbody.rotation * rotation);
    }
}
