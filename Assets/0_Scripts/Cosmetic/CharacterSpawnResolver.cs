using Photon.Pun;
using UnityEngine;

/// <summary>Validates the local prefab allowlist before passing a path to Photon.</summary>
public static class CharacterSpawnResolver
{
    public const string DefaultPrefabPath = "Player/Player1";

    public static bool TryGetPrefabPath(CharacterSkinSO skin, out string path)
    {
        path = null;
        if (skin == null || skin.CharacterPrefab == null || string.IsNullOrWhiteSpace(skin.PhotonPrefabPath)) return false;
        GameObject prefab = Resources.Load<GameObject>(skin.PhotonPrefabPath);
        if (prefab != skin.CharacterPrefab || !IsValidPlayerPrefab(prefab)) return false;
        path = skin.PhotonPrefabPath;
        return true;
    }

    public static bool IsValidPlayerPrefab(GameObject prefab)
    {
        if (prefab == null || prefab.GetComponent<PhotonView>() == null ||
            prefab.GetComponent<PlayerController>() == null ||
            prefab.GetComponent<PlayerAnimationController>() == null ||
            prefab.GetComponent<PlayerAxeSkinController>() == null) return false;
        Animator animator = prefab.GetComponent<Animator>();
        return animator != null && animator.avatar != null && animator.avatar.isValid &&
            animator.avatar.isHuman && animator.runtimeAnimatorController != null;
    }
}
