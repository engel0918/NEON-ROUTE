using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Inven_Ctrl : MonoBehaviour
{
    public TMP_Text id;
    public TMP_Text type;
    public TMP_Text value;

    [Header("NoneItem Prefab")]
    [SerializeField] GameObject Item;

    public List<Transform> QuickSlot;
    public List<Transform> Inven_WP;
    public List<Transform> Inven_CON;
    public List<Transform> Inven_MAT;
    public List<Transform> Inven_VAL;

    public Item_Load ItList;

    [Header("Fill X")]
    public List<string> Data_Quick;
    public List<string> Data_WP;
    public List<string> Data_CON;
    public List<string> Data_MAT;
    public List<string> Data_VAL;

    public List<int> Count_Quick;
    public List<int> Count_CON;
    public List<int> Count_MAT;
    public List<int> Count_VAL;

    private void Awake()
    {
        DataSet(QuickSlot, Data_Quick, Count_Quick);
        DataSet(Inven_WP, Data_WP, null);
        DataSet(Inven_CON, Data_CON, Count_CON);
        DataSet(Inven_MAT, Data_MAT, Count_MAT);
        DataSet(Inven_VAL, Data_VAL, Count_VAL);

    }

    void DataSet(List<Transform> trList, List<string> StrList, List<int> CountList)
    {
        for(int i = 0; i <= (trList.Count-1); i++)
        {
            StrList.Add("");
            if (CountList != null)
            { CountList.Add(0); }
        }
    }

    private void Start()
    {
        if (ItList == null)
        { ItList = GameObject.FindGameObjectWithTag("ItList").GetComponent<Item_Load>(); }
    }

    public void getIt_test()
    {
        string id_cd = id.text.ToString();
        string type_cd = type.text.ToString();
        int value_cd = int.Parse(value.text);
        GetItem(id_cd, type_cd, value_cd);
    }

    // id, Ÿ��, ����
    public bool GetItem(string id, string type, int Value)
    {
        if (type == "���")
        {
            int Val = EmptySlot(Data_WP);

            if (Val != -1)
            {
                ItemCtrl it = Instantiate(Item, Inven_WP[Val]).GetComponent<ItemCtrl>();
                it.ItList = ItList;
                it.ItemCheck(id);

                // ������ ó�� --------------------------------------------------
                Data_WP[Val] = id;
                return true;
            }
            else
            {
                Debug.Log("��� ������ ������ ����á���ϴ�.");
                return false;
            }
        }
        else if (type == "�Һ�")
        {
            int Count = CountCheck(id, Data_CON);

            if(Count != -1)
            {
                // ������ ó�� --------------------------------------------------
                Data_WP[Count] = id;
                Count_CON[Count] += Value;

                ItemCtrl itCtrl = Inven_CON[Count].GetChild(0).GetComponent<ItemCtrl>();
                itCtrl.Count = Count_CON[Count];
                itCtrl.txt_Count.text = itCtrl.Count.ToString();

                return true;
            }
            else
            {
                int Val = EmptySlot(Data_CON);

                if (Val != -1)
                {
                    ItemCtrl it = Instantiate(Item, Inven_CON[Val]).GetComponent<ItemCtrl>();
                    it.ItList = ItList;
                    it.ItemCheck(id);

                    // ������ ó�� --------------------------------------------------
                    Data_CON[Val] = id;
                    Count_CON[Val] += Value;

                    ItemCtrl itCtrl = Inven_CON[Val].GetChild(0).GetComponent<ItemCtrl>();
                    itCtrl.Count = Count_CON[Val];
                    itCtrl.txt_Count.text = itCtrl.Count.ToString();

                    return true;
                }
                else
                {
                    Debug.Log("�Һ� ������ ������ ����á���ϴ�.");
                    return false;
                }
            }
        }
        else
        {
            // �ٸ� Ÿ�� ó�� (��: �Һ���, ��� ��)
            Debug.Log($"�� �� ���� ������ Ÿ��: {type}");
            return false;
        }
    }

    int EmptySlot(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "")
            {
                return i; // �� ���� �߰� �� �ε��� ��ȯ
            }
        }
        return -1; // �� ������ ���� ��� -1 ��ȯ
    }

    int CountCheck(string str, List<string> list)
    {
        for(int i = 0; i <= (list.Count-1); i++)
        { if (list[i] == str) { return i; } }

        return -1; // �� ã���� -1 ��ȯ
    }

    void ChangeSlot()
    {

    }
}
