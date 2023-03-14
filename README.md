## What is this?
A utils library for Godot 4 C# RC5+

Created for personal needs.

Currently using https://github.com/Valks-Games/Sandbox2 (among other repos but mainly this one) to test out GodotUtils in various ways

## Features
- Pressing `F12` opens the console
- Various helper functions

## Todo
#### Multiplayer
#### Mod Loader UI
#### Camera Shake
#### Dialogue System
#### Inventory UI

## See the following for inspiration
- https://github.com/GodotModules/GodotModulesCSharp
- https://github.com/GodotModules/Sandbox
- https://github.com/Valks-Games/Project2D
- https://github.com/Valks-Games/DialogueSystem

## Install
Add this as a submodule to your GitHub repo
```
git submodule add https://github.com/Valks-Games/GodotUtils GodotUtils
```

Make sure this is in your `.csproj`
```xml
<ItemGroup>
	<PackageReference Include="Newtonsoft.Json" Version="13.0.1" />
</ItemGroup>
```

*https://www.nuget.org/packages/ValksGodotUtils/ (no longer being maintained or used)*

## Notes
- Remember to do `Logger.Update()` in `_PhysicsProcess(double delta)` and always use `Logger.Log()` over `GD.Print()`
- `UIConsole` will only be used if you add it to the scene with `AddChild(new UIConsole())`
- To create new commands for use in the console, create new classes that extend from `Command`
- Make sure the UIConsole is placed at the very bottom of all the nodes under a CanvasLayer node otherwise your console will appear below some other UI elements
