using Unity.VisualScripting;
using UnityEngine;

public class Assignment_BezierRandomMover : MonoBehaviour {
    private readonly string SpaceKey = "SpaceKey";

    [Header("=== 시작, 종료 포인트 ===")]
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    
    [Header("=== 랜덤 값 지정 ===")]
    [SerializeField] float _randomTimeMax = 3f;
    [SerializeField] private float _zRandomRange = 20f;
    [SerializeField] private float _yRandomRange = 20f;
    
    [Header("=== 한 번 눌렀을 때 생성할 개수 ===")]
    [SerializeField] private int _generateCount = 200;
    
    [Header("=== 생성할 Prefab ===")]
    [SerializeField] private Assignment_Mover _mover;
        
    private void Update() {
        if (Input.GetButtonDown(SpaceKey)) {
            for (int i = 0; i < _generateCount; i++) FireOnce();
        }
    }
    
    private void FireOnce() { 
        // 랜덤하게 점 생성
        float xStart = _startPoint.position.x;
        float xEnd = _endPoint.position.x;
        float xDiff = xEnd - xStart;
        float yPos = _startPoint.position.y;
        
        Vector3 p1 = new Vector3(Random.Range(xStart, xStart + xDiff / 2), yPos + Random.Range(0f, _yRandomRange), Random.Range(-_zRandomRange, _zRandomRange));
        Vector3 p2 = new Vector3(Random.Range(xStart + xDiff / 2, xEnd), yPos + Random.Range(0f, _yRandomRange), Random.Range(-_zRandomRange, _zRandomRange));
        
        // 생성된 두 점을 기반으로 Instantiate 후 Initialize
        var mover = Instantiate(_mover, _startPoint.position, Quaternion.identity);
        mover.transform.SetParent(transform, true);
        mover.Init(_startPoint.position, p1, p2, _endPoint.position, Random.Range(0.2f, _randomTimeMax));
    }
}
