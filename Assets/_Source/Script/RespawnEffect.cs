using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnEffect : MonoBehaviour
{
    public float spawnEffectTime = 2;
    public float pause = 1;
    public AnimationCurve fadeIn;

    private ParticleSystem ps;
    private float timer = 0;
    private Renderer _renderer;
    private int shaderProperty;
    private bool isPlaying = false;  // 파티클 재생 상태 체크

    void Start()
    {
        shaderProperty = Shader.PropertyToID("_cutoff");
        _renderer = GetComponent<Renderer>();
        ps = GetComponentInChildren<ParticleSystem>();

        var main = ps.main;
        main.duration = spawnEffectTime;
    }

    void Update()
    {
        if (isPlaying)
        {
            timer += Time.deltaTime;

            // 셰이더 값 업데이트 (spawnEffectTime 동안만 실행)
            if (timer < spawnEffectTime)
            {
                _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(timer / spawnEffectTime));
            }

            // 일정 시간이 지나면 자동으로 중지
            if (timer >= spawnEffectTime + pause)
            {
                isPlaying = false;
            }
        }
    }

    public void Ptc_Play()
    {
        if (!isPlaying) // 이미 재생 중이면 중복 실행 방지
        {
            isPlaying = true;
            timer = 0;
            ps.Play();
        }
    }
}
