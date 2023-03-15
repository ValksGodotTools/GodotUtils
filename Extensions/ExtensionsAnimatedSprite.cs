namespace GodotUtils;

public static class ExtensionsAnimatedSprite
{
    /// <summary>
    /// Instantly switch to the next animation without waiting for the current animation to finish
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D animatedSprite2D, string animation)
    {
        animatedSprite2D.Animation = animation;
        animatedSprite2D.Play(animation);
    }

    public static int GetWidth(this AnimatedSprite2D animatedSprite, string animation) =>
        animatedSprite.SpriteFrames.GetFrameTexture(animation, 0).GetWidth();

    public static int GetHeight(this AnimatedSprite2D animatedSprite, string animation) =>
        animatedSprite.SpriteFrames.GetFrameTexture(animation, 0).GetHeight();
}
