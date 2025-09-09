using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PageCtrl : MonoBehaviour
{
    public List<GameObject> Pages = new List<GameObject>();
    public void func_PageCtrl(int i)
    {
        foreach (GameObject page in Pages)
        {
            if (page != null)
                page.SetActive(false);
        }

        Pages[i].SetActive(true);
    }

    public void All_Check(bool b)
    {
        foreach (GameObject page in Pages)
        {
            if (page != null)
                page.SetActive(b);
        }
    }

    public void True_Page(int i)
    { Pages[i].SetActive(true); }

    public void False_Page(int i)
    { Pages[i].SetActive(false); }
}
