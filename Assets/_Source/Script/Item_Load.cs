using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Items
{
    public string Id, Name, Types, Parts, Description;
    public int MaxHP, MaxMP, HP, MP, STR, INT, DEX, ATK, DEF, Kg;
}

[System.Serializable]
public class Thum
{
    public string Id;
    public Sprite Thumnail;
}

public class Item_Load : MonoBehaviour
{
    public List<Items> It_Data;
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

        List<ItemData> allItems = ItemDatabaseManager.GetAllItems();
        if (allItems.Count == 0)
        {
            Debug.LogWarning("Item list is empty!");
            return;
        }

        It_Data.Clear();
        It_Data = new List<Items>();

        foreach (var item in allItems)
        {
            //Debug.Log($"ID:{item.Id}, Name:{item.Name}, Desc:{item.Description}, MaxHP:{item.MaxHP}, " +
            //    $"MaxMP:{item.MaxMP}, STR:{item.STR}, INT:{item.INT}, DEX:{item.DEX}, ATK:{item.ATK}, DEF:{item.DEF}, Kg:{item.Kg}");

            Items data = new Items()
            {
                Id = item.Id,
                Name = item.Name,
                Types = item.Types,
                Parts = item.Parts,
                Description = item.Description,
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
    }

}