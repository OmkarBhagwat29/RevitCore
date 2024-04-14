using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RevitCore.Extensions.Selection
{
    public class LinkDocumentOption : IPickElementsOption
    {
        public IEnumerable<Element> PickElements(UIDocument uiDoc, Func<Element, bool> validateElement)
        {
            var doc = uiDoc.Document;

            return uiDoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement,
                SelectionFilterFactory.CreateLinkableSelectionFilter(doc, validateElement))
                .Select(r =>
                {
                    var linkInstance = doc.GetElement(r.ElementId) as RevitLinkInstance;
                    return linkInstance.GetLinkDocument().GetElement(r.LinkedElementId);
                });
 
                
        }
    }
}
