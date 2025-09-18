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
    private bool isPlaying = false;  // ��ƼŬ ��� ���� üũ

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

            // ���̴� �� ������Ʈ (spawnEffectTime ���ȸ� ����)
            if (timer < spawnEffectTime)
            {
                _renderer.material.SetFloat(shaderProperty, fadeIn.Evaluate(timer / spawnEffectTime));
            }

            // ���� �ð��� ������ �ڵ����� ����
            if (timer >= spawnEffectTime + pause)
            {
                isPlaying = false;
            }
        }
    }

    public void Ptc_Play()
    {
        if (!isPlaying) // �̹� ��� ���̸� �ߺ� ���� ����
        {
            isPlaying = true;
            timer = 0;
            ps.Play();
        }
    }
}
