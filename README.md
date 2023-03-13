## What is this?
A utils library for Godot 4 C# RC5+

Created for personal needs.

## Todo
#### Debug Console UI
- Make exceptions log the full stack, not what its doing right now

#### Multiplayer
#### Mod Loader UI
#### Camera Shake
#### Option UI Helpers
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
