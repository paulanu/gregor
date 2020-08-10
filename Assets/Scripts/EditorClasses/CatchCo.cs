using System;
// https://c.atch.co/invoke-a-method-right-from-a-unity-inspector/
// Place this file in any folder that is or is a descendant of a folder named "Scripts"
namespace CatchCo
{
    // Restrict to methods only
    [AttributeUsage(AttributeTargets.Method)]
    public class ExposeMethodInEditorAttribute : Attribute
    {
    }
}
