using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Text;

namespace RevitCore.Extensions.Selection
{
    public static class SelectionFilterFactory
    {
        public static ElementSelectionFilter CreateElementSelectionFilter(Func<Element,bool> validateElement)
        {
            return new ElementSelectionFilter(validateElement);
        }

        public static LinkableSelectionFilter CreateLinkableSelectionFilter(Document doc,
            Func<Element, bool> validateElement)
        {
            return new LinkableSelectionFilter(doc, validateElement);
        }
    }
}
