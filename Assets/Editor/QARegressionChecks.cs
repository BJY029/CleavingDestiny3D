#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using Option;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

/// <summary>Isolated Editor checks: no player preferences, accounts, or save files.</summary>
public static class QARegressionChecks
{
    private static readonly BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
    private static T Get<T>(object target, string name) => (T)target.GetType().GetField(name, Fields).GetValue(target);
    private static void Check(bool value, string name, List<string> passed)
    {
        if (!value) throw new InvalidOperationException("FAIL: " + name);
        passed.Add(name);
    }

    public static List<string> KeyBindings()
    {
        if (EditorApplication.isPlaying) throw new InvalidOperationException("Run outside Play Mode.");
        var passed = new List<string>();
        var root = new GameObject("QA isolated key bindings") { hideFlags = HideFlags.HideAndDontSave };
        var keyboard = InputSystem.AddDevice<Keyboard>();
        var asset = ScriptableObject.CreateInstance<InputActionAsset>();
        InputActionReference reference = null;
        try
        {
            var map = asset.AddActionMap("QA");
            var action = map.AddAction("Interact", InputActionType.Button, "<Keyboard>/f");
            reference = InputActionReference.Create(action);
            var element = root.AddComponent<KeyBindElement>();
            var option = root.AddComponent<KeyBindOption>();
            var item = new KeyBindItem { actionReference = reference, element = element };
            option.KeyBindItems.Add(item);
            action.Enable();
            string before = asset.SaveBindingOverridesAsJson();
            option.StartRebinding(item);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Escape));
            InputSystem.Update();
            Check(Get<InputActionRebindingExtensions.RebindingOperation>(option, "_rebindOperation") == null,
                "ESC cancels the real Input System operation", passed);
            Check(action.enabled && action.bindings[0].effectivePath == "<Keyboard>/f"
                && asset.SaveBindingOverridesAsJson() == before, "ESC preserves enabled state and overrides", passed);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); InputSystem.Update();
            for (int i = 0; i < 20; i++)
            {
                option.StartRebinding(item);
                option.CancelRebinding();
                option.CancelRebinding();
            }
            Check(action.enabled && action.bindings[0].effectivePath == "<Keyboard>/f",
                "twenty repeated cancellations are idempotent", passed);
            action.Disable();
            option.StartRebinding(item); option.CancelRebinding();
            Check(!action.enabled, "cancel preserves originally disabled action", passed);
            action.Enable();
            option.StartRebinding(item);
            option.SendMessage("OnDisable"); option.SendMessage("OnDestroy");
            Check(action.enabled && Get<InputActionRebindingExtensions.RebindingOperation>(option, "_rebindOperation") == null,
                "disable then destroy cleans up once", passed);
            option.StartRebinding(item);
            Get<InputActionRebindingExtensions.RebindingOperation>(option, "_rebindOperation").OnMatchWaitForAnother(0);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.G)); InputSystem.Update();
            Check(Get<InputActionRebindingExtensions.RebindingOperation>(option, "_rebindOperation") == null
                && action.bindings[0].effectivePath == "<Keyboard>/g", "normal key completes without anyKey override", passed);
            string changed = asset.SaveBindingOverridesAsJson();
            InputSystem.QueueStateEvent(keyboard, new KeyboardState()); InputSystem.Update();
            option.StartRebinding(item);
            InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.Escape)); InputSystem.Update();
            Check(asset.SaveBindingOverridesAsJson() == changed && action.enabled,
                "cancel preserves a previously customized key", passed);
            action.RemoveAllBindingOverrides(); asset.LoadBindingOverridesFromJson(changed);
            Check(action.bindings[0].effectivePath == "<Keyboard>/g", "custom override round trips through saved JSON", passed);
        }
        finally
        {
            UnityEngine.Object.DestroyImmediate(root);
            if (reference != null) UnityEngine.Object.DestroyImmediate(reference);
            UnityEngine.Object.DestroyImmediate(asset);
            InputSystem.RemoveDevice(keyboard);
        }
        return passed;
    }
}
#endif
