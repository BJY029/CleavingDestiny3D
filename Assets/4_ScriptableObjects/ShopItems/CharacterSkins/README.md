# Character coin shop

CharacterShopPanel in LobbyScene is connected to CharacterShopController, the character item prefab and this catalog.

## UGS setup

- Keep the existing PurchaseSkin endpoint and SKIN_CATALOG. Merge character entries into the existing catalog; do not replace axe entries.
- RedWarrior currently uses skinId "red_warrior". Register that exact key with currency "coins" and the approved integer price. The SO price is not a fallback or an instruction to publish a price.
- character_basic is the local free default and does not require a server purchase. Its icon currently reuses the available character icon; its gameplay prefab is Player/Player1.
- Keep IDs globally unique across axe and character catalogs.
- Verify DataApi is imported in the deployed PurchaseSkin script using its installed Cloud Save SDK. The supplied script excerpt omitted that import.

## Behavior

Coins and ownedItems come from the server. UGSManager retains the raw ownership list; character ownership is filtered by this catalog. Existing axe purchase code is unchanged. Buying unlocks a character; Equip is a separate action. Only the equipped character ID is saved locally. Wallet updates without ownedItems retain the previous list.

A purchase exception triggers a wallet read. If that read also fails, purchases stay disabled until Retry succeeds. Reopening the shop refreshes the wallet. Character selection now chooses the gameplay prefab at match start. No separate Photon character property is needed: PhotonNetwork.Instantiate replicates the selected prefab path.

## Verification

Unity compilation and in-memory checks passed: character ownership filtering/deduplication, axe ownership preservation, equip/save round trip, legacy saves, revoked-item fallback, insufficient/exact coins, wrong currency, duplicate-request guard, already-owned guard and unchanged local coin balance during eligibility checks.

Still verify against the configured UGS environment: actual coin debit and ownedItems grant, insufficient funds, repeated/already-owned and simultaneous purchases, write-lock conflicts, response-loss recovery and existing axe sticks purchases. No real purchases or production Remote Config edits were performed during implementation.

## Gameplay character spawning

- GameScene PlayerManager references this catalog. The default character uses Player/Player1 for both seats; red_warrior uses Player/RedPlayer. AI prefabs are unchanged.
- CharacterSkinSO derives its Photon Resources path from CharacterPrefab in the editor. Every skin must reference a complete network player prefab inside Resources, including root PhotonView, PlayerController, PlayerAnimationController, PlayerAxeSkinController and a valid humanoid Animator/controller.
- At spawn, a paid selection waits at most 10 real-time seconds for a fresh protected-wallet read. Successful reads repair invalid/unowned saved selections. Network failures/timeouts use the default for this match without erasing the saved selection.
- A late response never respawns the player. Leaving/changing rooms during lookup cancels spawning. Changes to equipped skins take effect next match.
- Validated in editor: compilation, default/paid Resources mapping, invalid/mismatched prefab rejection, required animator/component references and camera/axe/effect/body roots on both player prefabs.
- Still exercise with two clients: each seat with each skin, identical and different skins, local body shadows vs remote body visibility, walking/running/strike/item animations, hit/turn progression, inventory ownership, AI, timeout/room departure and restart without opening the shop. No live matches were started by the implementation checks.
