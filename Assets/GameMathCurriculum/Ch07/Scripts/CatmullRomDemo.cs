using System;
using UnityEngine;
using TMPro;

// =============================================================================
// CatmullRomDemo.cs
// -----------------------------------------------------------------------------
// Catmull-Rom 스플라인으로 경로를 따라 이동하고 접선 방향으로 회전하는 데모
// =============================================================================

public class CatmullRomDemo : MonoBehaviour
{
    [Header("=== 경로 설정 ===")]
    [Tooltip("경로의 중간 지점들 (최소 2개 필요)")]
    [SerializeField] private Transform[] waypoints = new Transform[0];

    [Header("=== 이동 설정 ===")]
    [Range(0.1f, 2f)]
    [Tooltip("스플라인을 따라 이동하는 속도")]
    [SerializeField] private float speed = 0.5f;

    [Tooltip("t값 수동 조작 활성화")]
    [SerializeField] private bool useManualT = false;

    [Tooltip("수동 t값 (0~1)")]
    [Range(0f, 1f)]
    [SerializeField] private float manualT = 0f;

    [Tooltip("경로 루핑 활성화")]
    [SerializeField] private bool loopPath = true;

    [Tooltip("현재 방향(접선)을 보도록 회전")]
    [SerializeField] private bool lookAlongCurve = true;

    [Header("=== 시각화 설정 ===")]
    [SerializeField] private Color colorSpline = Color.green;
    [SerializeField] private Color colorWaypoints = Color.yellow;
    [SerializeField] private Color colorMovingPoint = Color.cyan;

    [Header("=== UI 연결 ===")]
    [Tooltip("정보 표시용 TMP_Text")]
    [SerializeField] private TMP_Text uiInfoText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [SerializeField] private float currentT;
    [SerializeField] private Vector3 currentPosition;
    [SerializeField] private Vector3 currentTangent;
    [SerializeField] private int currentSegment;
    [SerializeField] private float elapsedTime;

    private void Start()
    {
        // 재생 시간과 정규화 파라미터를 초기화한다.
        elapsedTime = 0f;
        currentT = 0f;
    }

    private void Update()
    {
        // t는 수동 슬라이더로 제어하거나 시간에 따라 자동으로 증가시킨다.
        if (useManualT) {
            currentT = manualT;
        } else {
            elapsedTime += Time.deltaTime * speed;
            if (loopPath) {
                currentT = Mathf.Repeat(elapsedTime, 1f);
            } else {
                currentT = Mathf.Clamp01(elapsedTime);
            }
        }
        
        // 현재 t에서 스플라인 위치와 접선 방향을 샘플링한다.
        currentPosition = EvaluateSpline(currentT);
        currentTangent = EvaluateSplineTangent(currentT);
        
        // 계산한 위치/방향을 오브젝트 Transform에 반영한다.
        transform.position = currentPosition;
        if (lookAlongCurve && currentTangent != Vector3.zero) {
            transform.rotation = Quaternion.LookRotation(currentTangent.normalized);   
        }

        UpdateUI();
    }

