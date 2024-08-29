namespace GodotUtils;

using Godot;
using System.IO;

public static class GDirectories
{
    /// <summary>
    /// Recursively traverses all directories and performs a action on each file path. 
    /// 
    /// <code>
    /// GDirectories.Traverse("res://", fullFilePath => GD.Print(fullFilePath))
    /// </code>
    /// </summary>
    public static void Traverse(string directory, Action<string> actionFullFilePath)
    {
        using DirAccess dir = DirAccess.Open(ProjectSettings.GlobalizePath(directory));

        dir.ListDirBegin();

        string nextFileName;

        while ((nextFileName = dir.GetNext()) != "")
        {
            string fullFilePath = Path.Combine(directory, nextFileName);

            if (dir.CurrentIsDir())
            {
                if (!nextFileName.StartsWith("."))
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


    /// <summary>
    /// Recursively searches for the file name and if found returns the full file path to
    /// that file.
    /// 
    /// <para>null is returned if the file is not found</para>
    /// 
    /// <code>
    /// string fullPathToPlayer = GDirectories.FindFile("res://", "Player.tscn")
    /// </code>
    /// </summary>
    public static string FindFile(string directory, string fileName)
    {
        using DirAccess dir = DirAccess.Open(ProjectSettings.GlobalizePath(directory));

        dir.ListDirBegin();

        string nextFileName;

        while ((nextFileName = dir.GetNext()) != "")
        {
            string fullFilePath = Path.Combine(directory, nextFileName);

            if (dir.CurrentIsDir())
            {
                if (!nextFileName.StartsWith("."))
                {
                    string result = FindFile(fullFilePath, fileName);

                    if (result != null)
                        return result;
                }
            }
            else
            {
                if (fileName == nextFileName)
                {
                    return fullFilePath;
                }
            }
        }

        dir.ListDirEnd();

        return null;
    }
}
