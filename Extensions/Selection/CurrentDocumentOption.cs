using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitCore.Extensions.Selection
{
    public class CurrentDocumentOption : IPickElementsOption
    {
        public IEnumerable<Element> PickElements(UIDocument uiDoc, Func<Element, bool> validateElement)
        {
           return uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element,
                SelectionFilterFactory.CreateElementSelectionFilter(validateElement))
                .Select(r=>uiDoc.Document.GetElement(r.ElementId));
        }
    }
}
