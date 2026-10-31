// Offline collaborators for the actual production shop sources linked by the project.
// No Unity session, account, network requests, purchases, or player saves are used.
using System.Reflection;

namespace UnityEngine
{
    public class Object
    {
        public static T Instantiate<T>(T original, Transform parent) where T : Object
        {
            var item = ShopFlow.NewItem();
            ShopFlow.Items.Add(item);
            return (T)(Object)item;
        }
    }
    public class ScriptableObject : Object { }
    public class GameObject : Object
    {
        public bool activeInHierarchy = true;
        public void SetActive(bool active) => activeInHierarchy = active;
    }
    public class Transform : Object { }
    public class Sprite : Object { }
    public class MonoBehaviour : Object
    {
        public GameObject gameObject = new();
        public bool isActiveAndEnabled => gameObject.activeInHierarchy;
    }
    public class SerializeField : Attribute { }
    public class HeaderAttribute(string name) : Attribute { }
    public class TextAreaAttribute : Attribute { }
    public class TooltipAttribute(string text) : Attribute { }
    public class CreateAssetMenuAttribute : Attribute
    {
        public string fileName, menuName;
    }
    public static class Debug
    {
        public static void LogException(Exception e) { }
    }
}
namespace TMPro
{
    public class TextMeshProUGUI : UnityEngine.MonoBehaviour { public string text; }
}
namespace UnityEngine.UI
{
    public class Image : UnityEngine.MonoBehaviour { public UnityEngine.Sprite sprite; }
    public class Button : UnityEngine.MonoBehaviour
    {
        public bool interactable = true;
        public ClickEvent onClick = new();
        public class ClickEvent
        {
            private event Action Click;
            public void AddListener(Action action) => Click += action;
            public void RemoveListener(Action action) => Click -= action;
            public void Invoke() => Click?.Invoke();
        }
    }
}
public sealed class AxeShopPrice(string currency, int price)
{
    public string Currency => currency;
    public int Price => price;
}
public class UGSManager
{
    public static UGSManager Instance;
    public bool IsWalletReady = true;
    public Dictionary<string, int> PlayerData = new() { ["sticks"] = 5000, ["coins"] = 1000 };
    public Dictionary<string, AxeShopPrice> Prices = new();
    public event Action WalletChanged;
    public Func<Task> CatalogLoad = () => Task.CompletedTask;
    public Func<Task> WalletLoad = () => Task.CompletedTask;
    public Func<string, Task<PurchaseResponse>> Purchase;
    public int CatalogCalls, WalletCalls, PurchaseCalls;
    public Task LoadSkinCatalogAsync() { CatalogCalls++; return CatalogLoad(); }
    public Task LoadPlayerData() { WalletCalls++; return WalletLoad(); }
    public async Task LoadShopDataAsync(bool refreshWallet = false)
    {
        await LoadSkinCatalogAsync();
        if (refreshWallet || !IsWalletReady) await LoadPlayerData();
    }
    public bool TryGetSkinPrice(string id, out AxeShopPrice price) => Prices.TryGetValue(id, out price);
    public async Task<PurchaseResponse> PurchaseSkinAsync(string id)
    {
        PurchaseCalls++;
        var result = await Purchase(id);
        if (result.status is "purchased" or "already_owned") PlayerProfile.Own(id);
        WalletChanged?.Invoke();
        return result;
    }
    public class PurchaseResponse { public string status; }
}
public static class PlayerProfile
{
    private static readonly HashSet<string> Owned = new();
    public static string EquippedAxeSkinId = "axe_basic";
    public static event Action OnAxeSkinChanged;
    public static bool OwnsAxeSkin(string id) => id != null && Owned.Contains(id);
    public static void Own(string id) { Owned.Add(id); OnAxeSkinChanged?.Invoke(); }
    public static bool EquipAxeSkin(string id)
    {
        if (!Owned.Contains(id) || EquippedAxeSkinId == id) return false;
        EquippedAxeSkinId = id;
        OnAxeSkinChanged?.Invoke();
        return true;
    }
    public static void Reset()
    {
        Owned.Clear(); Owned.Add("axe_basic"); EquippedAxeSkinId = "axe_basic"; OnAxeSkinChanged = null;
    }
}
public static class SaveManager { public static int Saves; public static void Save() => Saves++; }
public class AudioManager
{
    public static AudioManager Instance = new();
    public void PlaySfx2D(string id) { }
}
