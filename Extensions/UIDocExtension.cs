using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;


namespace RevitCore.Extensions.Selection
{

    public static class UIDocExtension
    {
        public static IEnumerable<Element> GetPreSelectedElements(this UIDocument uiDoc)
        {
            return uiDoc.Selection.GetElementIds().Select(id=>uiDoc.Document.GetElement(id));   
        }

        public static IEnumerable<Element> PickElements(this UIDocument uiDoc,Func<Element,bool> validateElement,
            IPickElementsOption pickElementsOption)
        {
            return pickElementsOption.PickElements(uiDoc, validateElement);
        }
    }
}
