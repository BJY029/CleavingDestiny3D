using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public sealed class AxeShopPrice
{
    public string Currency { get; }
    public int Price { get; }

    public AxeShopPrice(string currency, int price)
    {
        Currency = currency;
        Price = price;
    }

    // PurchaseSkin과 같은 SKIN_CATALOG 형식만 사용하며 SO 가격으로 대체하지 않습니다.
    public static Dictionary<string, AxeShopPrice> ParseCatalog(string json)
    {
        JToken token = JToken.Parse(json);
        if (token.Type == JTokenType.String) token = JToken.Parse(token.Value<string>());
        if (!(token is JObject catalog) || catalog.Count == 0)
            throw new InvalidOperationException("SKIN_CATALOG가 없거나 비어 있습니다.");

        var prices = new Dictionary<string, AxeShopPrice>(StringComparer.Ordinal);
        foreach (var property in catalog.Properties())
        {
            if (!(property.Value is JObject item) || item["price"]?.Type != JTokenType.Integer)
                throw new InvalidOperationException("잘못된 스킨 가격 형식: " + property.Name);
            string currency = (string)item["currency"];
            long price = item["price"].Value<long>();
            if ((currency != "sticks" && currency != "coins") || price < 0 || price > int.MaxValue)
                throw new InvalidOperationException("잘못된 스킨 재화/가격: " + property.Name);
            prices.Add(property.Name, new AxeShopPrice(currency, (int)price));
        }
        return prices;
    }
}
