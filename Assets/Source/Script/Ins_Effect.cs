using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Ins_Effect : MonoBehaviour
{
    AudioSource As;
    public AudioClip Clip;

    ParticleSystem Ptc;

    bool Fin;

    IEnumerator PtcCheck()
    {
        if (!Ptc.isPlaying)
        { Fin = true; }
        yield return new WaitForSeconds(1f);
        if (Fin == false) { StartCoroutine(PtcCheck()); }
        else { Destroy(this.gameObject); }
    }

    public void PtcPlay(ParticleSystem PTC)
    {
        Fin = false;
        Ptc = PTC;
        Ptc.Play();

        StartCoroutine(PtcCheck());
    }

    IEnumerator AudioCheck()
    {
        if (!As.isPlaying)
        { Fin = true; }
        yield return new WaitForSeconds(1f);
        if (Fin == false) { StartCoroutine(AudioCheck()); }
        else { Destroy(this.gameObject); }
    }

    public void AudioPlay(AudioClip clip)
    {
        Fin = false;
        As = transform.AddComponent<AudioSource>();
        As.clip = clip;
        As.Play();

        StartCoroutine(AudioCheck());
    }
}
