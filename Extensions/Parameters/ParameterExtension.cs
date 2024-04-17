
using Autodesk.Revit.UI;
using RevitCore.Extensions.Definition;
using RevitCore.Extensions.FamilyHelpers;

namespace RevitCore.Extensions.Parameters
{
    public static class ParameterExtension
    {
        public static void SetBuiltInParameterValue(this Element element, BuiltInParameter parameter, object value,bool showPopUp)
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

        public static void AddSharedParametersToFamily(this Family family, Document doc,
            List<ExternalDefinition> definitionsToAdd,ForgeTypeId groupTypeId, bool isInstance)
        {

          var familyDocument = doc.EditFamily(family);

            if (familyDocument == null)
                throw new ArgumentNullException("family failed to edit");

            FamilyManager familyManager = familyDocument.FamilyManager;

            familyDocument.UseTransaction(() =>
            {
                familyManager.AddSharedParametersToFamilyManager(definitionsToAdd, groupTypeId, isInstance);

            }, "Shared Parameters added");

            familyDocument.LoadFamily(doc, new LoadBatchFamiliesOption());
        }

        private static void AddSharedParametersToFamilyManager(this FamilyManager familyManager,
            List<ExternalDefinition> externalDefinitions,ForgeTypeId groupTypeId, bool isInstance)
        {
            foreach (var definition in externalDefinitions)
            {
                familyManager.AddParameter(definition, groupTypeId, isInstance);
            }
        }
    }
}
