## What is this?
An ever expanding utils library for Godot 4 C#

## Features
- [Autoloads](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Autoload)
- [Prefabs](https://github.com/ValksGodotTools/GodotUtils/tree/main/Prefabs)
- [Extensions](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Extensions)
- [Godot Helper Classes](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Godot%20Helpers)
- [Netcode](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/Netcode) ([multiplater template](https://github.com/ValksGodotTools/Multiplayer))
- [UI](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/UI)
- [2D Platformer Scripts](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/World2D/Platformer)
- [2D Top Down Scripts](https://github.com/ValksGodotTools/GodotUtils/tree/main/Scripts/World2D/TopDown)
- 3D Scripts (TBA)
- Inventory Script(s) / Prefab(s) (TBA)
- [EventManager](https://github.com/ValksGodotTools/GodotUtils/blob/main/Scripts/EventManager.cs)
- [Thread Safe Logger](https://github.com/ValksGodotTools/GodotUtils/blob/main/Scripts/Logger.cs)
- [State Pattern](https://github.com/ValksGodotTools/GodotUtils/blob/main/Scripts/State.cs)

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
