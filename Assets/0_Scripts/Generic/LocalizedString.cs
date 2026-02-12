using System;

[Serializable]
public struct LocalizedString
{
    public CSV_Type tableType;
    public string textID;

    public LocalizedString(CSV_Type tableType, string textID)
    {
        this.tableType = tableType;
        this.textID = textID;
    }
    public override readonly string ToString()
    {
        if (string.IsNullOrEmpty(textID)) return string.Empty;
        return LocalizationManager.Instance.GetText(tableType, textID);
    }

    // string으로의 암시적 형변환
    public static implicit operator string(LocalizedString localizedString)
    {
        return localizedString.ToString();
    }
}