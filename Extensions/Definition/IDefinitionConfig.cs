
namespace RevitCore.Extensions.Definition
{
    public interface IDefinitionConfig
    {
        string Name { get; set; }

#if REVIT2022_OR_GREATER
ForgeTypeId TypeId {get;set;}
#else
        ParameterType ParameterType { get; set; }
#endif
    }
}
