# CleavingDestiny3D - Project Overview

CleavingDestiny3D is a multiplayer 3D game built with **Unity 6 (6000.3.10f1)**. It features a turn-based or phase-based gameplay loop involving a lobby, a village, and a main game scene. The project leverages **Photon** for networking and follows a manager-centric architectural pattern.

## Core Technologies
- **Engine:** Unity 6 (6000.3.10f1)
- **Networking:** Photon Realtime / PUN (Room-based matchmaking)
- **Asynchronous Programming:** [UniTask](https://github.com/Cysharp/UniTask)
- **Tweening/Animation:** [PrimeTween](https://github.com/KyryloKuzyk/PrimeTween)
- **Asset Management:** Unity Addressables
- **Rendering:** Universal Render Pipeline (URP)
- **Input:** Input System Package
- **Camera:** Cinemachine

## Project Structure
- `Assets/0_Scripts/`: Main source code directory.
    - `Static/`: Global constants (`CommonDefine.cs`), localization, and helpers.
    - `LobbyScene/`: Matchmaking and connection logic (`MatchController.cs`, `LobbyConnectController.cs`).
    - `GameScene/`: Core gameplay logic, including `TurnManager.cs`, `PlayerManager.cs`, and `SpawnManager.cs`.
    - `VillageScene/`: Scripts related to the village phase/area.
    - `Generic/`: Reusable components (UI fades, localized strings).
    - `GameSetting/`: ScriptableObject-based configurations for items, players, and rooms.
- `Assets/3_Prefabs/`: Prefabs for players, items, and UI.
- `Assets/98_InputAction/`: Input action maps for the Input System.

## Architecture & Conventions
- **Manager Pattern:** Each major system (Turns, Players, UI, Time) has a dedicated manager script in the `GameScene` folder.
- **Networking:** Utilizes Photon Custom Properties (`RoomPropKeys`, `PlayerPropKeys`) for state synchronization across the network.
- **Asynchronous Flow:** Prefers `UniTask` over Coroutines for async operations (loading, network waits, animations).
- **Naming Conventions:**
    - Classes and Methods: `PascalCase`.
    - Constants and Enums: Often `SCREAMING_SNAKE_CASE` or `PascalCase`.
    - Scene Constants: Defined in `CommonDefine.cs`.

## Building and Running
- **Editor:** Open the project in Unity 6000.3.10f1.
- **Builds:** Standard Unity Build Pipeline (File -> Build Settings).
- **Testing:**
    - Use the **Unity Multiplayer Play Mode** package (if configured) for testing multiple clients in the editor.
    - Ensure `LobbyScene` is the entry point for matchmaking tests.

## Development TODOs
- Damage value ratio adjustment (See `Assets/0_Scripts/To Do.txt`).
- Localization is CSV-based; ensure new strings are added to the appropriate source.
