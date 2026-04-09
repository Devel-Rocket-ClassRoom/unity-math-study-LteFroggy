using UnityEngine;
using UnityEngine.UI;

public class TargetTracker : MonoBehaviour {
	private TrackingTarget[] _targets;
	private RectTransform[] _indicators;
	private Camera _camera;
	private GameObject _canvas;
	
	[Header("=== 디버그용 변수 ===")]
	[SerializeField] private string[] _showingState;
	[SerializeField] private float _screenWidth;
	[SerializeField] private float _screenHeight;
	
	private void Awake() {
		_camera = Camera.main;
		_canvas = GameObject.FindWithTag(Tags.Canvas);
		
		_screenWidth = Screen.width;
		_screenHeight = Screen.height;
		
		var targets = GameObject.FindGameObjectsWithTag(Tags.Target);
		
		// 찾은 타겟 개수만큼 초기화
		_targets = new TrackingTarget[targets.Length];
		_showingState = new string[_targets.Length];
		_indicators = new RectTransform[_targets.Length];
		
		for (int i = 0; i < targets.Length; i++) {
			_targets[i] = targets[i].GetComponent<TrackingTarget>();
			
			// 찾은 타겟 개수만큼 Indicator 생성
			// 1. Image Object 생성 -> RectTransform, CanvasRenderer, Image 3개가 붙어있어야 함.
			GameObject indicator = new GameObject($"Indicator", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
			// 2. 부모 지정
			indicator.transform.SetParent(_canvas.transform);
			// 3. 피벗, 앵커 설정
			_indicators[i] = indicator.GetComponent<RectTransform>();
			_indicators[i].anchorMin = new Vector2(0.0f, 0.0f);
			_indicators[i].anchorMax = new Vector2(0.0f, 0.0f);
			_indicators[i].pivot = new Vector2(0.0f, 0.0f);
			
			// 4. 색상 지정
			_indicators[i].gameObject.GetComponent<Image>().color = _targets[i].TargetColor;
		}
	}

	private void Update() {
		for (int i = 0; i < _targets.Length; i++) {
			// 매 포인트 상대의 위치를 screenPosition으로.
			Vector3 targetPoint = _camera.WorldToScreenPoint(_targets[i].transform.position);
			
			// 스크린 안에 있으면 true로
			if (targetPoint.x >= 0 &&
			    targetPoint.x <= Screen.width &&
			    targetPoint.y >= 0 &&
			    targetPoint.y <= Screen.height &&
			    targetPoint.z >= 0) 
			{
				// 인디케이터 비활성화
				_indicators[i].gameObject.SetActive(false);
			} else {
				// 인디케이터 활성화
				_indicators[i].gameObject.SetActive(true);
				
				// 뒤에 있으면, x값 반전 및 y값 0으로 설정
				if (targetPoint.z < 0) {
					// x는 스크린 크기 기반으로 반전해야 하는 것 같음?..
					targetPoint.x = Screen.width - targetPoint.x;
					targetPoint.y = 0f;
				}
				
				// 인디케이터 위치 정해주기
				float indicatorXLoc;
				float indicatorYLoc;
				if (targetPoint.x <= 0) { indicatorXLoc = 0f; } 
				else if (targetPoint.x >= Screen.width) { indicatorXLoc = Screen.width; } 
				else { indicatorXLoc = targetPoint.x; }
				if (targetPoint.y <= 0) { indicatorYLoc = 0f; } 
				else if (targetPoint.y >= Screen.height) { indicatorYLoc = Screen.height; } 
				else { indicatorYLoc = targetPoint.y; }
				
				// 현재 위치에 따라 피벗 위치 수정해주기
				float xPivot = targetPoint.x <= Screen.width / 2f ? 0.0f : 1f;
				float yPivot = targetPoint.y <= Screen.height / 2f ? 0.0f : 1f;
				
				_indicators[i].pivot = new Vector2(xPivot, yPivot);
				_indicators[i].position = new Vector3(indicatorXLoc, indicatorYLoc, 0f);
				
				_showingState[i] = $"위치 : {targetPoint.x:f1}, {targetPoint.y:f1}, {targetPoint.z:f1}\n" +
				                   $"pivot : {xPivot:f0}, {yPivot:f0}";
			}
		}
	}
}
