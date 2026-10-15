---
name: unity-character-onboarding
description: Turn a rigged character FBX, character name, and coin price into a working CleavingDestiny3D player prefab and character-shop entry. Use for adding playable character skins to this project's existing Player1, UGS, and Photon pipeline.
---

# Character onboarding

Use this skill for this project's existing character shop, not arbitrary Unity projects or unrigged-mesh rigging.

## Inputs and defaults

Require an existing FBX path, display name and nonnegative integer coin price. Derive a stable lowercase ASCII snake_case ID; if a name cannot produce a valid ID, propose an English ID and get the user's choice. Keep IDs unchanged after publication. Reuse the existing separate Equip action and next-match prefab selection. Do not change axe purchases, the default character, or existing character products.

## Run

1. Confirm the project contains Player1, CharacterSkinSO, CharacterSkinCatalogSO, CharacterSpawnResolver and the character shop. Check Unity is outside play/animation-preview mode and review current project instructions. Do not save or replace unrelated dirty scenes.
2. Deploy the bundled [Editor helper](scripts/CharacterOnboarding.cs) to Assets/Editor/CharacterOnboarding/CharacterOnboarding.cs only if absent. If installed code differs, inspect the differences before updating it; preserve unrelated changes. Refresh Unity and check compilation. Use the available Unity MCP tools and their current resource/tool documentation.
3. Read [workflow and validation](references/workflow.md). Call CharacterOnboarding.Build with safely encoded literal inputs through Unity execute_code. It returns a JSON report. Build changes local assets only; it never registers or buys a product and never changes UGS.
4. Open the icon and both animation preview PNGs. Examine texture/color/alpha, body proportions, pose deformation, feet and axe grip. Resolve warnings before registering. If auto mapping, textures, grip or deformation cannot be repaired confidently, retain the candidate and report the blocking issue; do not register it or claim completion.
5. After actual visual review passes, call CharacterOnboarding.Register(reportPath, true). Registration is already within a request to add the character; no extra user approval is required for this local step. Validate the returned report and catalog reference. Never pass true without opening the previews.
6. Return character ID, prefab/SO/icon paths, the exact remote-config-entry.json contents, checks completed and any unrun gameplay/network tests. Tell the user to MERGE the entry into the existing SKIN_CATALOG in the matching UGS environment and publish. Do not publish UGS or perform a real purchase under this skill.

## Invariants

- Work from the current Player1 template; use RedPlayer only as a known working comparison. Preserve source FBX/import settings and template prefabs.
- Normalize model size through the copied FBX importer, not post-Avatar rig/mesh Transform scaling. Clear the cached HumanDescription.skeleton when changing import scale so Avatar units are rebuilt; preserve human bone mappings.
- Use Humanoid bone mappings for sockets/effects, not assumed Mixamo names. Keep the Animator, PhotonView, observed components and gameplay scripts on the player root.
- Only validated candidates enter the catalog. An invalid Avatar, unresolved old-rig reference, missing texture needed for the intended appearance, missing preview or broken socket is a stop condition.
- Reuse same-input generated output; reject name/ID/price/source conflicts. Do not delete or overwrite unrelated assets or alter an existing product to make a rerun succeed.
- Missing bones/skinning require source-asset work. Extracting textures cannot reconstruct image files that were not supplied.
