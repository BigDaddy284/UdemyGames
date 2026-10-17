<div align="center">

# Udemy Games

**My Unity scripts, editor tools and Asset Forge generators from learning game development through Udemy courses.**

C# for Unity, Lua for Kenney's Asset Forge, and a few notes to self along the way.

*Learn it. Build it. Break it. Fix it.*

[Thanks](#thanks-and-accreditation) · [What's inside](#whats-inside) · [Game scripts](#-game-scripts) · [Editor tools](#-unity-editor-tools) · [Asset Forge](#-asset-forge-lua-generators) · [Using the scripts](#using-the-scripts) · [License](#license)

</div>

---

## Thanks and accreditation

- **[Udemy](https://www.udemy.com)** and the **[GameDev.tv](https://www.gamedev.tv)** Unity courses, where *Project Boost* and *Argon Assault* and most of what's in here was learned.
- **[Unity](https://unity.com)**, the game engine every C# script here is written for.
- **[Asset Forge](https://assetforge.io)** by **[Kenney](https://kenney.nl)**, the 3D model builder the Lua generators run in. Some of the generators started from Kenney's own example scripts, and the house generator credits *MellowYellow*, building on Kenney's building generator.
- **[Anthropic's Claude](https://claude.ai/code)**, the AI coding assistant that wrote this page from my request.

## A hobby project, offered as-is

This is a hobby project, made for fun in spare time while learning. It is offered **as-is, with no warranty, no support and no promised updates.** Some of these scripts are experiments, some are old versions kept for reference, and some are marked buggy on purpose. I'm not able to answer questions, take requests or fix problems. You're welcome to read it, learn from it and borrow from it at your own risk.

## What's inside

| Folder | What it holds |
| --- | --- |
| 🎮 [`Games/`](Games) | Gameplay scripts from the course projects: *Project Boost*, *Argon Assault*, reusable player controllers and moving obstacles. |
| 🛠️ [`Unity/`](Unity) | Unity **editor tools** I wrote to speed up level building, plus older and test versions of them. |
| 🏗️ [`AF Lua Scripts/`](AF%20Lua%20Scripts) | Lua scripts that generate random buildings, cars, trains, mini golf courses and ground tiles in Asset Forge. |
| 📄 [`General/`](General) | A first "hello world" Unity script. |
| 🔗 [`git/`](git) | A starter `.gitignore` for Unity projects and step-by-step notes for linking a PC folder to a GitHub repo. |

The `.cs.meta` files next to some scripts are Unity's own bookkeeping files. They're only needed when the scripts sit inside a Unity project.

---

## 🎮 Game scripts

### 🚀 Project Boost

A rocket game where you thrust and steer a ship through each level to a landing pad without touching anything.

| Script | What it does |
| --- | --- |
| `Movement.cs` | **Space** fires the main thruster, **A** and **D** rotate the rocket. Plays engine sound and four booster particle effects. |
| `CollisionHandler.cs` | Decides what happens when the rocket hits something: nothing on a *Friendly* object, next level on *Finish*, crash and restart on anything else. |
| `BlimpMovement.cs` / `WallMovement.cs` | Make obstacles drift back and forth, side to side and up and down. |
| `QuitApplication.cs` | **Esc** closes the game. |

**How the thrust works.** Rather than moving the rocket directly, `Movement.cs` pushes it with the physics engine, so gravity and momentum still apply:

```csharp
void StartThrusting()
{
    rb.AddRelativeForce(Vector3.up * mainThrust);   // push "up" relative to the way the rocket is facing
    if (!audioSource.isPlaying)
    {
        audioSource.PlayOneShot(mainEngine);
        boosterLF.Play();
        ...
    }
}
```

Rotation is done by hand, so the physics engine is paused for a moment while the rocket turns, then handed back control:

```csharp
void ApplyRotation(float rotationThisFrame)
{
    rb.freezeRotation = true;   // stop physics spinning the rocket while we rotate it
    transform.Rotate(Vector3.forward * rotationThisFrame * Time.deltaTime);
    rb.freezeRotation = false;  // let physics take over again
}
```

**How a crash or a win is decided.** `CollisionHandler.cs` looks at the **tag** of whatever the rocket touched:

```csharp
switch (other.gameObject.tag)
{
    case "Friendly":
        break;                    // launch pad, nothing happens
    case "Finish":
        StartSuccessSequence();   // win sound, particles, load next level
        break;
    default:
        StartCrashScene();        // crash sound, particles, restart this level
        break;
}
```

Both sequences turn off the player's controls and use `Invoke("LoadNextLevel", loadDelay)` to wait a second before changing scene, so the sound and particles have time to play. After the last level, `LoadNextLevel` wraps back round to the first.

There are two debug keys for testing levels: **L** skips to the next level and **C** turns collisions on and off.

### ✈️ Argon Assault

An on-rails space shooter: the ship flies along a set path while you move it around the screen and shoot.

| Script | What it does |
| --- | --- |
| `PlayerMovement.cs` | Moves the ship within the screen, tilts it as it moves and fires the lasers. |
| `Enimies.cs` | Destroys an enemy when a laser particle hits it. |
| `CollisionHandler.cs` | Logs collisions and triggers to the console while testing. |

**Keeping the ship on screen.** Each frame the new position is worked out from the input, then clamped so the ship can't leave the play area:

```csharp
float newXPos = transform.localPosition.x + xOffset;
float xPosLimit = Mathf.Clamp(newXPos, -xRange, xRange);
```

**Making it feel like flying.** The ship's pitch, yaw and roll come partly from *where* it is on screen and partly from *which way you're pushing*, which makes it bank into turns:

```csharp
float pitch = transform.localPosition.y * positionPitchFactor + yMove * controlPitchFactor;
float yaw   = transform.localPosition.x * positionYawFactor;
float roll  = xMove * controlPitchFactor;

transform.localRotation = Quaternion.Euler(pitch, yaw, roll);
```

**Lasers.** The lasers are particle systems that are always there; holding **Fire1** just switches their emission on and off. Enemies use `OnParticleCollision` to know when they've been hit.

The `[Header]` and `[Tooltip]` attributes in `PlayerMovement.cs` group the settings and add hover help in Unity's Inspector, so they're easy to tweak without opening the code.

### 🕹️ Player controllers

| Script | What it does |
| --- | --- |
| `Mover.cs` | The simplest possible controller: arrow keys or WASD move an object across a flat 3D surface. |
| `PlayerTEMPLATE.cs` | A starting point for a top-down vehicle: **Up/Down** accelerates, **Left/Right** steers, with slots in the Inspector for sound effects and an example of destroying what you crash into. |

### 🌀 Moving objects and enemies

`OscillatingMovement.cs` is the general version of the blimp and wall scripts. Drop it on any object and set a speed and range for each axis, plus an optional spin, all from the Inspector. It uses a sine wave so the object eases in and out at each end instead of bouncing:

```csharp
float newX = startX + Mathf.Sin(Time.time * m_Speedx) * m_Rangex;
```

`Mathf.Sin` swings smoothly between −1 and 1 over time, so the object swings between `startX - range` and `startX + range`.

---

## 🛠️ Unity editor tools

These aren't gameplay scripts. They add new windows to the Unity Editor to make building levels quicker. The finished versions are in `Unity/Tools`.

### Object Move Tool (v1.6)

Open it from **Window → Object Move Tool**. It makes dragging objects around feel like snapping to a grid.

- **Rounding**: positions and scales snap to whole numbers, tenths or hundredths as you move things.
- **Axis locks**: lock X, Y or Z so an object can only slide along the axes you want.
- **Reset Position**: puts the selected objects back where they were when you selected them.
- **Ctrl + Shift + R** toggles rounding on and off without opening the window.
- Works with several objects selected at once, and closes itself when you press Play.

It runs on the editor's update loop and only snaps an object on frames where it has actually moved:

```csharp
if (position != lastFramePositions[transform])
{
    if (isRoundingEnabled)
        position = new Vector3(
            RoundToPrecision(position.x, GetRoundingPrecision()), ...);
    ...
}

float RoundToPrecision(float value, float precision)
{
    return Mathf.Round(value / precision) * precision;   // e.g. 3.27 at 0.1 → 3.3
}
```

The rounding settings are saved with `EditorPrefs`, so they're remembered between Unity sessions.

### Object Creation Tool (v1.5)

Open it from **Tools → Object Creation Tool**. Select an object in the scene and the tool shows its position. Nudge an offset with the **X/Y/Z +/−** buttons (up to 20 units each way), then create something at that spot:

- a **primitive** (cube, sphere, capsule and so on),
- a **prefab** from `Assets/Prefabs`,
- a **2D sprite** from any image in `Assets/Prefabs`,
- an **audio source** using a clip from `Assets/Audio`, or an **audio reverb zone**.

Its drop-down lists fill themselves by searching the project, and refresh whenever the project changes:

```csharp
var prefabGuids = AssetDatabase.FindAssets("t:GameObject", new[] { "Assets/Prefabs" });
```

Everything it creates is registered with `Undo`, so **Ctrl + Z** removes it like anything else.

### Other versions

| Folder | What's there |
| --- | --- |
| `Unity/Scripts/Old -Working Versions` | Earlier versions that work, plus two small helpers that create the usual project folders (`Audio`, `Materials`, `Prefabs`, `Scripts`, `VFX`) from the **Tools** menu. |
| `Unity/Scripts/Old-Buggy Versions` | Earlier versions with known problems, kept for reference. |
| `Unity/Scripts/Testing` | **Object Move Assist v1.8**, a work-in-progress version with per-object axis locks and scale locking, heading for v2.0. Marked unstable. |

Editor tools must go in a folder called **`Editor`** inside your Unity project's `Assets`, or Unity will refuse to build the game.

---

## 🏗️ Asset Forge Lua generators

[Asset Forge](https://assetforge.io) builds 3D models out of ready-made blocks, and it can run Lua scripts to place those blocks for you. Each script here clears the scene and builds something new and random every time it runs.

| Script | Builds |
| --- | --- |
| `buildingGenerator.lua` | A town building 3 to 6 blocks wide with 2 to 4 floors, random windows, balconies, corner stones, a cornice and rooftop details. |
| `myBuildingGenerator.lua` | My own take on the building generator, allowing smaller and lower buildings. |
| `complexBuildingGenerator.lua` | A house with doors as well as windows, 1 or 2 floors. |
| `carGenerator.lua` | A car with a random height, width, front end and windscreen. |
| `trainGenerator.lua` | A diesel train with a random cab. |
| `golfGenerator.lua` | A random mini golf hole: a start, 10 to 20 pieces of course that turn and bump, and a hole at the end. |
| `flatGroundPlaneGenerator.lua` | A gently uneven grass ground tile with flat edges, so tiles join up. |
| `XXXandomGroundPlaneGenerator.lua` | A hillier experimental version of the ground tile. |

**How the building generator works.** It loops over every floor and every block along each wall, and picks what to place:

```lua
for floor = 0, floors - 1 do
    for x = 0, sizeX - 1 do
        buildWall = "Walls/wall"
        if floor == 0 then buildWall = "Walls/wall_border" end     -- ground floor gets a border
        if floor > 0 and x % 2 == 0 then buildWall = randomWindow end  -- every other block upstairs is a window

        forge.build(buildWall, {x, floor, 0}, {0, 0, 0})           -- front wall
        forge.build(buildWall, {x, floor, sizeZ}, {0, 0, 0})       -- back wall
    end
end
```

`forge.build(block, position, rotation)` is Asset Forge's command to place one block. One window style and one corner style are picked at the start so the whole building matches.

**How the golf generator works.** It walks forward one square at a time, randomly choosing to carry on or turn. A straight run gets a straight piece (sometimes with a bump or flags), a change of direction gets a bend, and the last square gets the hole.

**How the ground generator works.** It gives each corner of the grid a random height, keeps the outer edges at zero, then averages each point with its neighbours to smooth out any spikes before placing the tiles.

---

## Using the scripts

**In Unity**

1. Copy the `.cs` file into your project's `Assets/Scripts` folder (editor tools go in `Assets/Editor`).
2. Drag the script onto a GameObject in the Hierarchy.
3. Fill in the settings in the Inspector: speeds, ranges, sounds and particle effects.
4. For *Project Boost*, tag your launch pad **Friendly** and your landing pad **Finish**, and add your levels to **File → Build Settings** in order.

The class name must match the file name, so rename the class if you rename the file. Two scripts both called `CollisionHandler` can't live in the same Unity project.

**In Asset Forge**

Open the scripting panel, load or paste a `.lua` file and run it. Run it again for a new random result. The scripts expect Asset Forge's standard block library; the ground tile scripts also need a `modular_terrain_collection` block set.

**Setting up git for a new game**

See [`git/Link PC Folder to git Repo.txt`](git/Link%20PC%20Folder%20to%20git%20Repo.txt) for the commands, and copy [`git/.gitignore`](git/.gitignore) into the new project first so Unity's large generated folders aren't uploaded.

## License

There's no license file in this repository yet, so by default the code stays the author's own. Course projects follow the course's own terms; Asset Forge generator scripts based on Kenney's examples keep Kenney's credit.

<div align="center">

© GizMX Racing Development

</div>
