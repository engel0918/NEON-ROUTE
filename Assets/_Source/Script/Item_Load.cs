using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Items_
{
    public string Id, Types, Parts, Name, Description;
    public int Delay, BuffTime, MaxHP, MaxMP, HP, MP, STR, INT, DEX, ATK, DEF, Kg;
}

[System.Serializable]
public class Parts_
{
    public string Id, Name, Types, Parts, Description;
    public int MaxHP, MaxMP, STR, INT, DEX, ATK, DEF, Kg;
}

[System.Serializable]
public class Thum
{
    public string Id;
    public Sprite Thumnail;
}

public class Item_Load : MonoBehaviour
{
    public List<Items_> It_Data;
    public List<Parts_> Pt_Data;

    public List<Thum> thums;


    void Awake()
    {
        Load_AllItem();
    }

    void Load_ItemCD(string str)
    {
        // 전역 변수로 한 번만 로드
        ItemDatabaseManager.LoadDatabase();

        var item = ItemDatabaseManager.GetItemById(str);
   }

    void Load_AllItem()
    {
        // 전역 변수로 한 번만 로드
        ItemDatabaseManager.LoadDatabase();
        PartDatabaseManager.LoadDatabase();

        List<ItemData> allItems = ItemDatabaseManager.GetAllItems();
        if (allItems.Count == 0)
        {
            Debug.LogWarning("Item list is empty!");
            return;
        }

        List<PartData> allParts = PartDatabaseManager.GetAllParts();

        if (allParts.Count == 0)
        {
            Debug.LogWarning("Item list is empty!");
            return;
        }

        It_Data.Clear();
        It_Data = new List<Items_>();

        foreach (var item in allItems)
        {
            Items_ data = new Items_()
            {
                Id = item.Id,
                Name = item.Name,
                Types = item.Types,
                Parts = item.Parts,
                Description = item.Description,
                Delay = item.Delay,
                BuffTime = item.BuffTime,
                MaxHP = item.MaxHP,
                MaxMP = item.MaxMP,
                HP = item.HP,
                MP = item.MP,
                STR = item.STR,
                INT = item.INT,
                DEX = item.DEX,
                ATK = item.ATK,
                DEF = item.DEF,
                Kg = item.Kg
            };

            It_Data.Add(data);
        }


        Pt_Data.Clear();
        Pt_Data = new List<Parts_>();

        foreach (var part in allParts)
        {
            Parts_ data = new Parts_()
            {
                Id = part.Id,
                Name = part.Name,
                Types = part.Types,
                Parts = part.Parts,
                Description = part.Description,
                MaxHP = part.MaxHP,
                MaxMP = part.MaxMP,
                STR = part.STR,
                INT = part.INT,
                DEX = part.DEX,
                ATK = part.ATK,
                DEF = part.DEF,
                Kg = part.Kg
            };

            Pt_Data.Add(data);
        }
    }

}