using System;

[Serializable]
public class EncryptedSaveEnvelope
{
    public string iv;
    public string cipherText;
    public string mac;
}
