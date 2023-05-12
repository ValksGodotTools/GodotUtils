namespace GodotUtils;

public static class ExtensionsAnimatedSprite
{
    /// <summary>
    /// There may be a small delay when switching between animations. Use this function
    /// to remove that delay.
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string anim)
    {
        sprite.Animation = anim;
        sprite.Play(anim);
    }

    /// <summary>
    /// There may be a small delay when switching between animations. Use this function
    /// to remove that delay.
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string anim, int frame)
    {
        sprite.Animation = anim;

        var frameCount = sprite.SpriteFrames.GetFrameCount(anim);

        if (frameCount - 1 >= frame)
            sprite.Frame = frame;
        else
            Logger.LogWarning($"The frame '{frame}' specified for {sprite.Name} is" +
                $"lower than the frame count '{frameCount}'");

        sprite.Play(anim);
    }

    /// <summary>
    /// <para>
    /// Play a animation starting at a random frame
    /// </para>
    /// 
    /// <para>
    /// This is useful if making for example coin animations play at random frames
    /// </para>
    /// </summary>
    public static void PlayRandom(this AnimatedSprite2D sprite, string anim)
    {
        sprite.InstantPlay(anim);
        sprite.Frame = GD.RandRange(0, sprite.SpriteFrames.GetFrameCount(anim));
    }

    /// <summary>
    /// Gets the scaled width of the specified sprite frame
    /// </summary>
    public static int GetWidth(this AnimatedSprite2D sprite, string anim) =>
        (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetWidth() * sprite.Scale.X);

    /// <summary>
    /// Gets the scaled height of the specified sprite frame
    /// </summary>
    public static int GetHeight(this AnimatedSprite2D sprite, string anim) =>
        (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetHeight() * sprite.Scale.Y);

    /// <summary>
    /// Gets the scaled size of the specified sprite frame
    /// </summary>
    public static Vector2 GetSize(this AnimatedSprite2D sprite, string anim) =>
        new Vector2(GetWidth(sprite, anim), GetHeight(sprite, anim));

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
    public static Vector2 GetPixelSize(this AnimatedSprite2D sprite, string anim) =>
        new Vector2(GetPixelWidth(sprite, anim), GetPixelHeight(sprite, anim));

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
    public static int GetPixelWidth(this AnimatedSprite2D sprite, string anim)
    {
        var tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        var img = tex.GetImage();
        var size = img.GetSize();

        var spriteWidth = sprite.SpriteFrames.GetFrameTexture(anim, 0).GetWidth();

        var transColumnsLeft = GetTransparentColumnsLeft(img, size);
        var transColumnsRight = GetTransparentColumnsRight(img, size);

        var pixelWidth = spriteWidth - transColumnsLeft - transColumnsRight;

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
    public static int GetPixelHeight(this AnimatedSprite2D sprite, string anim)
    {
        var tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        var img = tex.GetImage();
        var size = img.GetSize();

        var spriteHeight = sprite.SpriteFrames.GetFrameTexture(anim, 0).GetHeight();

        var transRowsTop = GetTransparentRowsTop(img, size);
        var transRowsBottom = GetTransparentRowsBottom(img, size);

        var pixelHeight = spriteHeight - transRowsTop - transRowsBottom;

        return (int)(pixelHeight * sprite.Scale.Y);
    }

    private static int GetTransparentColumnsLeft(Image img, Vector2 size)
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

    private static int GetTransparentColumnsRight(Image img, Vector2 size)
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

    private static int GetTransparentRowsTop(Image img, Vector2 size)
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

    private static int GetTransparentRowsBottom(Image img, Vector2 size)
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
}
