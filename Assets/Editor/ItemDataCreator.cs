#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class ItemDataCreator
{
    [MenuItem("Tools/Encrypt Item Data (from CSV)")]
    public static void SaveFromCSV()
    {
        string ItData_csvPath = Path.Combine(Application.dataPath, "_Source/Script/ItemData/items.csv");

        string PtData_csvPath = Path.Combine(Application.dataPath, "_Source/Script/ItemData/Parts.csv");


        if (!File.Exists(ItData_csvPath) && !File.Exists(PtData_csvPath))
        {
            Debug.LogError("CSV file not found: " + ItData_csvPath);
            return;
        }


        var It_db = new ItemDatabaseWrapper { Items = new List<ItemData>() };
        string[] It_lines = File.ReadAllLines(ItData_csvPath);

        for (int i = 1; i < It_lines.Length; i++) // 첫 줄은 헤더
        {
            string[] cols = It_lines[i].Split(',');
            if (cols.Length < 3) continue;

            It_db.Items.Add(new ItemData
            {
                //Id = TryParseInt(cols[0]),
                Id = cols[0],
                Types = cols[1],
                Parts = cols[2],
                Name = cols[3],
                Description = cols[4],

                Delay = TryParseInt(cols[5]),
                BuffTime = TryParseInt(cols[6]),
                MaxHP = TryParseInt(cols[7]),
                MaxMP = TryParseInt(cols[8]),
                HP = TryParseInt(cols[9]),
                MP = TryParseInt(cols[10]),
                STR = TryParseInt(cols[11]),
                INT = TryParseInt(cols[12]),
                DEX = TryParseInt(cols[13]),
                ATK = TryParseInt(cols[14]),
                DEF = TryParseInt(cols[15]),
                Kg = TryParseInt(cols[16])
            });
        }

        string It_outDir = Path.Combine(Application.streamingAssetsPath, "ItemData");
        Directory.CreateDirectory(It_outDir);
        string It_outPath = Path.Combine(It_outDir, "items.bytes");

        ItemDatabaseManager.SaveEncrypted(It_db, It_outPath);



        var Pt_db = new PartDatabaseWrapper { Parts = new List<PartData>() };
        string[] Pt_lines = File.ReadAllLines(PtData_csvPath);

        for (int i = 1; i < Pt_lines.Length; i++) // 첫 줄은 헤더
        {
            string[] cols = Pt_lines[i].Split(',');
            if (cols.Length < 3) continue;

            Pt_db.Parts.Add(new PartData
            {
                //Id = TryParseInt(cols[0]),
                Id = cols[0],
                Types = cols[1],
                Parts = cols[2],
                Name = cols[3],
                Description = cols[4],

                Delay = TryParseInt(cols[5]),
                MaxHP = TryParseInt(cols[6]),
                MaxMP = TryParseInt(cols[7]),
                STR = TryParseInt(cols[8]),
                INT = TryParseInt(cols[9]),
                DEX = TryParseInt(cols[10]),
                ATK = TryParseInt(cols[11]),
                DEF = TryParseInt(cols[12]),
                Kg = TryParseInt(cols[13])
            });
        }

        string Pt_outDir = Path.Combine(Application.streamingAssetsPath, "PartData");
        Directory.CreateDirectory(Pt_outDir);
        string Pt_outPath = Path.Combine(Pt_outDir, "Parts.bytes");

        PartDatabaseManager.SaveEncrypted(Pt_db, Pt_outPath);
    }

    private static int TryParseInt(string value)
    {
        if (int.TryParse(value, out int result))
            return result;
        Debug.LogWarning($"정수 변환 실패: '{value}'");
        return 0; // 기본값 또는 원하는 fallback 값
    }

}
#endif
