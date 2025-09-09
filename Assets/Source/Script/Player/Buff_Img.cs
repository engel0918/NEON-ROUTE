using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Buff_Img : MonoBehaviour
{
    [Header("Out, Gage_Frame, Gage, InnerBg")]
    public List<Image> ImgList;

    public Image MainImg;

    [Header("Run, Drag, ")]
    public string Buff;

    public float CountLim;
    public float Count;

    public List<Buff_Design> Type;
    public List<Buff_List> BuffList;

    private void FixedUpdate()
    {
        if (Buff != "")
        {
            if (CountLim >= Count)
            {
                Count += Time.deltaTime;
                ImgList[2].fillAmount = Count / CountLim;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void BuffOn(string str, int i)
    {
        CountLim = i;
        Count = 0;

        Buff = str;
        SetBuff(str);

        if (str == "Run")
        {
            SetDesign("Buff");
        }
    }


    void SetDesign(string str)
    {
        for(int i = 0; i <= (Type.Count-1); i++)
        {
            if(str == Type[i].Set)
            {
                for(int o = 0; o <= (Type[i].Colors.Count-1); o++)
                { ImgList[o].color = Type[i].Colors[o]; }
            }

            break;
        }
    }

    void SetBuff(string str)
    {
        for (int i = 0; i <= (BuffList.Count - 1); i++)
        {
            if(str == BuffList[i].Buff)
            { MainImg.sprite = BuffList[i].Spr; }
        }
    }
}

[System.Serializable]
public struct Buff_Design
{
    public string Set;

    [Header("Out, Gage_Frame, Gage, InnerBg")]
    public List<Color> Colors;
}

[System.Serializable]
public struct Buff_List
{
    public string Buff;
    public Sprite Spr;
}
