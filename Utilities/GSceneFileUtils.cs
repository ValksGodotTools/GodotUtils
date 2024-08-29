namespace GodotUtils;

using System.IO;

public static class GSceneFileUtils
{
    public static void FixBrokenDependencies()
    {
        FixBrokenResourcePaths("res://");
    }

    private static void FixBrokenResourcePaths(string relativeFolder)
    {
        GDirectories.Traverse(relativeFolder, fullFilePath =>
        {
            if (fullFilePath.EndsWith(".tscn"))
            {
                string text = File.ReadAllText(fullFilePath);

                Regex regex = new("path=\"(?<path>.+)\" ");

                foreach (Match match in regex.Matches(text))
                {
                    string oldResourcePath = match.Groups["path"].Value;

                    if (!Godot.FileAccess.FileExists(oldResourcePath))
                    {
                        string newResourcePath = FindLocalPathToFile("res://", oldResourcePath.GetFile());

                        if (!string.IsNullOrWhiteSpace(newResourcePath))
                        {
                            text = text.Replace(oldResourcePath, newResourcePath);
                        }
                        else
                        {
                            GD.Print($"Failed to fix a resource path for the scene '{fullFilePath.GetFile()}'. The resource '{oldResourcePath.GetFile()}' could not be found in the project.");
                        }
                    }
                }

                File.WriteAllText(fullFilePath, text);
            }
        });
    }

    private static string FindLocalPathToFile(string relativeFolder, string fileName)
    {
        string file = null;

        GDirectories.Traverse(relativeFolder, fullFilePath =>
        {
            if (fullFilePath.GetFile() == fileName)
            {
                file = ProjectSettings.LocalizePath(fullFilePath);
            }
        });

        return file;
    }
}
