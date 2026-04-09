using System;
using UnityEngine;

public class DragTarget : MonoBehaviour {
	private bool _isDragged;
	private bool _isGoaled;
	private BoxCollider _collider;
	private Vector3 _initialLocation;
	private Vector3 _goalLocation;
	private float _moveSpeed = 5f;
	
	public float Height => _collider.bounds.size.y;

	private void Awake() {
		_collider = GetComponent<BoxCollider>();
		// 처음 시작된 위치 기억
		_initialLocation = transform.position;
	}

	// 드래그 값 변경
	public void SetDragged(bool isDragged) {
		_isDragged = isDragged;
	}
	
	// 도착했을 때 호출할 함수
	public void SetGoal(Vector3 transfrom) {
		_goalLocation = transfrom;
		_isGoaled = true;
	}

	private void Update() {
		// 지금 드래그중이 아니라면, 내 원래 자리로 돌아가야 함
		if (!_isDragged) {
			transform.position = Vector3.Lerp(transform.position, _initialLocation, _moveSpeed * Time.deltaTime);
		} else if (_isGoaled) {
			transform.position = Vector3.Lerp(transform.position, _goalLocation, _moveSpeed * Time.deltaTime);
		}
		
	}
}