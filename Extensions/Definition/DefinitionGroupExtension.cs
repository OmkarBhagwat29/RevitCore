
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

            return definitionConfigs.Select(c => definitionGroup.CreateExternalDefinition(c.Name, c.TypeId)).ToList();

        }

        public static bool ContainsDefinition(this DefinitionGroup definitionGroup, string definitionName) =>
            definitionGroup.Definitions.Any(def => def.Name == definitionName);


        public static ExternalDefinition CreateExternalDefinition(this DefinitionGroup definitionGroup, string definitionName,
    ForgeTypeId forgeTypeId)
        {
            if (definitionGroup == null) throw new ArgumentNullException(nameof(definitionGroup));
            if (definitionName == null) throw new ArgumentNullException(nameof(definitionName));

            if (definitionGroup.ContainsDefinition(definitionName))
                throw new ArgumentNullException($"{definitionGroup.Name} group already contains definition {definitionName}");

            return definitionGroup.Definitions.Create(new ExternalDefinitionCreationOptions(definitionName, forgeTypeId)) as ExternalDefinition;

        }

        public static IEnumerable<(string groupName, string defName)> GetAllGroupParameters(this DefinitionGroup definitionGroup)
        {
            
            foreach (var definition in definitionGroup.Definitions)
            {
               yield return (groupName:definitionGroup.Name,defName:definition.Name);
            }
        }

        public static Definition GetDefinitionInGroup(this DefinitionGroup definitionGroup,
            string definitionName)
        {

            foreach (var definition in definitionGroup.Definitions)
            {
                if(definition.Name == definitionName)
                    return definition;
            }
            return null;
        }

        public static ExternalDefinition GetOrCreateDefinitionInGroup(this DefinitionGroup definitionGroup,
            string definitionName, ForgeTypeId dataTypeId)
        {
            if(dataTypeId==null)
                throw new ArgumentNullException("Forge Type id is null here");

            foreach (var definition in definitionGroup.Definitions)
            {
                if (definition.Name == definitionName)
                    return definition as ExternalDefinition;
            }
            return definitionGroup.CreateExternalDefinition(definitionName,dataTypeId);
        }

    }

}
