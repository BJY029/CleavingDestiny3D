using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;


public static class SaveCrypto
{
    //로컬 클라이언트에 있는 키이므로 절대적 보안 보장 불가
    //일반적인 Save 파일 직접 수정 난이도를 올리는 목적
    //출시 후 삭제
    private const string Secret = "WPQKFCNLDJQWHATLZUWNTPDY";

    private static readonly byte[] encryptionKey;
    private static readonly byte[] hmackey;

    static SaveCrypto()
    {
        using SHA512 sha512 = SHA512.Create();

        byte[] secretBytes = Encoding.UTF8.GetBytes(Secret);
        byte[] hash = sha512.ComputeHash(secretBytes);

        encryptionKey = new byte[32];
        hmackey = new byte[32];

        Buffer.BlockCopy(hash, 0, encryptionKey, 0, 32);
        Buffer.BlockCopy(hash, 32, hmackey, 0, 32);
    }

    public static string Encrypt(string plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            throw new ArgumentException("암호화 할 데이터가 없습니다.");

        using Aes aes = Aes.Create();

        aes.KeySize = 256;
        aes.BlockSize = 128;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        aes.Key = encryptionKey;
        aes.GenerateIV();

        byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
        byte[] cipherBytes;

        using (MemoryStream memoryStream = new())
        {
            using CryptoStream cryptoStream = new(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write);

            cryptoStream.Write(plainBytes, 0, plainBytes.Length);
            cryptoStream.FlushFinalBlock();

            cipherBytes = memoryStream.ToArray();
        }

        byte[] macBytes = CreateMac(aes.IV, cipherBytes);

        EncryptedSaveEnvelope envelope = new()
        {
            iv = Convert.ToBase64String(aes.IV),
            cipherText = Convert.ToBase64String(cipherBytes),
            mac = Convert.ToBase64String(macBytes)
        };

        return JsonUtility.ToJson(envelope);
    }

    public static bool TryDecrypt(string encryptedData, out string plainText)
    {
        plainText = null;

        if (string.IsNullOrWhiteSpace(encryptedData))
            return false;

        try
        {
            EncryptedSaveEnvelope envelope =
                JsonUtility.FromJson<EncryptedSaveEnvelope>(encryptedData);

            if (envelope == null)
                return false;

            if (string.IsNullOrWhiteSpace(envelope.iv) ||
                string.IsNullOrWhiteSpace(envelope.cipherText) ||
                string.IsNullOrWhiteSpace(envelope.mac))
            {
                return false;
            }

            byte[] iv = Convert.FromBase64String(envelope.iv);
            byte[] cipherBytes = Convert.FromBase64String(envelope.cipherText);
            byte[] savedMac = Convert.FromBase64String(envelope.mac);

            byte[] calculatedMac = CreateMac(iv, cipherBytes);

            if (!FixedTimeEquals(savedMac, calculatedMac))
            {
                Debug.LogWarning("[SaveCrypto] Save 무결성 검증 실패");
                return false;
            }

            using Aes aes = Aes.Create();

            aes.KeySize = 256;
            aes.BlockSize = 128;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = encryptionKey;
            aes.IV = iv;

            using MemoryStream memoryStream = new(cipherBytes);

            using CryptoStream cryptoStream = new(
                memoryStream,
                aes.CreateDecryptor(),
                CryptoStreamMode.Read
            );

            using StreamReader reader = new(cryptoStream, Encoding.UTF8);

            plainText = reader.ReadToEnd();

            return true;
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[SaveCrypto] 복호화 실패 : {e.Message}");
            return false;
        }
    }

    private static byte[] CreateMac(byte[] iv, byte[] cipherBytes)
    {
        byte[] combined = new byte[iv.Length + cipherBytes.Length];

        Buffer.BlockCopy(iv, 0, combined, 0, iv.Length);
        Buffer.BlockCopy(cipherBytes, 0, combined, iv.Length, cipherBytes.Length);

        using HMACSHA256 hmac = new(hmackey);

        return hmac.ComputeHash(combined);
    }

    private static bool FixedTimeEquals(byte[] a, byte[] b)
    {
        if (a == null || b == null)
            return false;

        if (a.Length != b.Length)
            return false;

        int difference = 0;

        for (int i = 0; i < a.Length; i++)
        {
            difference |= a[i] ^ b[i];
        }

        return difference == 0;
    }
}
