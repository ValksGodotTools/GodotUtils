namespace GodotUtils;

using Godot;
using System.IO;

public static class GDirectories
{
    /// <summary>
    /// Recursively traverses all directories and performs a action on each file path. This ignores
    /// any directories that start with a period like .godot
    /// 
    /// <code>
    /// GDirectories.Traverse("res://", fullFilePath => GD.Print(fullFilePath))
    /// </code>
    /// </summary>
    public static void Traverse(string relativeFolder, Action<string> actionFullFilePath)
    {
        using DirAccess dir = DirAccess.Open(relativeFolder);

        if (dir == null)
        {
            throw new Exception("Failed to open directory: " + relativeFolder);
        }

        dir.ListDirBegin();

        string fileName;

        while ((fileName = dir.GetNext()) != "")
        {
            string fullFilePath = Path.Combine(ProjectSettings.GlobalizePath(relativeFolder), fileName);

            if (dir.CurrentIsDir())
            {
                if (!fileName.StartsWith("."))
                {
                    Traverse(fullFilePath, actionFullFilePath);
                }
            }
            else
            {
                actionFullFilePath(fullFilePath);
            }
        }

        dir.ListDirEnd();
    }

    public static string FindLocalFile(string relativeFolder, string theFile)
    {
        using DirAccess dir = DirAccess.Open(relativeFolder);

        if (dir == null)
        {
            throw new Exception("Failed to open directory: " + relativeFolder);
        }

        dir.ListDirBegin();

        string fileName;

        while ((fileName = dir.GetNext()) != "")
        {
            string fullFilePath = Path.Combine(ProjectSettings.GlobalizePath(relativeFolder), fileName);

            if (dir.CurrentIsDir())
            {
                if (!fileName.StartsWith("."))
                {
                    string result = FindLocalFile(fullFilePath, theFile);

                    if (result != null)
                        return result;
                }
            }
            else
            {
                if (theFile == fileName)
                {
                    string localPath = ProjectSettings.LocalizePath(fullFilePath);
                    return localPath;
                }
            }
        }

        dir.ListDirEnd();

        return null;
    }
}
