# Project workflow

## Project contracts

- Template: Assets/Resources/Player/Player1.prefab. Root Animator uses NewPlayerAnimation. Root PlayerController/PlayerAnimationController and Photon observed components must remain on that root for IK, state-exit callbacks and RPCs.
- Comparison: Assets/Resources/Player/RedPlayer.prefab. Do not instantiate a second complete player under Player1.
- Catalog: Assets/4_ScriptableObjects/ShopItems/CharacterSkins/CharacterSkinCatalogSO.asset.
- CharacterSkinSO stores skinId, displayName, price, icon, characterPrefab, photonPrefabPath. Runtime purchase price comes only from server SKIN_CATALOG.
- PlayerEffectPoints has head/body/foot/rightHand/leftHand/axe references. PlayerAnimationController has axeTransform, leftHandTarget, itemTransform, animator and camera references. PlayerAxeSkinController has axeVisualRoot. Body mesh roots belong in PlayerController.characterModelObjects; exclude weapons/cameras.
- Runtime spawning already validates paid ownership and selects a Resources prefab at match start. No new runtime feature is needed per character.

## Commands

Call in Unity Editor via execute_code (method body). Encode C# string literals safely; never paste unescaped user text into code.

    return CharacterOnboarding.Build("C:/path/knight.fbx", "Blue Knight", 250, "blue_knight");

Build creates Assets/CharacterOnboarding/<id>/ for source copy, textures, materials, icon, previews, Skin.asset and report.json. The playable prefab is Assets/Resources/Player/Characters/<id>.prefab. The original FBX and Player1 are untouched. Only sibling image files and the source FBX's adjacent .fbm folder are collected automatically; supply other external textures explicitly if needed. Source-project material remaps are reused when the FBX is already imported.

Existing output is reusable only for the same source/dependencies, name, price and ID. A report records ownership of intermediate output. On failure fix the candidate's issue without modifying the source/template; do not forge validated=true or delete conflicts. For a changed source/name/price use a new ID unless the user explicitly requests an existing-product update (outside this skill).

The helper copies Player1, swaps its rig/meshes/avatar, normalizes body height through ModelImporter.globalScale with regenerated Avatar skeleton data (never by scaling rig children after Avatar creation), transfers AxePivot to the new mapped right hand, repairs surviving bone references, assigns effect anchors and body renderer roots, and preserves Photon/root gameplay components. Automatic grip placement may require a local candidate adjustment and renewed validation.

Review Icon.png, Preview_walk.png and Preview_hit.png. This project's clip aliases include _WC for walk, _RC for run, _3H for strike, and 2Loop for idle. The preview includes a default axe for rough socket placement review. Sampled clips do NOT execute PlayerAnimationController.OnAnimatorIK, so verify the actual left-hand IK grip with an animator-driven gameplay test; never claim that static previews prove IK. Material maps/alpha require visual review; flat-color materials can be intentional.

After changing a candidate grip/material/prefab, regenerate previews and repeat review:

    return CharacterOnboarding.RefreshPreviews("Assets/CharacterOnboarding/blue_knight/report.json");

Registration rejects changed prefab dependencies until previews are refreshed. Refresh does not overwrite the candidate with the template.

After passing review:

    return CharacterOnboarding.Register("Assets/CharacterOnboarding/blue_knight/report.json", true);

Register revalidates the candidate and adds its SO once. It refuses conflicting IDs. It does not change UGS or equip a player. Open remote-config-entry.json and return its exact contents for merging with existing axe/character entries, not replacing the whole server catalog.

## Test isolation and acceptance

For tool validation, Build(..., verificationOnly: true) writes beneath Assets/CharacterOnboardingVerification/<id>, including a nested Resources prefab, and cannot be registered. Use a unique test ID. Keep generated test assets out of the catalog and remove only artifacts the test created after recording results.

Structural checks: valid Avatar, skinned-mesh bones, required player/camera/socket/effect references, no external/stale rig transform references, template animation parameters and Resource/prefab identity.

Visual checks: base textures, normal/alpha if present, proportions, feet, movement/strike deformation and axe grip. Do not call a build visually approved if any image is missing or has not been inspected.

Gameplay checks when a test session is available: both player seats, local body hidden with shadows, remote body visible, move/run, prepare/strike, impact callback and turn progression, item use, selected axe, single-player AI and two-client Photon replication. Separate editor/preview checks from real gameplay results. No live purchase or UGS mutation is implied by onboarding.
