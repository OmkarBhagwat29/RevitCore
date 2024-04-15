
using Autodesk.Revit.UI;

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
    }
}
