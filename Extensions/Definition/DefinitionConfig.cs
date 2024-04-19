
namespace RevitCore.Extensions.DefinitionExt
{
    public class DefinitionConfig : IDefinitionConfig
    {
        public string Name { get; set; }

#if REVIT2022_OR_GREATER

        public ForgeTypeId TypeId { get; set; }


        public DefinitionConfig(string _parameterName, ForgeTypeId _typeID)
        {
            TypeId = _typeID;
            Name = _parameterName;
        }



#else

        public ParameterType ParameterType { get; set; }


        public DefinitionConfig(string _parameterName, ParameterType _paramType)
        {
            ParameterType = _paramType;
            Name = _parameterName;
        }

#endif


    }
}
