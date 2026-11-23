var ui = new TreeCanvasController
{
    TreeHPSlider = new UnityEngine.UI.Slider(),
    TreeHP = new TMPro.TextMeshProUGUI()
};
int passed = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
    Console.WriteLine("PASS " + name);
    passed++;
}
// Reproduce the master calling UI initialization before Photon properties arrive.
ui.UpdateTreeHP(0f, 0f);
Check(float.IsFinite(ui.TreeHPSlider.value), "uninitialized HP cannot send NaN to slider");
ui.UpdateTreeHP(30000f, 30000f);
Check(ui.TreeHPSlider.value == 1f && ui.TreeHP.text == "30000", "room initialization fills bar");
ui.UpdateTreeHP(30000f, 15000f);
Check(ui.TreeHPSlider.value == 0.5f && ui.TreeHP.text == "15000", "damage updates ratio and text");
foreach (var pair in new[] { (0f, 1f), (-1f, 1f), (float.NaN, 1f),
    (float.PositiveInfinity, 1f), (30000f, float.NaN), (30000f, float.NegativeInfinity) })
{
    ui.UpdateTreeHP(pair.Item1, pair.Item2);
    Check(ui.TreeHPSlider.value == 0.5f && ui.TreeHP.text == "15000", "invalid HP preserves valid UI " + pair);
}
ui.UpdateTreeHP(30000f, 0f);
Check(ui.TreeHPSlider.value == 0f && ui.TreeHP.text == "0", "destroyed tree displays empty bar");
Console.WriteLine($"{passed} checks passed.");

// Only the UI surface is stubbed; tests execute the production controller above.
namespace UnityEngine
{
    public class MonoBehaviour
    {
        public object gameObject;
        public static void Destroy(object value) { }
    }
}
namespace UnityEngine.UI
{
    public class Slider { public float value; }
}
namespace TMPro
{
    public class TextMeshProUGUI { public string text; }
}
