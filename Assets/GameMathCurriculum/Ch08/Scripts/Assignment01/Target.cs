using System;
using UnityEngine;

public class Target : MonoBehaviour {
	public Color TargetColor;

	private void Awake() {
		// 시작 시에 자기 색으로 염색
		GetComponent<Renderer>().material.color = TargetColor;
	}
}