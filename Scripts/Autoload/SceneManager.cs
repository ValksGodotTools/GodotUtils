namespace SideScrollGame;

// About Scene Switching: https://docs.godotengine.org/en/latest/tutorials/scripting/singletons_autoload.html
public partial class SceneManager : Node
{
    private static SceneManager Instance     { get; set; }
    private static Node         CurrentScene { get; set; }
    private static SceneTree    Tree         { get; set; }

    public override void _Ready()
    {
        Instance = this;
        Tree = GetTree();
        var root = Tree.Root;
        CurrentScene = root.GetChild(root.GetChildCount() - 1);
    }

    public static void SwitchScene(string name)
    {
        Instance.FadeTo(TransType.Black, 2, () =>
        {
            // Wait for engine to be ready to switch scene
            Instance.CallDeferred(nameof(DeferredSwitchScene), name);
        });
    }

    private void DeferredSwitchScene(string name)
    {
        // Safe to remove scene now
        CurrentScene.Free();

        // Load a new scene.
        var nextScene = (PackedScene)GD.Load($"res://Scenes/{name}.tscn");

        // Instance the new scene.
        CurrentScene = nextScene.Instantiate();

        // Add it to the active scene, as child of root.
        Tree.Root.AddChild(CurrentScene);

        // Optionally, to make it compatible with the SceneTree.change_scene_to_file() API.
        Tree.CurrentScene = CurrentScene;

        FadeTo(TransType.Transparent, 1);
    }

    private void FadeTo(TransType transType, double duration, Action finished = null)
    {
        // Add canvas layer to scene
        var canvasLayer = new CanvasLayer
        {
            Layer = 10 // render on top of everything else
        };

        CurrentScene.AddChild(canvasLayer);

        // Setup color rect
        var colorRect = new ColorRect
        {
            Color = new Color(0, 0, 0, transType == TransType.Black ? 0 : 1),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        // Make the color rect cover the entire screen
        colorRect.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
        canvasLayer.AddChild(colorRect);

        // Animate color rect
        var tween = colorRect.CreateTween();
        tween.TweenProperty(colorRect, "color", new Color(0, 0, 0, transType == TransType.Black ? 1 : 0), duration);
        tween.TweenCallback(Callable.From(() =>
        {
            canvasLayer.QueueFree();
            finished?.Invoke();
        }));
    }

    private enum TransType
    {
        Black,
        Transparent
    }
}
