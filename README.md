## What is this?
An ever expanding utils library for Godot 4 C#. This is the library I am using across all my games, now open source for everyone else to enjoy as well.

## Features
- [Autoloads](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Autoload)
- [Prefabs](https://github.com/ValksGodotTools/GodotUtils/tree/main/Prefabs)
- [Extensions](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Extensions)
- [Godot Helper Classes](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Godot%20Helpers)
- [Netcode](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Netcode) ([multiplater template](https://github.com/ValksGodotTools/Multiplayer))
- [UI](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/UI)
- [2D Platformer Scripts](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/World2D/Platformer)
- [2D Top Down Scripts](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/World2D/TopDown)
- [EventManager](https://github.com/ValksGodotTools/GodotUtils/blob/main/Scripts/EventManager.cs)
- [Thread Safe Logger](https://github.com/ValksGodotTools/GodotUtils/blob/main/Scripts/Logger.cs)
- [State Pattern](https://github.com/ValksGodotTools/GodotUtils/blob/main/Scripts/State.cs)

## Thinking of Adding
- 3D Scripts
- Inventory Script(s) / Prefab(s)
- Modular Mod Manager Logic and UI
- Procedurally generated tech tree

## Install
Add this as a submodule to your GitHub repo
```
git submodule add https://github.com/Valks-Games/GodotUtils GodotUtils
```

Add the following to your `.csproj`
```xml
<ItemGroup>
    <PackageReference Include="ENet-CSharp" Version="2.4.8" />
    <PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
</ItemGroup>
```

## Credits
- Shaders are from https://godotshaders.com/
- Thank you to everyone in the Godot Discord for helping me (especially [the31](https://github.com/31)), without you guys I would not have got as far as I've come today
