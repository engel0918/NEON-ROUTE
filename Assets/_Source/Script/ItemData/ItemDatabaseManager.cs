using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string Id, Types, Parts, Name, Description;
    public int Delay, BuffTime, MaxHP, MaxMP, HP, MP, STR, INT, DEX, ATK, DEF, Kg;
}

[Serializable]
public class PartData
{
    public string Id, Types, Parts, Name, Description;
    public int Delay, MaxHP, MaxMP, STR, INT, DEX, ATK , DEF, Kg;
}

[Serializable]
public class ItemDatabaseWrapper
{
    public List<ItemData> Items;
}

[Serializable]
public class PartDatabaseWrapper
{
    public List<PartData> Parts;
}

public static class ItemDatabaseManager
{
    // 전역 변수로 데이터 저장
    public static ItemDatabaseWrapper db;

    // ---------------- AES 암호화/복호화 ----------------
    private static byte[] Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = ItemList_KeyMgr.GetUserKey();
        aes.GenerateIV();
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // [IV(16)] + [Cipher]
        return aes.IV.Concat(cipherBytes).ToArray();
    }

    private static string Decrypt(byte[] data)
    {
        using Aes aes = Aes.Create();
        aes.Key = ItemList_KeyMgr.GetUserKey();
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = data.Take(16).ToArray();
        byte[] cipherBytes = data.Skip(16).ToArray();
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor();
        byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
        return Encoding.UTF8.GetString(plainBytes);
    }

    // ---------------- 공개 메서드 ----------------
    public static void LoadDatabase()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "ItemData/items.bytes");

        if (!File.Exists(path))
        {
            Debug.LogError("Encrypted item file not found: " + path);
            db = new ItemDatabaseWrapper { Items = new List<ItemData>() };
            return;
        }

        byte[] encrypted = File.ReadAllBytes(path);
        string json = Decrypt(encrypted);

        if (string.IsNullOrEmpty(json))
        {
            db = new ItemDatabaseWrapper { Items = new List<ItemData>() };
            return;
        }

        db = JsonUtility.FromJson<ItemDatabaseWrapper>(json);
        if (db.Items == null) db.Items = new List<ItemData>();

        Debug.Log($"Item database loaded. Total items: {db.Items.Count}");
    }

    public static ItemData GetItemById(string id)
    {
        if (db == null || db.Items == null) return null;
        return db.Items.Find(item => item.Id == id);
    }

    public static List<ItemData> GetAllItems()
    {
        if (db == null || db.Items == null) return new List<ItemData>();
        return db.Items;
    }

    public static void SaveEncrypted(ItemDatabaseWrapper dbToSave, string path)
    {
        string json = JsonUtility.ToJson(dbToSave, true);
        byte[] encrypted = Encrypt(json);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllBytes(path, encrypted);
        Debug.Log($"Encrypted item data saved to: {path}");
    }
}

public static class PartDatabaseManager
{
    // 전역 변수로 데이터 저장
    public static PartDatabaseWrapper db;

    // ---------------- AES 암호화/복호화 ----------------
    private static byte[] Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = ItemList_KeyMgr.GetUserKey(); // Steam 기반 키
        aes.GenerateIV();
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);

        // [IV(16)] + [Cipher]
        return aes.IV.Concat(cipherBytes).ToArray();
    }

    private static string Decrypt(byte[] data)
    {
        using Aes aes = Aes.Create();
        aes.Key = ItemList_KeyMgr.GetUserKey();
        aes.Padding = PaddingMode.PKCS7;

        byte[] iv = data.Take(16).ToArray();
        byte[] cipherBytes = data.Skip(16).ToArray();
        aes.IV = iv;

        ICryptoTransform decryptor = aes.CreateDecryptor();
        try
        {
            byte[] plainBytes = decryptor.TransformFinalBlock(cipherBytes, 0, cipherBytes.Length);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (Exception e)
        {
            Debug.LogError($"[PartDatabase] Decryption failed. Possibly wrong key or corrupted file. Error: {e.Message}");
            return null;
        }
    }

    // ---------------- 공개 메서드 ----------------
    public static void LoadDatabase()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "PartData/parts.bytes");

        if (!File.Exists(path))
        {
            Debug.LogError("Encrypted part file not found: " + path);
            db = new PartDatabaseWrapper { Parts = new List<PartData>() };
            return;
        }

        byte[] encrypted = File.ReadAllBytes(path);
        string json = Decrypt(encrypted);

        if (string.IsNullOrEmpty(json))
        {
            db = new PartDatabaseWrapper { Parts = new List<PartData>() };
            return;
        }

        db = JsonUtility.FromJson<PartDatabaseWrapper>(json);
        if (db.Parts == null) db.Parts = new List<PartData>();

        Debug.Log($"Part database loaded. Total parts: {db.Parts.Count}");
    }

    public static PartData GetPartById(string id)
    {
        if (db == null || db.Parts == null) return null;
        return db.Parts.Find(part => part.Id == id);
    }

    public static List<PartData> GetAllParts()
    {
        if (db == null || db.Parts == null) return new List<PartData>();
        return db.Parts;
    }

    public static void SaveEncrypted(PartDatabaseWrapper dbToSave, string path)
    {
        string json = JsonUtility.ToJson(dbToSave, true);
        byte[] encrypted = Encrypt(json);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllBytes(path, encrypted);
        Debug.Log($"Encrypted part data saved to: {path}");
    }
}



public static class ItemList_KeyMgr
{
    public static byte[] GetUserKey()
    {
        // 고정 키 문자열 (32바이트 이상 권장)
        string fixedKey = "MySuperSecretFixedKey1234567890!@#"; // 원하는 문자열로 변경 가능

        // SHA256 해시 → 32바이트 AES 키로 변환
        using var sha = SHA256.Create();
        byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(fixedKey));
        return hash; // AES 256bit key
    }
}