    private Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        // p1->p2 구간을 p0, p3 이웃점을 포함해 Catmull-Rom으로 보간한다.
        var qt = 0.5f * ((2f * p1) + (-(p0) + p2) * t) 
                        + (((2f * p0) - (5f * p1) + (4f * p2) - p3) * Mathf.Pow(t, 2)) 
                        + (-(p0) + ( 3 * p1) - (3 * p2) + p3) * Mathf.Pow(t, 3);
        return qt;
    }

    private Vector3 EvaluateSpline(float t)
    {
        int n = waypoints.Length;
        
        // 이동 가능한 경로를 만들려면 최소 2개의 웨이포인트가 필요하다.
        if (n < 2) {
            return transform.position;
        }
        
        // 전역 정규화 t를 구간 인덱스와 구간 내부 t로 변환한다.
        float globalT = t * (n - 1);
        int segmentIndex = Mathf.FloorToInt(globalT);
        float localT = globalT - segmentIndex;
        
        // t가 끝에 도달했을 때 마지막 유효 구간으로 제한한다.
        if (segmentIndex >= n - 1) {
            segmentIndex = n - 2;
            localT = 1f;
        }
        
        // Catmull-Rom 계산을 위해 현재 구간 주변 4개 점을 준비한다.
        Vector3 p0 = GetWaypoint(segmentIndex - 1);
        Vector3 p1 = GetWaypoint(segmentIndex);
        Vector3 p2 = GetWaypoint(segmentIndex + 1);
        Vector3 p3 = GetWaypoint(segmentIndex + 2);
        
        return CatmullRom(p0, p1, p2, p3, localT);
    }

    private Vector3 EvaluateSplineTangent(float t)
    {
        // t 주변의 유한 차분으로 접선을 수치적으로 근사한다.
        float delta = 0.0001f;
        Vector3 p1 = EvaluateSpline(Mathf.Clamp01(t - delta));
        Vector3 p2 = EvaluateSpline(Mathf.Clamp01(t + delta));
           
        return (p2 - p1).normalized;
    }

    private Vector3 GetWaypoint(int index)
    {
        int n = waypoints.Length;
        // 경계 구간에서도 안전하도록 인덱스를 보정해 점을 조회한다.
        int wrappedIndex = Mathf.Clamp(index, 0, n - 1);
        
        return waypoints[wrappedIndex].position;
    }

    private void OnDrawGizmos()
    {
        if (!enabled) return;

        // 스플라인 정의가 부족하면 기즈모를 그리지 않는다.
        if (waypoints == null || waypoints.Length < 2) return;

        // 웨이포인트 마커를 그린다.
        float wpSize = 0.2f;
        Gizmos.color = colorWaypoints;
        for (int i = 0; i < waypoints.Length; i++)
        {
            if (waypoints[i] == null) continue;
            Gizmos.DrawSphere(waypoints[i].position, wpSize);
        }

        // 샘플링한 점들을 이어 스플라인 폴리라인을 그린다.
        int segments = 100;
        Vector3 prevPoint = EvaluateSpline(0f);
        Gizmos.color = colorSpline;

        for (int i = 1; i <= segments; i++)
        {
            float t = i / (float)segments;
            Vector3 nextPoint = EvaluateSpline(t);
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        // 현재 샘플 위치와 접선 화살표를 그린다.
        Gizmos.color = colorMovingPoint;
        Gizmos.DrawSphere(currentPosition, wpSize * 1.3f);

        if (currentTangent != Vector3.zero)
        {
            VectorGizmoHelper.DrawArrow(currentPosition,
                currentPosition + currentTangent * 0.5f,
                colorMovingPoint, 0.2f);
        }

#if UNITY_EDITOR
        // 씬 뷰에서 빠른 확인용 디버그 라벨을 표시한다.
        Vector3 labelPos = currentPosition + Vector3.up * 0.6f;
        string info = $"t: {currentT:F3}\nSeg: {currentSegment}";
        VectorGizmoHelper.DrawLabel(labelPos, info, colorMovingPoint);
#endif
    }

    private void UpdateUI()
    {
        if (uiInfoText == null) return;

        // 실행 중 상태 정보를 UI 텍스트에 갱신한다.
        int wpCount = waypoints != null ? waypoints.Length : 0;

        uiInfoText.text =
            $"[CatmullRomDemo] Catmull-Rom 스플라인\n" +
            $"경로점 개수: {wpCount}\n" +
            $"현재 t: {currentT:F3}\n" +
            $"현재 구간: {currentSegment}\n" +
            $"위치: ({currentPosition.x:F2}, {currentPosition.y:F2}, {currentPosition.z:F2})\n" +
            $"모드: {(loopPath ? "<color=green>루프</color>" : "<color=red>끝점 멈춤</color>")}";
    }
}
