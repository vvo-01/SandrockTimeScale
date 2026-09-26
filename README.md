# Sandrock TimeScale

A BepInEx plugin for **My Time At Sandrock** that lets you control the game speed (`Time.timeScale`) with hotkeys, including a true "bullet-time" slow-motion effect.

Unlike simple timescale mods, this plugin uses a **Harmony patch** on `set_timeScale` to prevent the game from resetting the value every frame — so the chosen speed is held until you change it.

---

## Features

- **Slow motion** — press `LeftAlt` to set time to `0.25x`
- **Speed up** — press `NumPad0` to set time to `2.0x`
- **Reset** — press `Z` to return to normal `1.0x`
- All hotkeys and values are configurable
- `Enabled` and `Debug` toggles in the config
- Works reliably even though the game actively manages `Time.timeScale`

---

## Installation

1. Make sure **BepInEx 5.4.x** is installed for My Time At Sandrock.
2. Download `SandrockTimeScale.dll` from the [latest release](https://github.com/YOUR_USERNAME/SandrockTimeScale/releases).
3. Place the `.dll` into:
   ```
   <GameFolder>\BepInEx\plugins\
   ```
4. Launch the game once to generate the config file.

> **Important:** In `BepInEx\config\BepInEx.cfg`, set
> ```
> [Chainloader]
> HideManagerGameObject = true
> ```
> Without this, `Update()` in the plugin will not be called in this game.

---

## Configuration

After the first launch, a config file is created at:
```
BepInEx\config\sandrock.timescale.mod.cfg
```

Available options:

| Section   | Key          | Default   | Description                          |
|-----------|--------------|-----------|--------------------------------------|
| General   | `Enabled`    | `true`    | Enable or disable the mod            |
| General   | `Debug`      | `false`   | Enable debug logging                 |
| Controls  | `SlowKey`    | `LeftAlt` | Key for slow motion                  |
| Controls  | `ResetKey`   | `Z`       | Key to reset to 1.0                  |
| Controls  | `SpeedKey`   | `Keypad0` | Key for speed up                     |
| Scaling   | `SlowValue`  | `0.25`    | `timeScale` when SlowKey is pressed  |
| Scaling   | `SpeedValue` | `2.0`     | `timeScale` when SpeedKey is pressed |

Keys use Unity's `KeyCode` names (e.g. `LeftAlt`, `Keypad0`, `F1`, `Z`, `LeftControl`).

---

## How it works

The game constantly overwrites `Time.timeScale`, so simply setting it once has no lasting effect. This plugin:

1. **Patches `Time.set_timeScale` via Harmony** — whenever the game tries to change the timescale while a custom value is "locked", the call is intercepted and replaced with the plugin's target value.
2. **Re-applies the value in `Update()`** as a fallback, in case the patch is bypassed.

This two-layer approach is the same technique used by UnityExplorer's timescale widget.

---

## Building from source

Requirements:
- Visual Studio with **.NET Framework 4.7.2** targeting pack
- References to `BepInEx.dll`, `0Harmony.dll` (from the game's `BepInEx\core`) and the Unity assemblies from `Sandrock_Data\Managed`

Build in **Release**, then copy the resulting `SandrockTimeScale.dll` into `BepInEx\plugins`.

---

## License

MIT — see [LICENSE](LICENSE).
