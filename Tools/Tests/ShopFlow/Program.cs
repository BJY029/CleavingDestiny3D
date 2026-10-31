using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Run: dotnet run --project Tools/Tests/ShopFlow
// Deferred tasks exercise real async-void continuations and tab activation tokens.
public static class ShopFlow
{
    public static readonly List<AxeShopItemUI> Items = new();
    private static readonly List<string> Passed = new();
    public static void Set(object obj, string name, object value) =>
        obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).SetValue(obj, value);
    private static T Get<T>(object obj, string name) =>
        (T)obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.NonPublic).GetValue(obj);
    private static void Call(object obj, string name) =>
        obj.GetType().GetMethod(name, BindingFlags.Instance | BindingFlags.NonPublic).Invoke(obj, null);
    private static void Check(bool condition, string name)
    {
        if (!condition) throw new Exception("FAIL: " + name);
        Passed.Add(name);
    }
    public static AxeShopItemUI NewItem()
    {
        var item = new AxeShopItemUI();
        Set(item, "iconImage", new Image());
        Set(item, "nameText", new TextMeshProUGUI());
        Set(item, "priceText", new TextMeshProUGUI());
        Set(item, "buyButton", new Button());
        Set(item, "equipButton", new Button());
        Set(item, "equippedLabel", new GameObject());
        Set(item, "priceGroup", new GameObject());
        Set(item, "soldoutGroup", new GameObject());
        return item;
    }
    private static AxeSkinSO Skin(string id, int localPrice = 99999)
    {
        var skin = new AxeSkinSO();
        Set(skin, "skinId", id); Set(skin, "displayName", id); Set(skin, "price", localPrice);
        return skin;
    }
    private sealed class Fixture
    {
        public readonly UGSManager Server = new();
        public readonly AxeShopController Shop = new();
        public readonly AxeSkinSO Basic = Skin("axe_basic"), Iron = Skin("axe_iron");
        public readonly TextMeshProUGUI Status = new();
        public readonly Button Retry = new();
        public Fixture()
        {
            PlayerProfile.Reset(); Items.Clear(); SaveManager.Saves = 0;
            UGSManager.Instance = Server;
            Server.Prices["axe_iron"] = new("sticks", 1200);
            Server.Purchase = id => Task.FromResult(new UGSManager.PurchaseResponse { status = "purchased" });
            var catalog = new AxeSkinCatalogSO();
            // Missing entries and repeated defaults must neither hide valid skins nor duplicate cards.
            Set(catalog, "skins", new List<AxeSkinSO> { null, Iron, Basic, Iron });
            Set(Shop, "catalog", catalog); Set(Shop, "itemPrefab", NewItem());
            Set(Shop, "contentRoot", new Transform()); Set(Shop, "statusText", Status);
            Set(Shop, "retryButton", Retry);
        }
        public void Open() { Shop.gameObject.SetActive(true); Call(Shop, "OnEnable"); }
        public void Close() { Call(Shop, "OnDisable"); Shop.gameObject.SetActive(false); }
        public AxeShopItemUI IronItem => Items[1];
    }
    public static void Main()
    {
        System.Globalization.CultureInfo.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;
        var f = new Fixture();
        var catalogGate = new TaskCompletionSource();
        f.Server.CatalogLoad = () => catalogGate.Task;
        f.Open();
        Check(f.Shop.IsBusy && !f.Shop.IsShopReady && !Get<Button>(f.IronItem, "buyButton").interactable, "loading locks actions");
        Check(Get<TextMeshProUGUI>(f.IronItem, "priceText").text == "불러오는 중…", "local SO price never displayed");
        f.Shop.RetryLoad();
        Check(f.Server.CatalogCalls == 1 && Items.Count == 2, "deduplicates requests and cards; skips null catalog entries");
        catalogGate.SetResult();
        Check(f.Shop.IsShopReady && !f.Shop.IsBusy && f.Server.WalletCalls == 0, "reuses the wallet prepared at startup");
        Check(Get<TextMeshProUGUI>(f.IronItem, "priceText").text == "1,200 Sticks", "server price and existing currency");
        f.Server.PlayerData["sticks"] = 1199;
        Check(!f.Shop.CanPurchase(f.Iron), "insufficient balance blocks purchase");
        f.Server.PlayerData["sticks"] = 1200;
        Check(f.Shop.CanPurchase(f.Iron), "exact balance allows purchase");
        f.Server.Prices["axe_iron"] = new("coins", 500);
        f.Shop.RetryLoad();
        Check(Get<TextMeshProUGUI>(f.IronItem, "priceText").text == "500 Coin", "uses server currency instead of forcing sticks");
        f.Server.Prices.Remove("axe_iron"); f.Shop.RetryLoad();
        Check(!f.Shop.CanPurchase(f.Iron) && Get<TextMeshProUGUI>(f.IronItem, "priceText").text == "판매 정보 없음", "missing price disables sale");
        Check(!f.Shop.CanPurchase(Skin("unknown")), "rejects items outside local catalog");
        f.Close();

        f = new Fixture(); f.Server.CatalogLoad = () => Task.FromException(new Exception("offline")); f.Open();
        Check(!f.Shop.IsShopReady && f.Retry.gameObject.activeInHierarchy && f.Status.text.Contains("조회 실패"), "load failure exposes retry");
        f.Server.CatalogLoad = () => Task.CompletedTask; f.Retry.onClick.Invoke();
        Check(f.Shop.IsShopReady && !f.Retry.gameObject.activeInHierarchy && f.Server.WalletCalls == 1, "retry recovers load and refreshes server wallet");
        var buyGate = new TaskCompletionSource<UGSManager.PurchaseResponse>();
        f.Server.Purchase = id => buyGate.Task;
        f.Shop.TryPurchase(f.Iron); f.Shop.TryPurchase(f.Iron); f.Shop.TryEquip(f.Basic);
        Check(f.Server.PurchaseCalls == 1 && f.Shop.IsBusy && SaveManager.Saves == 0, "purchase locks repeated clicks and equip");
        buyGate.SetResult(new() { status = "purchased" });
        Check(f.Shop.GetSkinState(f.Iron) == AxeSkinState.Owned && PlayerProfile.EquippedAxeSkinId == "axe_basic", "purchase owns item without auto equip");
        Check(!Get<GameObject>(f.IronItem, "priceGroup").activeInHierarchy && Get<GameObject>(f.IronItem, "soldoutGroup").activeInHierarchy, "owned card hides price and shows ownership");
        f.Shop.TryEquip(f.Iron);
        Check(SaveManager.Saves == 1 && f.Shop.GetSkinState(f.Iron) == AxeSkinState.Equipped && Get<GameObject>(f.IronItem, "equippedLabel").activeInHierarchy, "equip saves and updates equipped label");
        f.Close();

        foreach (var status in new[] { "already_owned", "insufficient_funds", "skin_not_found" })
        {
            f = new Fixture(); f.Server.Purchase = id => Task.FromResult(new UGSManager.PurchaseResponse { status = status }); f.Open(); f.Shop.TryPurchase(f.Iron);
            Check(!f.Shop.IsBusy && f.Status.text == (status == "already_owned" ? "이미 보유한 도끼입니다." : status == "insufficient_funds" ? "나뭇가지가 부족합니다." : "판매 정보가 없습니다."), "purchase response: " + status);
            f.Close();
        }
        foreach (var recovery in new[] { "owned", "not_owned", "offline" })
        {
            f = new Fixture(); f.Open();
            f.Server.Purchase = id => Task.FromException<UGSManager.PurchaseResponse>(new Exception("lost response"));
            f.Server.WalletLoad = () => { if (recovery == "offline") return Task.FromException(new Exception("offline")); if (recovery == "owned") PlayerProfile.Own("axe_iron"); return Task.CompletedTask; };
            f.Shop.TryPurchase(f.Iron);
            Check(f.Server.WalletCalls == 1 && f.Status.text == (recovery == "owned" ? "구매 상태를 복구했습니다." : recovery == "not_owned" ? "구매 실패 · 다시 시도해 주세요." : "구매 확인 실패 · 잔액을 다시 조회해 주세요."), "lost purchase response recovery: " + recovery);
            if (recovery == "offline") Check(!f.Shop.IsShopReady && f.Retry.gameObject.activeInHierarchy, "failed reconciliation locks shop");
            f.Close();
        }
        f = new Fixture(); catalogGate = new(); f.Server.CatalogLoad = () => catalogGate.Task; f.Open(); f.Close(); f.Open(); catalogGate.SetResult();
        Check(f.Shop.IsShopReady && f.Server.CatalogCalls == 2 && Items.Count == 2, "close and reopen during load restarts once without duplicate cards");
        f.Close();
        f = new Fixture(); f.Open(); buyGate = new(); f.Server.Purchase = id => buyGate.Task; f.Shop.TryPurchase(f.Iron); f.Close(); f.Open(); buyGate.SetResult(new() { status = "purchased" });
        Check(f.Shop.IsShopReady && f.Status.text == "" && f.Server.CatalogCalls == 2 && f.Server.PurchaseCalls == 1, "stale purchase cannot overwrite reopened tab; fresh load reconciles");
        f.Close();
        f = new Fixture(); f.Open(); f.Close(); f.Open();
        Check(f.Shop.IsShopReady && f.Server.WalletCalls == 0, "opening and reopening prepared shop needs no wallet request");
        f.Shop.RetryLoad();
        Check(f.Server.WalletCalls == 1, "explicit retry still refreshes wallet");
        f.Close();
        f = new Fixture(); f.Server.IsWalletReady = false; f.Open();
        Check(f.Server.WalletCalls == 1 && !f.Shop.IsShopReady, "unprepared wallet is fetched and shop stays locked until ready");
        f.Close();
        Console.WriteLine(string.Join(Environment.NewLine, Passed.Select(x => "PASS " + x)));
        Console.WriteLine(Passed.Count + " checks passed; no network, purchases or player-save writes.");
    }
}
