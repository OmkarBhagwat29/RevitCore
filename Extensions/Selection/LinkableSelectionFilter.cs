using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitCore.Extensions.Selection
{
    public class LinkableSelectionFilter : BaseSelectionFilter
    {
        public LinkableSelectionFilter(Document doc, Func<Element,bool> validateElement) : base(validateElement)
        {
            Doc = doc;
        }

        public Document Doc { get; }

        public override bool AllowElement(Element elem) => true;

        public override bool AllowReference(Reference reference, XYZ position)
        {
            if(!(Doc.GetElement(reference.ElementId) is RevitLinkInstance linkInstance)) {
                return ValidateElement(Doc.GetElement(reference.ElementId));
            }

            var element = linkInstance.GetLinkDocument()
                .GetElement(reference.LinkedElementId);

            return ValidateElement(element);

        }
    }
}
