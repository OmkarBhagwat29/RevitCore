
using System.Runtime.CompilerServices;

namespace RevitCore.Extensions.DefinitionExt
{
    public static class DefinitionGroupExtension
    {
        public static List<ExternalDefinition> CreateExternalDefinitions(this DefinitionGroup definitionGroup,
            List<IDefinitionConfig> definitionConfigs)
        {
            if(definitionGroup == null) throw new ArgumentNullException(nameof(definitionGroup));
            if(definitionConfigs==null) throw new ArgumentNullException(nameof(definitionConfigs));

#if REVIT2022_OR_GREATER
            return definitionConfigs.Select(c => definitionGroup.CreateExternalDefinition(c.Name, c.TypeId)).ToList();
#else
            return definitionConfigs.Select(c => definitionGroup.CreateExternalDefinition(c.Name, c.ParameterType)).ToList();
#endif
        }

        public static bool ContainsDefinition(this DefinitionGroup definitionGroup, string definitionName) =>
            definitionGroup.Definitions.Any(def => def.Name == definitionName);

#if REVIT2022_OR_GREATER

        public static ExternalDefinition CreateExternalDefinition(this DefinitionGroup definitionGroup, string definitionName,
    ForgeTypeId forgeTypeId)
        {
            if (definitionGroup == null) throw new ArgumentNullException(nameof(definitionGroup));
            if (definitionName == null) throw new ArgumentNullException(nameof(definitionName));

            if (definitionGroup.ContainsDefinition(definitionName))
                throw new ArgumentNullException($"{definitionGroup.Name} group already contains definition {definitionName}");

            return definitionGroup.Definitions.Create(new ExternalDefinitionCreationOptions(definitionName, forgeTypeId)) as ExternalDefinition;

        }

#else

        public static ExternalDefinition CreateExternalDefinition(this DefinitionGroup definitionGroup, string definitionName,
            ParameterType paramType)
        {
            if (definitionGroup == null) throw new ArgumentNullException(nameof(definitionGroup));
            if (definitionName == null) throw new ArgumentNullException(nameof(definitionName));

            if (definitionGroup.ContainsDefinition(definitionName))
                throw new ArgumentNullException($"{definitionGroup.Name} group already contains definition {definitionName}");

            return definitionGroup.Definitions.Create(new ExternalDefinitionCreationOptions(definitionName,paramType)) as ExternalDefinition;

        }
#endif

        public static IEnumerable<(string groupName, string parameterName)> GetAllGroupParameters(this DefinitionGroup definitionGroup)
        {
            
            foreach (var definition in definitionGroup.Definitions)
            {
               yield return (groupName: definitionGroup.Name, parameterName: definition.Name);
            }
        }

    }

}
