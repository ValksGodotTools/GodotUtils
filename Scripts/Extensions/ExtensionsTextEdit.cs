namespace GodotUtils;

public static class ExtensionsTextEdit
{
    private static Dictionary<ulong, string> PrevTexts { get; } = new();

    public static string Filter(this TextEdit textEdit, Func<string, bool> filter)
    {
        var text = textEdit.Text;
        var id = textEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return PrevTexts.ContainsKey(id) ? PrevTexts[id] : null;

        if (!filter(text))
        {
            if (!PrevTexts.ContainsKey(id))
            {
                textEdit.ChangeTextEditText("");
                return null;
            }

            textEdit.ChangeTextEditText(PrevTexts[id]);
            return PrevTexts[id];
        }

        PrevTexts[id] = text;
        return text;
    }
    private static void ChangeTextEditText(this TextEdit textEdit, string text)
    {
        textEdit.Text = text;
        //textEdit.CaretColumn = text.Length;
    }
}
