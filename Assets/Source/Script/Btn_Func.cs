using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Btn_Func : MonoBehaviour
{
    public enum Type { Save, }
    public Type Case;

    AudioSource As;

    float E_Count;
    [Header("0.Up, 1.Down Sound")]
    public List<AudioClip> E_Clips;
    public List<GameObject> E_Img;


    [Header("0.�浹, 1.�浹��, 2.�Ϸ�")]
    public List<AudioClip> Clips;

    public Collider Coll;

    public bool pushis, done;

    public float Count;
    public float CountMax;

    ConfigurableJoint joint;

     float damper = 50f;
     float maxForce = 200f;

    private Coroutine currentCountingCoroutine = null;

    private void Start()
    {
        E_Count = -1;
        Energy_img(0);

        joint = GetComponent<ConfigurableJoint>();
        JointCtrl(300f);
        As = GetComponent<AudioSource>();
    }

    void Func_Sound(AudioClip clip)
    {
        if (clip != null)
        {
            GameObject Audio = new GameObject(transform.name + "_Audio");
            Audio.transform.position = transform.position;
            Audio.AddComponent<Ins_Effect>().AudioPlay(clip);
        }
    }

    private void OnCollisionEnter(Collision coll)
    {

        if (coll.gameObject.Equals(Coll.gameObject))
        {
            if (done == false)
            {
                pushis = true;

                if (coll.gameObject.Equals(Coll.gameObject))
                {
                    // �̹� ���� ���� Counting �ڷ�ƾ�� �ִٸ� ����
                    if (currentCountingCoroutine != null)
                    {
                        StopCoroutine(currentCountingCoroutine);
                    }
                }

                // ���ο� Counting �ڷ�ƾ ����
                currentCountingCoroutine = StartCoroutine(Counting());

                As.clip = Clips[1];
                As.Play();
            }

            JointCtrl(50f);
        }
    }

    public void Energy_img(int i)
    {
        if (i != E_Count)
        {
            for (int index = 0; index < E_Img.Count; index++)
            {
                if (index < i)
                    E_Img[index].SetActive(true);
                else
                    E_Img[index].SetActive(false);
            }

            if (E_Count >= 0)
            {
                if (E_Count > i) { Func_Sound(E_Clips[1]); }
                else if (E_Count < i) { Func_Sound(E_Clips[0]); }
            }

            E_Count = i;
        }
    }

    void JointCtrl(float f)
    {
        JointDrive drive = new JointDrive
        {
            positionSpring = f,
            positionDamper = damper,
            maximumForce = maxForce
        };

        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;
    }

    private void OnCollisionExit(Collision coll)
    {
        if (coll.gameObject.Equals(Coll.gameObject))
        {
            //Count = 0;
            pushis = false;

            As.Stop();
            JointCtrl(300f);
        }
    }

    public void Sel_Evt()
    {
        if(Case .Equals(Type.Save))
        {
            Debug.Log("���̺� ��");
            Func_Sound(Clips[2]);
            done = true;
        }
    }

    IEnumerator Counting()
    {
        int lastIntCount = Mathf.FloorToInt(Count); // ���� int �� ����

        while (!done)
        {
            // ������ �ִ� ��
            if (pushis)
            {
                if (Count < CountMax)
                {
                    Count += Time.deltaTime;

                    int newIntCount = Mathf.FloorToInt(Count);
                    if (newIntCount != lastIntCount)
                    {
                        Energy_img(newIntCount);

                        //Debug.Log($"Count Increased to: {newIntCount}");
                        lastIntCount = newIntCount;
                    }
                }
                else
                {
                    Sel_Evt();
                }
            }
            // �� ������ �ִ� ��
            else
            {
                if (Count > 0)
                {
                    Count -= Time.deltaTime;
                    Count = Mathf.Max(0, Count); // ���� ����

                    int newIntCount = Mathf.FloorToInt(Count);
                    if (newIntCount != lastIntCount)
                    {
                        Energy_img(newIntCount);

                        //Debug.Log($"Count Decreased to: {newIntCount}");
                        lastIntCount = newIntCount;
                    }
                }
            }

            yield return null;
        }
    }
}
