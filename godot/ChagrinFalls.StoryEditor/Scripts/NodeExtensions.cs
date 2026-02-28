using Godot;
namespace ChagrinFalls.StoryEditor.Scripts;
/// <summary>
/// Convenience extensions for forcing LTR layout on dynamically created nodes.
/// </summary>
public static class NodeExtensions
{
    /// <summary>Sets LayoutDirection to LTR and returns the node for chaining.</summary>
    public static T Ltr<T>(this T control) where T : Control
    {
        control.LayoutDirection = Control.LayoutDirectionEnum.Ltr;
        return control;
    }
}
