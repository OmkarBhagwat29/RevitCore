using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;


namespace RevitCore.Extensions.Selection
{
    public interface IPickElementsOption
    {
        IEnumerable<Element> PickElements(UIDocument uiDoc, Func<Element, bool> validateElement);
    }
}
