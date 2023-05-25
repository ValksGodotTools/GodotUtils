namespace MyGame;

public static class ExtensionsSprite2D
{
    public static Vector2 GetSize(this Sprite2D sprite) => sprite.Texture.GetSize();

    /// <summary>
    /// <para>
    /// Gets the actual pixel size of the sprite. All rows and columns 
    /// consisting of transparent pixels are subtracted from the size.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static Vector2 GetPixelSize(this Sprite2D sprite) =>
        new Vector2(GetPixelWidth(sprite), GetPixelHeight(sprite));

    /// <summary>
    /// <para>
    /// Gets the actual pixel width of the sprite. All columns consisting of 
    /// transparent pixels are subtracted from the width.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static int GetPixelWidth(this Sprite2D sprite)
    {
        var img = sprite.Texture.GetImage();
        var size = img.GetSize();

        var transColumnsLeft = GodotUtilities.GetTransparentColumnsLeft(img, size);
        var transColumnsRight = GodotUtilities.GetTransparentColumnsRight(img, size);

        var pixelWidth = size.X - transColumnsLeft - transColumnsRight;

        return (int)(pixelWidth * sprite.Scale.X);
    }

    /// <summary>
    /// <para>
    /// Gets the actual pixel height of the sprite. All rows consisting of 
    /// transparent pixels are subtracted from the height.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static int GetPixelHeight(this Sprite2D sprite)
    {
        var img = sprite.Texture.GetImage();
        var size = img.GetSize();

        var transRowsTop = GodotUtilities.GetTransparentRowsTop(img, size);
        var transRowsBottom = GodotUtilities.GetTransparentRowsBottom(img, size);

        var pixelHeight = size.Y - transRowsTop - transRowsBottom;

        return (int)(pixelHeight * sprite.Scale.Y);
    }

    public static int GetPixelBottomY(this Sprite2D sprite)
    {
        var img = sprite.Texture.GetImage();
        var size = img.GetSize();

        // Might not work with all sprites but works with ninja. The -2 offset that is
        var diff = -2;

        for (int y = (int)size.Y - 1; y >= 0; y--)
        {
            if (img.GetPixel((int)size.X / 2, y).A != 0)
                break;

            diff++;
        }

        return diff;
    }
}
