using UnityEngine;

public class FollowingPlayerCamera : MonoBehaviour
{
    private Transform _player;
    
    [Header("=== 플레이어와 카메라가 떨어진 거리 ===")]
    [SerializeField] private float _yOffSet = 3.5f;
    [SerializeField] private float _zOffSet = 5f;
    
    [Header("=== 카메라 SmoothDamp 관련 변수 ===")]
    [SerializeField] private float _cameraMoveSmoothTime = 0.1f;
    [SerializeField] private float _cameraRotateSpeed = 180f;
    
    private Vector3 _currentVelocity = Vector3.zero;
    
    private void Awake() {
        _player = GameObject.FindWithTag(Tags.Player).transform;
    }
    
    private void LateUpdate() {
        // 플레이어의 뒷방향으로 yOffset, zOffset만큼 떨어지기
        Vector3 localOffset = new Vector3(0f, _yOffSet, -_zOffSet);
        // 플레이어의 TransformPoint를 통해 로컬 좌표계로 변환 후, 더하기
        Vector3 newLocation = _player.TransformPoint(localOffset);
        // 위치 적용
        transform.position = newLocation;
        
        // SmoothDamp. 멀미나서 임시 적용 해제
        //transform.position = Vector3.SmoothDamp(transform.position, newLocation, ref _currentVelocity, _cameraMoveSmoothTime);
        
        // 플레이어 보기
        Quaternion targetRotation =  Quaternion.LookRotation((_player.position - transform.position).normalized);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * _cameraRotateSpeed);
        
        // Slerp 일정 회전각 이하면 회전 중지
        if (Quaternion.Angle(transform.rotation, targetRotation) <= 1f) {
            transform.rotation = targetRotation;
        }
    }
}
