using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[Serializable]
public class ItemData
{
    public string Id, Types, Parts, Name, Description;
    public int MaxHP, MaxMP, HP, MP, STR, INT, DEX, ATK, DEF, Kg;
}

[Serializable]
public class ItemDatabaseWrapper
{
    public List<ItemData> Items;
}

public static class ItemDatabaseManager
{
    // 전역 변수로 데이터 저장
    public static ItemDatabaseWrapper db;

    private static readonly string key = "YourSecretKey1234567890123456"; // 32byte AES key

    // ---------------- AES 암호화/복호화 ----------------
    private static byte[] GetAesKey(string keyString, int size = 32)
    {
        byte[] keyBytes = Encoding.UTF8.GetBytes(keyString);
        Array.Resize(ref keyBytes, size);
        return keyBytes;
    }

    private static byte[] Encrypt(string plainText)
    {
        using Aes aes = Aes.Create();
        aes.Key = GetAesKey(key, 32);
        aes.IV = new byte[16];
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform encryptor = aes.CreateEncryptor();
        byte[] bytes = Encoding.UTF8.GetBytes(plainText);
        return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
    }

    private static string Decrypt(byte[] cipherText)
    {
        using Aes aes = Aes.Create();
        aes.Key = GetAesKey(key, 32);
        aes.IV = new byte[16];
        aes.Padding = PaddingMode.PKCS7;

        ICryptoTransform decryptor = aes.CreateDecryptor();
        try
        {
            byte[] decrypted = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            return Encoding.UTF8.GetString(decrypted);
        }
        catch (Exception e)
        {
            Debug.LogError($"Decryption failed. Possibly wrong key or corrupted file. Error: {e.Message}");
            return null;
        }
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
