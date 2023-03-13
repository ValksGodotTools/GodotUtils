## What is this?
A utils library for Godot 4 C# RC5+

Created for personal needs.

## Todo
- Debug Console UI
- Multiplayer
- Mod Loader UI
- Camera Shake
- Option UI Helpers
- Dialogue System
- Inventory UI

See the following for inspiration
- https://github.com/GodotModules/GodotModulesCSharp
- https://github.com/GodotModules/Sandbox
- https://github.com/Valks-Games/Project2D
- https://github.com/Valks-Games/DialogueSystem

## Install
https://www.nuget.org/packages/ValksGodotUtils/

The nuget package will not always be up-to-date with the source. To ensure you're getting the latest release build the dll yourself, then add it to the root of your project and add the following to your `.csproj` file. 

```csproj
<ItemGroup>
    <Reference Include="GodotUtils">
        <HintPath>GodotUtils.dll</HintPath>
    </Reference>
</ItemGroup>
```

Or if you don't want to build the dll, you can just copy the entire project directory to your project.

Note that if you have `<EnableDynamicLoading>true</EnableDynamicLoading>` enabled for whatever reason then the dll will not be found for some reason. There is a high chance that you don't even have this setting in your `.csproj` so you can ignore this.
