namespace GodotUtils;

public static class Utils
{
    public static void ValidateNumber(this string value, LineEdit input, int min, int max, ref int prevNum)
    {
        // do NOT use text.Clear() as it will trigger _on_NumAttempts_text_changed and cause infinite loop -> stack overflow
        if (string.IsNullOrEmpty(value))
        {
            prevNum = 0;
            EditInputText(input, "");
            return;
        }

        if (!int.TryParse(value.Trim(), out int num))
        {
            EditInputText(input, $"{prevNum}");
            return;
        }

        if (value.Length > max.ToString().Length && num <= max)
        {
            var spliced = value.Remove(value.Length - 1);
            prevNum = int.Parse(spliced);
            EditInputText(input, spliced);
            return;
        }

        if (num < min)
        {
            num = min;
            EditInputText(input, $"{min}");
        }

        if (num > max)
        {
            num = max;
            EditInputText(input, $"{max}");
        }

        prevNum = num;
    }

    private static void EditInputText(LineEdit input, string text)
    {
        input.Text = text;
        input.CaretColumn = input.Text.Length;
    }
}
