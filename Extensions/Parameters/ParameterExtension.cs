
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitCore.Extensions.FamilyHelpers;

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

        public static void TryAddSharedParametersToFamily(this Family family, Document doc,
            List<(Definition definition, bool isInstance)> definitionsToAdd, ForgeTypeId groupTypeId)
        {

            var familyDocument = doc.EditFamily(family) ?? throw new ArgumentNullException("family failed to edit");

            FamilyManager familyManager = familyDocument.FamilyManager;

            familyDocument.UseTransaction(() =>
            {
              var familyParameters =  familyManager
                .TryAddSharedParametersToFamilyManager(definitionsToAdd, groupTypeId);

                if (familyParameters.Count() != definitionsToAdd.Count)
                    throw new ArgumentNullException($"Operation Failed!!! Some parameters were not able to add to Family: {family.Name}");
            }, "Shared Parameters added");

            familyDocument.LoadFamily(doc, new LoadBatchFamiliesOption());
        }

        private static IEnumerable<FamilyParameter> TryAddSharedParametersToFamilyManager(this FamilyManager familyManager,
            List<(Definition definition, bool isInstance)> definitionData, ForgeTypeId groupTypeId)
        {
            foreach (var data in definitionData)
            {
                if (data.definition is not ExternalDefinition externalDefinition)
                    throw new ArgumentNullException($"Parameter {data.definition.Name} can not add to family.");

                if (!familyManager.FamilySharedParameterExists(externalDefinition, out FamilyParameter parameter))
                {
                    parameter = familyManager.AddParameter(externalDefinition, groupTypeId, data.isInstance);
                }

                yield return parameter;

            }
        }

        public static void DeleteSharedParametersFromFamily(this Family family, Document doc,
            List<Definition> definitions)
        {
            var familyDocument = doc.EditFamily(family) ?? throw new ArgumentNullException("family failed to edit");
            FamilyManager familyManager = familyDocument.FamilyManager;

            familyDocument.UseTransaction(() =>
            {

                foreach (var def in definitions)
                {
                    if (familyManager.FamilySharedParameterExists(def, out FamilyParameter familyParameter))
                    {
                        familyManager.RemoveParameter(familyParameter);
                    }
                }

            }, "Shared Parameters deleted");

            familyDocument.LoadFamily(doc, new LoadBatchFamiliesOption());
        }

        public static bool FamilySharedParameterExists(this FamilyManager manager,
            Definition definition, out FamilyParameter familyParameter)
        {
            familyParameter = manager.get_Parameter(definition.Name);

            if (familyParameter == null)
                return false;

            if(familyParameter.IsShared)
                return true;

            return false;
        }
    }
}
