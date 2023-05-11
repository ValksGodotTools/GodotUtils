namespace GodotUtils;

public static class ExtensionsAnimatedSprite
{
    /// <summary>
    /// Instantly switch to the next animation without waiting for the current animation to finish
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string anim)
    {
        sprite.Animation = anim;
        sprite.Play(anim);
    }

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
    /// Play a animation starting at a random frame
    /// </summary>
    public static void PlayRandom(this AnimatedSprite2D sprite, string anim)
    {
        sprite.InstantPlay(anim);
        sprite.Frame = GD.RandRange(0, sprite.SpriteFrames.GetFrameCount(anim));
    }

    public static int GetWidth(this AnimatedSprite2D sprite, string anim) =>
        (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetWidth() * sprite.Scale.X);

    public static int GetHeight(this AnimatedSprite2D sprite, string anim) =>
        (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetHeight() * sprite.Scale.Y);

    public static Vector2 GetSize(this AnimatedSprite2D sprite, string anim) =>
        sprite.SpriteFrames.GetFrameTexture(anim, 0).GetSize();
}
