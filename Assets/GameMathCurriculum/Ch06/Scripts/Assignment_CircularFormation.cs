// =============================================================================
// Assignment_CircularFormation.cs
// -----------------------------------------------------------------------------
// 목적: 원형 진형 배치 과제 (기본 난이도) — 쿼터니언 합성 & 벡터 곱셈 연습
// ★ 과제 설명:
//    리더 오브젝트 주위에 N개 유닛을 원형으로 배치한다.
//    리더가 이동/회전하면 진형 전체가 함께 따라 이동/회전한다.
//    - 쿼터니언 합성: 리더 회전 * 배치 회전
//    - 쿼터니언 * 벡터: 배치 쿼터니언 * Vector3.forward * radius → 오프셋 위치
// =============================================================================

using System;
using System.Numerics;
using NUnit.Framework;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class Assignment_CircularFormation : MonoBehaviour
{
    [Header("=== 진형 설정 ===")]
    [Tooltip("진형의 중심이 될 리더 오브젝트")]
    [SerializeField] private Transform leader;

    [Tooltip("원형으로 배치할 유닛들의 갯수")]
    [SerializeField] private int _unitCount;
    [SerializeField] private GameObject unitPrefab;

    [Tooltip("리더로부터 유닛까지의 거리 (원의 반지름)")]
    [SerializeField] [UnityEngine.Range(1f, 10f)] private float formationRadius = 3f;
    
    [Header("=== 회전 설정 ===")]
    [Tooltip("Rad값 기준")]
    [UnityEngine.Range(0f, 2f)]
    [SerializeField] private float _rotationSpeed = 3f;

    [Header("=== UI 연결 ===")]
    [SerializeField] private TMP_Text uiInfoText;

    [Header("=== 디버그 정보 (읽기 전용) ===")]
    [SerializeField] private int currentUnitCount;
    [SerializeField] private Vector3 leaderRotationEuler;
    
    private float _stdRotation = 0f;
    private Transform[] units;

    private void Awake() {
        units = new Transform[_unitCount];
        for (int i = 0; i < _unitCount; i++) {
            units[i] = Instantiate(unitPrefab).transform;
        }
    }

    private void Update()
    {
        if (leader == null || units == null || units.Length == 0)
        {
            UpdateUI();
            return;
        }

        currentUnitCount = units.Length;
        leaderRotationEuler = leader.rotation.eulerAngles;

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == null) continue;

            // 1. 유닛들을 리더 주위에 원형으로 균등 배치한다
            // 2. 리더가 이동하면 진형이 함께 이동한다
            // 3. 리더가 회전하면 진형 전체가 함께 회전한다
            // 4. 각 유닛은 리더 기준 바깥 방향을 바라본다
            //
            // **사용 API:** `Quaternion.AngleAxis`, 쿼터니언 곱(`*`), 쿼터니언 × 벡터(`Quaternion * Vector3`)
            
            // 1~3. 원형 균등 배치, 진형 이동, 진형 회전
            // 나로부터 몇 도?
            
            _stdRotation += (_rotationSpeed * Mathf.Rad2Deg) * Mathf.PI * Time.deltaTime;
            _stdRotation %= 360f;
            float angle = 360f / units.Length * i + _stdRotation;
            // 그쪽으로 가는 방향 구하기
            Quaternion unitRotation = Quaternion.AngleAxis(angle, leader.up);
            // 특정 위치만큼 떨어트리기
            Vector3 location = leader.position + (unitRotation * leader.forward * formationRadius);
            units[i].position = location;
            
            // 4. 각 유닛은 리더 기준 바깥쪽 보기
            Vector3 leaderVec = units[i].position - leader.position;
            units[i].rotation = Quaternion.LookRotation(leaderVec, leader.up);
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        if (uiInfoText == null) return;

        string leaderInfo = leader != null ? leader.name : "없음";

        uiInfoText.text = $"[Assignment_CircularFormation]\n" +
            $"리더: {leaderInfo}\n" +
            $"유닛 수: {currentUnitCount}\n" +
            $"반지름: {formationRadius:F1}m\n" +
            $"리더 회전: ({leaderRotationEuler.x:F0}, {leaderRotationEuler.y:F0}, {leaderRotationEuler.z:F0})\n" +
            $"\n<color=yellow>리더를 이동/회전하여 진형 변화를 확인하세요!</color>";
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!enabled) return;

        if (!Application.isPlaying || leader == null || units == null) return;

        // 원형 궤도 표시
        VectorGizmoHelper.DrawCircleXZ(leader.position, formationRadius, new Color(0.3f, 1f, 0.3f, 0.3f));

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] == null) continue;

            // 리더 → 유닛 연결선
            Gizmos.color = new Color(0.3f, 1f, 0.3f, 0.5f);
            Gizmos.DrawLine(leader.position, units[i].position);

            // 유닛 위치 구
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(units[i].position, 0.15f);

            // 유닛 forward 방향
            VectorGizmoHelper.DrawArrow(units[i].position,
                units[i].position + units[i].forward * 0.8f,
                Color.cyan, 0.1f, 15f);
        }

        // 리더 위치 표시
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(leader.position, 0.25f);
    }
#endif
}
