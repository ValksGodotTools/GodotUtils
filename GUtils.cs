namespace GodotUtils;

public static class GUtils
{
    public async static Task WaitOneFrame(Node parent) => 
        await parent.ToSignal(parent.GetTree(), "process_frame");

    public static Vector2 GetMovementInput(string prefix = "")
    {
        if (!string.IsNullOrWhiteSpace(prefix))
            prefix += "_";

        // GetActionStrength(...) supports controller sensitivity
        var inputHorz = Input.GetActionStrength($"{prefix}move_right") - Input.GetActionStrength($"{prefix}move_left");
        var inputVert = Input.GetActionStrength($"{prefix}move_down") - Input.GetActionStrength($"{prefix}move_up");

        // Normalize vector to prevent fast diagonal strafing
        return new Vector2(inputHorz, inputVert).Normalized();
    }

    public static Vector2 RandDir(float dist = 1)
    {
        var theta = RandAngle();
        return new Vector2(Mathf.Cos(theta) * dist, Mathf.Sin(theta) * dist);
    }

    public static float RandAngle() => (float)GD.RandRange(0, Mathf.Pi * 2);

    public static Area2D CreateAreaRect(Node parent, Vector2 size, string debugColor = "ff001300") =>
        CreateArea(parent, new RectangleShape2D { Size = size }, debugColor);

    public static Area2D CreateAreaCircle(Node parent, float radius, string debugColor = "ff001300") =>
        CreateArea(parent, new CircleShape2D { Radius = radius }, debugColor);

    public static Area2D CreateArea(Node parent, Shape2D shape, string debugColor = "ff001300")
    {
        var area = new Area2D();
        var areaCollision = new CollisionShape2D
        {
            DebugColor = new Color(debugColor),
            Shape = shape
        };

        area.AddChild(areaCollision);
        parent.AddChild(area);

        return area;
    }

    /// <summary>
    /// <para>Returns a Godot.Color based off rgb ranging from 0 to 255</para>
    /// <para>The alpha still ranges from 0.0 to 1.0</para>
    /// </summary>
    public static Godot.Color Color(int r, int g, int b, float a = 1)
        => new Color(r / 255f, g / 255f, b / 255f, a);

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

    public static int GetTransparentColumnsLeft(Image img, Vector2 size)
    {
        var columns = 0;

        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
                if (img.GetPixel(x, y).A != 0)
                    return columns;

            columns++;
        }

        return columns;
    }

    public static int GetTransparentColumnsRight(Image img, Vector2 size)
    {
        var columns = 0;

        for (int x = (int)size.X - 1; x >= 0; x--)
        {
            for (int y = 0; y < size.Y; y++)
                if (img.GetPixel(x, y).A != 0)
                    return columns;

            columns++;
        }

        return columns;
    }

    public static int GetTransparentRowsTop(Image img, Vector2 size)
    {
        var rows = 0;

        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
                if (img.GetPixel(x, y).A != 0)
                    return rows;

            rows++;
        }

        return rows;
    }

    public static int GetTransparentRowsBottom(Image img, Vector2 size)
    {
        var rows = 0;

        for (int y = (int)size.Y - 1; y >= 0; y--)
        {
            for (int x = 0; x < size.X; x++)
                if (img.GetPixel(x, y).A != 0)
                    return rows;

            rows++;
        }

        return rows;
    }

    private static void EditInputText(LineEdit input, string text)
    {
        input.Text = text;
        input.CaretColumn = input.Text.Length;
    }
}
