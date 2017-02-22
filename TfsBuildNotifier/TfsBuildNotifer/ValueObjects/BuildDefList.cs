using System.Collections.Generic;

namespace TfsBuildNotifier.ValueObjects
{
    public class BuildDefList
    {
        public List<BuildDef> BuildDefs { get; set; }
    }

    public class BuildDef   
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}