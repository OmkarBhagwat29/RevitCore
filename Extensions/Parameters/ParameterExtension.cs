
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitCore.Extensions.FamilyHelpers;
using System.Runtime.CompilerServices;

namespace RevitCore.Extensions.Parameters
{
    public static class ParameterExtension
    {
        public static void SetBuiltInParameterValue(this Element element, BuiltInParameter parameter, object value, bool showPopUp)
        {
            Parameter param = element.get_Parameter(parameter);

            // Check if parameter is found and writable
            if (param != null && param.IsReadOnly == false)
            {
                // Set the value based on the parameter's storage type
                switch (param.StorageType)
                {
                    case StorageType.Double:
                        param.Set(Convert.ToDouble(value));
                        break;
                    case StorageType.Integer:
                        param.Set(Convert.ToInt32(value));
                        break;
                    case StorageType.String:
                        param.Set(value.ToString());
                        break;
                    default:
                        // Handle unsupported types
                        break;
                }
            }
            else
            {
                // Handle parameter not found or not writable
                if (showPopUp)
                {
                    if (param != null && param.IsReadOnly)
                    {
                        TaskDialog.Show("Warning", $"{param.Definition.Name} is a read only parameter,\ncan not set the value");

                    }
                    else if (param == null)
                    {
                        TaskDialog.Show("Warning", $"Built in Parameter: {parameter} not found for element {element.Id}");
                    }
                }
            }
        }

        public static IEnumerable<ElementId> GetParameterIds(this List<BuiltInParameter> parameters) => parameters.Select(p => new ElementId(p));

        public static void TryAddSharedParametersToFamily(this Document familyDocument,
            List<(Definition definition, bool isInstance)> definitionsToAdd, ForgeTypeId groupTypeId)
        {
            if (!familyDocument.IsFamilyDocument)
                throw new Exception("This is not a family document, it must be family document to add shared parameter to a family");


              var familyParameters = familyDocument
                .TryAddSharedParametersToFamilyManager(definitionsToAdd, groupTypeId);

                if (familyParameters.Count != definitionsToAdd.Count)
                    throw new ArgumentNullException($"Operation Failed!!! Some parameters were not able to add to Family: {familyDocument.PathName}");
        }

        private static List<FamilyParameter> TryAddSharedParametersToFamilyManager(this Document familyDocument,
            List<(Definition definition, bool isInstance)> definitionData, ForgeTypeId groupTypeId)
        {
            var paramData = new List<FamilyParameter>();
            var familyManager = familyDocument.FamilyManager;
            foreach (var data in definitionData)
            {
                if (data.definition is not ExternalDefinition externalDefinition)
                    throw new ArgumentNullException($"Parameter {data.definition.Name} can not add to family.");

                if (!familyManager.FamilySharedParameterExists(externalDefinition, out FamilyParameter parameter))
                {
                    parameter = familyManager.AddParameter(externalDefinition, groupTypeId, data.isInstance);
                }
                else
                {
                    //if exists remove and update with new settings
                    familyDocument.DeleteSharedParametersFromFamily([data.definition]);

                    parameter = familyManager.AddParameter(externalDefinition, groupTypeId, data.isInstance);
                }

                if (parameter == null)
                    throw new ArgumentNullException($"Operation Failed!\n{data.definition.Name} was not able to add to family {familyDocument.PathName}");

                paramData.Add(parameter);
            }

            return paramData;
        }

        public static void DeleteSharedParametersFromFamily(this Document familyDocument,
            List<Definition> definitions)
        {
          
            FamilyManager familyManager = familyDocument.FamilyManager;
            if (!familyDocument.IsFamilyDocument)
                throw new Exception("This is not a family document, it must be family document to delete shared parameters from a family");

            foreach (var def in definitions)
            {
                if (familyManager.FamilySharedParameterExists(def, out FamilyParameter familyParameter))
                {
                    familyManager.RemoveParameter(familyParameter);
                }
            }
        }

        public static bool FamilySharedParameterExists(this FamilyManager manager,
            Definition definition, out FamilyParameter familyParameter)
        {
            familyParameter = manager.get_Parameter(definition.Name);

            if (familyParameter == null)
                return false;

            if (definition is not ExternalDefinition externalDefinition)
                return false;

            if (familyParameter.IsShared && externalDefinition.GUID == familyParameter.GUID)
                return true;

            return false;
        }

        public static IEnumerable<ParameterGroupInfo> GetParameterGroups()
        {
            var groupForgeIds = ParameterUtils.GetAllBuiltInGroups();

            foreach (var forgeGroupId in groupForgeIds)
            {
                yield return new ParameterGroupInfo() {ForgeTypeId = forgeGroupId, Name = forgeGroupId.ToGroupLabel()};
            }
        }
    }
}
