using Autodesk.Revit.DB;



namespace RevitCore.Extensions.Selection
{
    public class ElementSelectionFilter : BaseSelectionFilter
    {

        private readonly Func<Reference, bool> ValidateReference;

        public ElementSelectionFilter(Func<Element,bool> validateElement) : base(validateElement)
        {
        }

        public ElementSelectionFilter(
            Func<Element, bool> validateElement, Func<Reference,bool> validateReference) : this(validateElement)
        {
            this.ValidateReference = validateReference;
        }

        public override bool AllowElement(Element elem)
        {
            return this.ValidateElement(elem);
        }

        public override bool AllowReference(Reference reference, XYZ position)
        {
            return ValidateReference?.Invoke(reference) ?? true;
        }
    }
}
