using Godot;



namespace GodotUtils;

public static class ExtensionsSceneTree
{
    /// <summary>
    /// Unfocus the current UI element that is being focused
    /// </summary>
    public static void UnfocusCurrentControl(this SceneTree tree)
    {
        // Get the currently focused control
        Control focusedControl = tree.Root.GuiGetFocusOwner();

        if (focusedControl != null)
        {
            // Set the focus mode to None to unfocus it
            focusedControl.FocusMode = Control.FocusModeEnum.None;
        }
    }
}

