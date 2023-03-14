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
