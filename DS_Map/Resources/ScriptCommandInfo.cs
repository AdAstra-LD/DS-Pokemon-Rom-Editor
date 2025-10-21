using System.Collections.Generic;

namespace DSPRE.Resources
{
    /// <summary>
    /// Represents metadata for a script command loaded from JSON database
    /// </summary>
    public class ScriptCommandInfo
    {
        public ushort CommandId { get; set; }
        public string Name { get; set; }
        public string DecompName { get; set; }
        public byte[] ParameterSizes { get; set; }
        public List<ScriptParameter.ParameterType> ParameterTypes { get; set; }
        public List<string> ParameterNames { get; set; }
        public string Description { get; set; }

        public ScriptCommandInfo()
        {
            ParameterSizes = new byte[0];
            ParameterTypes = new List<ScriptParameter.ParameterType>();
            ParameterNames = new List<string>();
        }

        public int ParameterCount => ParameterSizes?.Length ?? 0;

        public bool HasConditionalParameters => ParameterSizes != null && ParameterSizes.Length > 0 && ParameterSizes[0] == 0xFF;
    }

    /// <summary>
    /// Represents metadata for a movement/action command
    /// </summary>
    public class MovementCommandInfo
    {
        public ushort CommandId { get; set; }
        public string Name { get; set; }
        public string DecompName { get; set; }
        public string Description { get; set; }
    }
}
