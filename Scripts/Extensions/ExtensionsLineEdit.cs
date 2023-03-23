namespace GodotUtils;

public static class ExtensionsLineEdit
{
    private static Dictionary<ulong, string> PrevTexts { get; } = new();
    private static Dictionary<ulong, int> PrevNums { get; } = new();

    public static string Filter(this LineEdit lineEdit, Func<string, bool> filter)
    {
        var text = lineEdit.Text;
        var id = lineEdit.GetInstanceId();

        if (string.IsNullOrWhiteSpace(text))
            return PrevTexts.ContainsKey(id) ? PrevTexts[id] : null;

        if (!filter(text))
        {
            if (!PrevTexts.ContainsKey(id))
            {
                lineEdit.ChangeLineEditText("");
                return null;
            }

            lineEdit.ChangeLineEditText(PrevTexts[id]);
            return PrevTexts[id];
        }

        PrevTexts[id] = text;
        return text;
    }

    public static int FilterRange(this LineEdit lineEdit, int maxRange)
    {
        var text = lineEdit.Text;
        var id = lineEdit.GetInstanceId();

        // Ignore blank spaces
        if (string.IsNullOrWhiteSpace(text))
        {
            lineEdit.ChangeLineEditText("");    
            return 0;
        }

        // Text is not a number
        if (!int.TryParse(text.Trim(), out int num))
        {
            // No keys are in the dictionary for the first case, so handle this by returning 0
            if (!PrevNums.ContainsKey(id))
            {
                lineEdit.ChangeLineEditText("");
                return 0;
            }

            // Scenario #1: Text is 'a'  -> returns ""
            // Scenario #2: Text is '1a' -> returns ""
            if (text.Length == 1 || text.Length == 2)
            {
                if (!int.TryParse(text, out int number))
                {
                    lineEdit.ChangeLineEditText("");
                    return 0;
                }
            }

            // Text is '123', user types a letter -> returns "123"
            lineEdit.ChangeLineEditText($"{PrevNums[id]}");
            return PrevNums[id];
        }

        // Not sure why this is here but I'm sure it's here for a good reason
        if (text.Length > maxRange.ToString().Length && num <= maxRange)
        {
            var spliced = text.Remove(text.Length - 1);
            PrevNums[id] = int.Parse(spliced);

            lineEdit.Text = spliced;
            lineEdit.CaretColumn = spliced.Length;
            return PrevNums[id];
        }

        // Text is at max range, return max range text if greater than max range
        if (num > maxRange)
        {
            num = maxRange;
            lineEdit.ChangeLineEditText($"{maxRange}");
        }

        // Keep track of the previous number
        PrevNums[id] = num;
        return num;
    }

    private static void ChangeLineEditText(this LineEdit lineEdit, string text)
    {
        lineEdit.Text = text;
        lineEdit.CaretColumn = text.Length;
    }
}
