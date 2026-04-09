using UnityEngine;

public class SelectByCursor : MonoBehaviour {
	private int _targetLayerMask;
	private int _groundLayerMask;
	private int _goalLayerMask;
	private Camera _mainCamera;
	
	private Transform _draggingTarget;
	
	[Header("=== RayCast 관련 변수 ===")]
	[SerializeField] private float _maxDistance = 100f;

	private void Awake() {
		_mainCamera = Camera.main;
		
		_groundLayerMask = LayerMask.GetMask("Ground");
		_targetLayerMask = LayerMask.GetMask("Target");
		_goalLayerMask = LayerMask.GetMask("Goal");
	}

	private void Update() {
		if (Input.GetMouseButtonDown(0)) {
			// 커서 위치 기반으로 가능한 위치 선택
			Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
		
			// Target만 잡히게 RayCast
			if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _targetLayerMask)) {
				Debug.Log("Target 잡힘");
				// 있다면, 드래그 토글
				_draggingTarget = hit.collider.gameObject.transform;
				_draggingTarget.GetComponent<DragTarget>().SetDragged(true);
			}
		}
		
		// 계속 누르고 있다면, 해당 Target의 위치를 수정해줘야 함
		else if (Input.GetMouseButton(0)) {
			// Ray 만들기
			Ray toGround = _mainCamera.ScreenPointToRay(Input.mousePosition);
			// 커서 위치 기반으로 바닥과 RayCast
			if (Physics.Raycast(toGround, out RaycastHit hit, _maxDistance, _groundLayerMask)) {
				Debug.Log("바닥과 닿음");
				_draggingTarget.position = hit.point +
				                           // 실제 크기만큼 좀 띄우기.
				                           _draggingTarget.GetComponent<DragTarget>().Height * Vector3.up;
			}
		}
		
		// 마우스를 떼면, 드래그 끝	
		else if (Input.GetMouseButtonUp(0)) {
			if (_draggingTarget == null) { return; }
			
			Debug.Log("Target 놓음");
			
			// 마우스 땔 때 Goal에 잘 놨는지 체크
			Ray toGoal = _mainCamera.ScreenPointToRay(Input.mousePosition);
			// Goal과 Raycast
			if (Physics.Raycast(toGoal, out RaycastHit hit, _maxDistance, _goalLayerMask)) {
				_draggingTarget.GetComponent<DragTarget>().SetGoal(hit.collider.transform.position + 
				                                                   _draggingTarget.GetComponent<DragTarget>().Height * Vector3.up);
			}
			
			// 닿지 않았다면, dragged 초기화
			else {
				_draggingTarget.GetComponent<DragTarget>().SetDragged(false);
				_draggingTarget = null;	
			}
			
		}
	}
}
