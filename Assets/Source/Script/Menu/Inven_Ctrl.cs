using System.Collections.Generic;
using System.Linq;
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
    public List<string> Data_Quick, Data_WP, Data_CON, Data_MAT, Data_VAL;
    public List<int> Count_Quick, Count_CON, Count_MAT, Count_VAL;

    public int G;

    public List<string> Data_Equip;
    public List<string> Head_Col, Core_Col, Body_Col, Arms_Col, Legs_Col;

    SteamSave steam;

    private void Awake()
    {
        StartSet();

        //if (steam != null)
        //{ GameObject.FindGameObjectWithTag("SteamMgr").GetComponent<SteamSave>(); }
        //steam.Load_Cha();
    }

    void StartSet()
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

        Load_Cha();
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
                it.inven = this;
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
            int QuickCount = CountCheck(id, Data_Quick);

            if (Count != -1)
            {
                // ������ ó�� --------------------------------------------------
                Data_CON[Count] = id;
                Count_CON[Count] += Value;

                ItemCtrl itCtrl = Inven_CON[Count].GetChild(0).GetComponent<ItemCtrl>();
                itCtrl.Count = Count_CON[Count];
                itCtrl.txt_Count.text = itCtrl.Count.ToString();

                return true;
            }
            else if(QuickCount != -1)
            {
                // ������ ó�� --------------------------------------------------
                Data_Quick[QuickCount] = id;
                Count_Quick[QuickCount] += Value;

                ItemCtrl itCtrl = QuickSlot[QuickCount].GetChild(0).GetComponent<ItemCtrl>();
                itCtrl.Count = Count_Quick[QuickCount];
                itCtrl.txt_Count.text = itCtrl.Count.ToString();

                return true;
            }
            else
            {
                int Val = EmptySlot(Data_CON);

                if (Val != -1)
                {
                    ItemCtrl it = Instantiate(Item, Inven_CON[Val]).GetComponent<ItemCtrl>();
                    it.inven = this;
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

    public void Exchange(Transform org, Transform bf, Transform tr)
    {
        // ���� ���Թ� ���� ������ Ÿ���� Ȯ��
        string bf_Type = bf.GetComponent<DropSlot>().Type;
        string org_type = org.GetComponent<DropSlot>().Type;

        // ������ ���� �����Ϳ��� ������ ���� �����ͷ� ������ �̵�

        //������ �ű��
        string bf_It = "";
        int bf_count = 0;

        string org_It = "";
        int org_count = 0;

        if (bf_Type == "Quick")
        {
            bf_It = Data_Quick[CheckSlot(QuickSlot, bf)];
            bf_count = Count_Quick[CheckSlot(QuickSlot, bf)];
        }
        else if (bf_Type == "���")
        {
            bf_It = Data_WP[CheckSlot(Inven_WP, bf)];
        }
        else if (bf_Type == "�Һ�")
        {
            bf_It = Data_CON[CheckSlot(Inven_CON, bf)];
            bf_count = Count_CON[CheckSlot(Inven_CON, bf)];
        }


        if (org_type == "Quick")
        {
            org_It = Data_Quick[CheckSlot(QuickSlot, org)];
            org_count = Count_Quick[CheckSlot(QuickSlot, org)];
        }
        else if (org_type == "���")
        {
            org_It = Data_WP[CheckSlot(Inven_WP, org)];
        }
        else if (org_type == "�Һ�")
        {
            org_It = Data_CON[CheckSlot(Inven_CON, org)];
            org_count = Count_CON[CheckSlot(Inven_CON, org)];
        }

        //-----------------------------------------------------

        int bf_value = -1;

        if (bf_Type == "Quick")
        {
            bf_value = CheckSlot(QuickSlot, bf);
            ChangeData(bf_value, Data_Quick, org_It, Count_Quick, org_count);
        }
        else if (bf_Type == "���")
        {
            bf_value = CheckSlot(Inven_WP, bf);
            ChangeData(bf_value, Data_WP, org_It, null, org_count);
        }
        else if (bf_Type == "�Һ�")
        {
            bf_value = CheckSlot(Inven_CON, bf);
            ChangeData(bf_value, Data_CON, org_It, Count_CON, org_count);
        }


        int org_value = -1;

        if (org_type == "Quick")
        {
            org_value = CheckSlot(QuickSlot, org);
            ChangeData(org_value, Data_Quick, bf_It, Count_Quick, bf_count);
        }
        else if (org_type == "���")
        {
            org_value = CheckSlot(Inven_WP, org);
            ChangeData(org_value, Data_WP, bf_It, null, bf_count);
        }
        else if (org_type == "�Һ�")
        {
            org_value = CheckSlot(Inven_CON, org);
            ChangeData(org_value, Data_CON, bf_It, Count_CON, bf_count);
        }

        // ������ ------------------------------------

        Transform org_drag = org.childCount > 0 ? org.GetChild(0) : null;
        
        org_drag.transform.SetParent(bf);
        org_drag.transform.localPosition = Vector3.zero;
        org_drag.GetComponent<DragItem>().BeforeParents = org;
        org_drag.GetComponent<DragItem>().originalParent = bf;

        tr.transform.SetParent(org);
        tr.transform.localPosition = Vector3.zero;
        tr.GetComponent<DragItem>().BeforeParents = bf;
        tr.GetComponent<DragItem>().originalParent = org;
    }

    public void MoveSlot(Transform org, Transform bf)
    {
        // ���� ���Թ� ���� ������ Ÿ���� Ȯ��
        string bf_Type = bf.GetComponent<DropSlot>().Type;
        string org_type = org.GetComponent<DropSlot>().Type;

        // ������ ���� �����Ϳ��� ������ ���� �����ͷ� ������ �̵�

        //������ �ű��
        string bf_It = "";
        int bf_count = 0;

        if (bf_Type == "Quick")
        { 
            bf_It = Data_Quick[CheckSlot(QuickSlot, bf)];
            bf_count = Count_Quick[CheckSlot(QuickSlot, bf)];
        }
        else if (bf_Type == "���")
        {
            bf_It = Data_WP[CheckSlot(Inven_WP, bf)];
        }
        else if (bf_Type == "�Һ�")
        {
            bf_It = Data_CON[CheckSlot(Inven_CON, bf)];
            bf_count = Count_CON[CheckSlot(Inven_CON, bf)];
        }


        int org_value = -1;

        if (org_type == "Quick")
        {
            org_value = CheckSlot(QuickSlot, org);
            ChangeData(org_value, Data_Quick, bf_It, Count_Quick, bf_count);
        }
        else if (org_type == "���")
        {
            org_value = CheckSlot(Inven_WP, org);
            ChangeData(org_value, Data_WP, bf_It, null, bf_count);
        }
        else if (org_type == "�Һ�")
        {
            org_value = CheckSlot(Inven_CON, org);
            ChangeData(org_value, Data_CON, bf_It, Count_CON, bf_count);
        }


        // ������ �����
        int bf_value = -1;
        
        if(bf_Type == "Quick") 
        {
            bf_value = CheckSlot(QuickSlot, bf);
            ChangeData(bf_value, Data_Quick, "", Count_Quick, 0);
        }
        else if (bf_Type == "���")
        {
            bf_value = CheckSlot(Inven_WP, bf);
            ChangeData(bf_value, Data_WP, "", null, 0);
        }
        else if (bf_Type == "�Һ�")
        {
            bf_value = CheckSlot(Inven_CON, bf);
            ChangeData(bf_value, Data_CON, "", Count_CON, 0);
        }
    }

    int CheckSlot(List<Transform> slots, Transform tr)
    {
        for(int i = 0; i <= slots.Count-1; i++)
        { if (slots[i] == tr) { return i; } }

        return -1; // ���°�� -1�� ���
    }

    void ChangeData(int value, List<string> ItList, string It, List<int> Counts, int Count)
    {
        // value ��°�� ItList�����͸� It�� ����, Counts�� Count�� ����
        ItList[value] = It;
        if (Counts != null) { Counts[value] = Count; }
    }

    // ���° ��? / ������
    public void Quick_to_Mov(int val, ItemCtrl item)
    {
        if (item.Type == "���")
        {
            int Val = EmptySlot(Data_WP);

            if (Val != -1)
            { 
                // �̵�
                item.transform.SetParent(Inven_WP[Val]);
                item.transform.localPosition = new Vector3(0, 0, 0);

                // �巡�� ����
                DragItem drag = item.GetComponent<DragItem>();
                drag.BeforeParents = drag.originalParent;
                drag.originalParent = Inven_WP[Val];

                // ������ ó�� --------------------------------------------------

                // ���ο� ����
                Data_WP[Val] = Data_Quick[val];
                //Count_WP[val] = Count_Quick[val];

                //�� ������ �̵�
                Data_Quick[val] = "";
                Count_Quick[val] = 0;
            }
            else
            {
                Debug.Log("��� ������ ������ ����á���ϴ�.");
            }
        }
        else if (item.Type == "�Һ�")
        {
            int Val = EmptySlot(Data_CON);

            if (Val != -1)
            {
                // �̵�
                item.transform.SetParent(Inven_CON[Val]);
                item.transform.localPosition = new Vector3(0, 0, 0);

                // �巡�� ����
                DragItem drag = item.GetComponent<DragItem>();
                drag.BeforeParents = drag.originalParent;
                drag.originalParent = Inven_CON[Val];

                // ������ ó�� --------------------------------------------------

                // ���ο� ����
                Data_CON[Val] = Data_Quick[val];
                Count_CON[Val] = Count_Quick[val];

                //�� ������ �̵�
                Data_Quick[val] = "";
                Count_Quick[val] = 0;
            }
            else
            {
                Debug.Log("��� ������ ������ ����á���ϴ�.");
            }
        }

    }

    void Load_Cha()
    {

        Data_Cha Data = SteamSave.Instance.GetLoadedData();

        for (int i = 0; i <= (Data_Quick.Count - 1); i++)
        {
            if (Data.Inven_Quick[i] != "")
            {
                Data_Quick[i] = Data.Inven_Quick[i];
                Count_Quick[i] = Data.Count_Quick[i];

                ItemCtrl it = Instantiate(Item, QuickSlot[i]).GetComponent<ItemCtrl>();
                it.inven = this;
                it.ItList = ItList;
                it.ItemCheck(Data.Inven_Quick[i]);

                it.Count = Count_Quick[i];
                it.txt_Count.text = it.Count.ToString();
            }
        }

        for (int i = 0; i <= (Data_WP.Count - 1); i++)
        {
            if (Data.Inven_WP[i] != "") 
            {
                Data_WP[i] = Data.Inven_WP[i];

                ItemCtrl it = Instantiate(Item, Inven_WP[i]).GetComponent<ItemCtrl>();
                it.inven = this;
                it.ItList = ItList;
                it.ItemCheck(Data.Inven_WP[i]);
            } 
        }

        for (int i = 0; i <= (Data_CON.Count - 1); i++)
        {
            if (Data.Inven_CON[i] != "")
            {
                Data_CON[i] = Data.Inven_CON[i];
                Count_CON[i] = Data.Count_CON[i];

                ItemCtrl it = Instantiate(Item, Inven_CON[i]).GetComponent<ItemCtrl>();
                it.inven = this;
                it.ItList = ItList;
                it.ItemCheck(Data.Inven_CON[i]);

                it.Count = Count_CON[i];
                it.txt_Count.text = it.Count.ToString();
            } 
        }

        for (int i = 0; i <= (Data_MAT.Count - 1); i++)
        {
            if (Data.Inven_MAT[i] != "")
            {
                Data_MAT[i] = Data.Inven_MAT[i];
                Count_MAT[i] = Data.Count_MAT[i];

                ItemCtrl it = Instantiate(Item, Inven_MAT[i]).GetComponent<ItemCtrl>();
                it.inven = this;
                it.ItList = ItList;
                it.ItemCheck(Data.Inven_MAT[i]);

                it.Count = Count_MAT[i];
                it.txt_Count.text = it.Count.ToString();
            }
        }

        for (int i = 0; i <= (Data_VAL.Count - 1); i++)
        {
            if (Data.Inven_VAL[i] != "")
            {
                Data_VAL[i] = Data.Inven_VAL[i];
                Count_VAL[i] = Data.Count_VAL[i];

                ItemCtrl it = Instantiate(Item, Inven_VAL[i]).GetComponent<ItemCtrl>();
                it.inven = this;
                it.ItList = ItList;
                it.ItemCheck(Data.Inven_VAL[i]);

                it.Count = Count_VAL[i];
                it.txt_Count.text = it.Count.ToString();
            }
        }
    }

    public void Save_Cha()
    {
        steam = new GameObject("SteamMgr").AddComponent<SteamManager>().AddComponent<SteamSave>();

        steam.Save_Cha(Data_Quick, Data_WP, Data_CON, Data_MAT, Data_VAL
            , Count_Quick, Count_CON, Count_MAT, Count_VAL, G, Data_Equip, 
            Head_Col, Core_Col, Body_Col, Arms_Col, Legs_Col);
    }

    string SearchItem(string id)
    {
        return ItList.It_Data.FirstOrDefault(x => x.Id == id)?.Types ?? "";
    }
}
