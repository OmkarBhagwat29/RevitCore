
namespace RevitCore.Extensions.FamilyHelpers
{
    public class LoadFamilyOptions : IFamilyLoadOptions
    {
        private readonly bool overwriteParameterValues;

        public LoadFamilyOptions(bool _overwriteParameterValues)
        {
            overwriteParameterValues = _overwriteParameterValues;
        }
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = this.overwriteParameterValues;
            return true;
        }

        public bool OnSharedFamilyFound(Autodesk.Revit.DB.Family sharedFamily,
            bool familyInUse,
            out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;

            return true;
        }
    }
}
