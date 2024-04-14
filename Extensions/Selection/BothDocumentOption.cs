using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitCore.Extensions.Selection
{
    public class BothDocumentOption : IPickElementsOption
    {
        public IEnumerable<Element> PickElements(UIDocument uiDoc, Func<Element, bool> validateElement)
        {
            var doc = uiDoc.Document;

            return uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.PointOnElement,
                SelectionFilterFactory.CreateLinkableSelectionFilter(doc, validateElement))
                .Select(r =>
                {

                    if (!(doc.GetElement(r.ElementId) is RevitLinkInstance linkInstance))
                    {
                        return doc.GetElement(r.ElementId);
                    }

                    return linkInstance.GetLinkDocument().GetElement(r.LinkedElementId);

                });
        }
    }
}
