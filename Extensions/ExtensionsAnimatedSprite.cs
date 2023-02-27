namespace GodotUtils;

public static class ExtensionsAnimatedSprite
{
	public static int GetWidth(this AnimatedSprite2D animatedSprite, string animation) =>
		animatedSprite.SpriteFrames.GetFrameTexture(animation, 0).GetWidth();

	public static int GetHeight(this AnimatedSprite2D animatedSprite, string animation) =>
		animatedSprite.SpriteFrames.GetFrameTexture(animation, 0).GetHeight();
}
