## What is this?
A utils library for Godot 4 C# RC5+

Created for personal needs.

## Todo
- Debug Console UI
- Multiplayer

## Install
https://www.nuget.org/packages/ValksGodotUtils/

The nuget package will not always be up-to-date with the source. To ensure you're getting the latest release build the dll yourself, then add it to the root of your project and add the following to your `.csproj` file. Or if you don't want to build the dll, you can just copy the entire project directory to your project.

```csproj
<Reference Include="GodotUtils">
    <HintPath>GodotUtils.dll</HintPath>
</Reference>
```
