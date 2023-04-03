namespace GodotUtils;

public static class ExtensionsAnimatedSprite
{
    /// <summary>
    /// Instantly switch to the next animation without waiting for the current animation to finish
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string animation)
    {
        sprite.Animation = animation;
        sprite.Play(animation);
    }

    public static void InstantPlay(this AnimatedSprite2D sprite, string animation, int frame)
    {
        sprite.Animation = animation;

        if (sprite.SpriteFrames.GetFrameCount(animation) - 1 >= frame)
            sprite.Frame = frame;

        sprite.Play(animation);
    }

    /// <summary>
    /// Play a animation starting at a random frame
    /// </summary>
    public static void PlayRandom(this AnimatedSprite2D sprite, string animation)
    {
        sprite.InstantPlay(animation);
        sprite.Frame = GD.RandRange(0, sprite.SpriteFrames.GetFrameCount(animation));
    }

    public static int GetWidth(this AnimatedSprite2D sprite, string animation) =>
        sprite.SpriteFrames.GetFrameTexture(animation, 0).GetWidth();

    public static int GetHeight(this AnimatedSprite2D sprite, string animation) =>
        sprite.SpriteFrames.GetFrameTexture(animation, 0).GetHeight();

    public static Vector2 GetSize(this AnimatedSprite2D sprite, string animation) =>
        sprite.SpriteFrames.GetFrameTexture(animation, 0).GetSize();
}
