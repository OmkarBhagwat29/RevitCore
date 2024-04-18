
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
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

        public static void TryAddSharedParametersToFamily(this Family family, Document doc,
            List<(Definition definition, bool isInstance)> definitionsToAdd,ForgeTypeId groupTypeId)
        {

          var familyDocument = doc.EditFamily(family) ?? throw new ArgumentNullException("family failed to edit");

            FamilyManager familyManager = familyDocument.FamilyManager;

            familyDocument.UseTransaction(() =>
            {
                familyManager.TryAddSharedParametersToFamilyManager(definitionsToAdd, groupTypeId);

            }, "Shared Parameters added");

            familyDocument.LoadFamily(doc, new LoadBatchFamiliesOption());
        }

        private static void TryAddSharedParametersToFamilyManager(this FamilyManager familyManager,
            List<(Definition definition, bool isInstance)> definitionData,ForgeTypeId groupTypeId)
        {
            try
            {
                foreach (var data in definitionData)
                {
                    if (data.definition is not ExternalDefinition externalDefinition)
                        throw new ArgumentNullException($"Parameter {data.definition.Name} can not add to family.");

                    if(!familyManager.FamilySharedParameterExists(externalDefinition,out FamilyParameter parameter))
                        familyManager.AddParameter(externalDefinition, groupTypeId, data.isInstance);
                }
            }
            catch (Exception e)
            {
                throw e;
            }

        }

        public static void TryDeleteSharedParametersFromFamily(this Family family, Document doc,
            List<Definition> definitions)
        {
            var familyDocument = doc.EditFamily(family) ?? throw new ArgumentNullException("family failed to edit");
            FamilyManager familyManager = familyDocument.FamilyManager;

            familyDocument.UseTransaction(() => {

                foreach (var def in definitions)
                {
                    if (familyManager.FamilySharedParameterExists(def as ExternalDefinition, out FamilyParameter familyParameter))
                    {
                        familyManager.RemoveParameter(familyParameter);
                    }
                }
            
            }, "Shared Parameters deleted");
        }

        public static bool FamilySharedParameterExists(this FamilyManager manager,
            ExternalDefinition externalDefinition, out FamilyParameter familyParameter)
        {
            familyParameter = null;
            foreach (var item in manager.Parameters)
            {
                if (item is not FamilyParameter fm)
                    continue;

                if (fm.Definition is ExternalDefinition extDef)
                {
                    if (extDef.GUID == externalDefinition.GUID)
                    {
                        familyParameter = fm;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
