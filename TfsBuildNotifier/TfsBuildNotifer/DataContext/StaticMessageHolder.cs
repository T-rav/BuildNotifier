using TfsBuildNotifier.ValueObjects;

namespace TfsBuildNotifier.DataContext
{
    public static class StaticMessageHolder
    {
         public static BuildMessage Message { get; set; }

         public static BuildState CurrentState { get; set; }

         public static bool IsFirstBuild { get; set; }
    }
}