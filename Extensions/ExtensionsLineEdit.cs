namespace GodotUtils;

using Godot;
using System;
using System.Collections.Generic;

public static class ExtensionsLineEdit
{
    private static readonly Dictionary<ulong, string> prevTexts = new();

    /// <summary>
    /// <para>
    /// Prevents certain characters from being inputted onto the LineEdit.
    /// The following example makes it so only alphanumeric characters are allowed
    /// to be inputted.
    /// </para>
    /// 
    /// <code>
    /// LineEdit.TextChanged += text =>
    /// {
    ///     string username = LineEdit.Filter(text => text.IsAlphaNumeric());
    ///     GD.Print(username);
    /// };
    /// </code>
    /// </summary>
    public static string Filter(this LineEdit lineEdit, Func<string, bool> filter)
    {
        ulong id = lineEdit.GetInstanceId();

        if (!filter(lineEdit.Text))
        {
            lineEdit.Text = prevTexts[id];
            lineEdit.CaretColumn = prevTexts[id].Length;
            return prevTexts.ContainsKey(id) ? prevTexts[id] : "";
        }

        prevTexts[id] = lineEdit.Text;

        return lineEdit.Text;
    }
}
