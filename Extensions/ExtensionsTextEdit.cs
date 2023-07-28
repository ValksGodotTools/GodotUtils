namespace GodotUtils;

using Godot;
using System;
using System.Collections.Generic;

public static class ExtensionsTextEdit
{
    private static readonly Dictionary<ulong, string> prevTexts = new();

    public static string Filter(this TextEdit textEdit, Func<string, bool> filter)
    {
        var text = textEdit.Text;
        var id = textEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return prevTexts.ContainsKey(id) ? prevTexts[id] : null;

        if (!filter(text))
        {
            if (!prevTexts.ContainsKey(id))
            {
                textEdit.ChangeTextEditText("");
                return null;
            }

            textEdit.ChangeTextEditText(prevTexts[id]);
            return prevTexts[id];
        }

        prevTexts[id] = text;
        return text;
    }
    private static void ChangeTextEditText(this TextEdit textEdit, string text)
    {
        textEdit.Text = text;
        //textEdit.CaretColumn = text.Length;
    }
}
