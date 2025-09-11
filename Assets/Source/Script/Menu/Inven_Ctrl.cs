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

    // id, 타입, 갯수
    public bool GetItem(string id, string type, int Value)
    {
        if (type == "장비")
        {
            int Val = EmptySlot(Data_WP);

            if (Val != -1)
            {
                ItemCtrl it = Instantiate(Item, Inven_WP[Val]).GetComponent<ItemCtrl>();
                it.inven = this;
                it.ItList = ItList;
                it.ItemCheck(id);

                // 데이터 처리 --------------------------------------------------
                Data_WP[Val] = id;
                return true;
            }
            else
            {
                Debug.Log("장비 아이템 슬롯이 가득찼습니다.");
                return false;
            }
        }
        else if (type == "소비")
        {
            int Count = CountCheck(id, Data_CON);
            int QuickCount = CountCheck(id, Data_Quick);

            if (Count != -1)
            {
                // 데이터 처리 --------------------------------------------------
                Data_CON[Count] = id;
                Count_CON[Count] += Value;

                ItemCtrl itCtrl = Inven_CON[Count].GetChild(0).GetComponent<ItemCtrl>();
                itCtrl.Count = Count_CON[Count];
                itCtrl.txt_Count.text = itCtrl.Count.ToString();

                return true;
            }
            else if(QuickCount != -1)
            {
                // 데이터 처리 --------------------------------------------------
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

                    // 데이터 처리 --------------------------------------------------
                    Data_CON[Val] = id;
                    Count_CON[Val] += Value;

                    ItemCtrl itCtrl = Inven_CON[Val].GetChild(0).GetComponent<ItemCtrl>();
                    itCtrl.Count = Count_CON[Val];
                    itCtrl.txt_Count.text = itCtrl.Count.ToString();

                    return true;
                }
                else
                {
                    Debug.Log("소비 아이템 슬롯이 가득찼습니다.");
                    return false;
                }
            }
        }
        else
        {
            // 다른 타입 처리 (예: 소비형, 재료 등)
            Debug.Log($"알 수 없는 아이템 타입: {type}");
            return false;
        }
    }

    int EmptySlot(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i] == "")
            {
                return i; // 빈 슬롯 발견 시 인덱스 반환
            }
        }
        return -1; // 빈 슬롯이 없는 경우 -1 반환
    }

    int CountCheck(string str, List<string> list)
    {
        for(int i = 0; i <= (list.Count-1); i++)
        { if (list[i] == str) { return i; } }

        return -1; // 못 찾으면 -1 반환
    }

    public void MoveSlot(Transform org, Transform bf)
    {
        // 이전 슬롯및 현재 슬롯의 타입을 확인
        string bf_Type = bf.GetComponent<DropSlot>().Type;
        string org_type = org.GetComponent<DropSlot>().Type;

        // 이전의 슬롯 데이터에서 현재의 슬롯 데이터로 데이터 이동

        //데이터 옮기기
        string bf_It = "";
        int bf_count = 0;

        if (bf_Type == "Quick")
        { 
            bf_It = Data_Quick[CheckSlot(QuickSlot, bf)];
            bf_count = Count_Quick[CheckSlot(QuickSlot, bf)];
        }
        else if (bf_Type == "장비")
        {
            bf_It = Data_WP[CheckSlot(Inven_WP, bf)];
        }
        else if (bf_Type == "소비")
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
        else if (org_type == "장비")
        {
            org_value = CheckSlot(Inven_WP, org);
            ChangeData(org_value, Data_WP, bf_It, null, bf_count);
        }
        else if (org_type == "소비")
        {
            org_value = CheckSlot(Inven_CON, org);
            ChangeData(org_value, Data_CON, bf_It, Count_CON, bf_count);
        }


        // 데이터 지우기
        int bf_value = -1;
        
        if(bf_Type == "Quick") 
        {
            bf_value = CheckSlot(QuickSlot, bf);
            ChangeData(bf_value, Data_Quick, "", Count_Quick, 0);
        }
        else if (bf_Type == "장비")
        {
            bf_value = CheckSlot(Inven_WP, bf);
            ChangeData(bf_value, Data_WP, "", null, 0);
        }
        else if (bf_Type == "소비")
        {
            bf_value = CheckSlot(Inven_CON, bf);
            ChangeData(bf_value, Data_CON, "", Count_CON, 0);
        }
    }

    int CheckSlot(List<Transform> slots, Transform tr)
    {
        for(int i = 0; i <= slots.Count-1; i++)
        { if (slots[i] == tr) { return i; } }

        return -1; // 없는경우 -1을 출력
    }

    void ChangeData(int value, List<string> ItList, string It, List<int> Counts, int Count)
    {
        // value 번째의 ItList데이터를 It로 변경, Counts를 Count로 변경
        ItList[value] = It;
        if (Counts != null) { Counts[value] = Count; }
    }
}
