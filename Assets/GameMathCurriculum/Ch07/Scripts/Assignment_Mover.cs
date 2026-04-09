using System;
using System.Security.Cryptography;
using UnityEngine;
using Random = UnityEngine.Random;

public class Assignment_Mover : MonoBehaviour
{
    
    private Vector3 _p0;
    private Vector3 _p1;
    private Vector3 _p2;
    private Vector3 _p3;
    
    private Renderer _renderer;
    private float _duration;
    private float _t;


    private void Awake() {
        _renderer = GetComponent<Renderer>();
    }

    public void Init(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float duration, float scale, Material material) {
        _p0 = p0;
        _p1 = p1;
        _p2 = p2;
        _p3 = p3;
        _duration = duration;
        _renderer.material = material;
        
        transform.localScale =  new Vector3(scale, scale, scale);
        
        _t = 0f;
    }

    private void Update() {
        if(_t >= 1f) { Destroy(gameObject); }
        
        _t += Time.deltaTime / _duration;
        
        transform.position = CubicBezier(_t);
    }
    
    Vector3 CubicBezier(float t)  {
        // de Casteljau 알고리즘 — 3단계 Lerp
        Vector3 a = Vector3.Lerp(_p0, _p1, t);
        Vector3 b = Vector3.Lerp(_p1, _p2, t);
        Vector3 c = Vector3.Lerp(_p2, _p3, t);

        Vector3 d = Vector3.Lerp(a, b, t);
        Vector3 e = Vector3.Lerp(b, c, t);

        return Vector3.Lerp(d, e, t);
    }
}
