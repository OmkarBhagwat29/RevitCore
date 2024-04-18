
namespace RevitCore.Extensions.DefinitionExt
{
    public static class DefinitionFileExtension
    {
        public static DefinitionGroup CreateGroup(this DefinitionFile definitionFile, string groupName)
        {
            if (definitionFile == null) throw new ArgumentNullException(nameof(definitionFile));
            if(groupName == null) throw new ArgumentNullException(nameof(groupName));

            if (definitionFile.GroupExists(groupName))
                throw new ArgumentNullException($"The {groupName} already exists");

            return definitionFile.Groups.Create(groupName);
           
        }

        public static DefinitionGroup CreateOrGetGroup(this DefinitionFile definitionFile, string groupName)
        {
            if (definitionFile == null) throw new ArgumentNullException(nameof(definitionFile));
            if (groupName == null) throw new ArgumentNullException(nameof(groupName));

            if (definitionFile.GroupExists(groupName))
                return definitionFile.Groups.get_Item(groupName);

            return definitionFile.Groups.Create(groupName);
        }

        public static bool GroupExists(this DefinitionFile definitionFile, string groupName)
        {
            return definitionFile.Groups.Any(g=>g.Name == groupName);
        }

        public static IEnumerable<IEnumerable<(string groupName, string parameterName)>> GetAllParametersFromFile(this DefinitionFile definitionFile)
        {
            foreach (var group in definitionFile.Groups)
            {
               yield return group.GetAllGroupParameters();
            }
        }


    }
}
