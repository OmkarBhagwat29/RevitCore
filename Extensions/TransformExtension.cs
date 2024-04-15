
namespace RevitCore.Extensions
{
    public static class TransformExtension
    {
        public static void MoveElement(this Document doc, ElementId elmId, XYZ moveLocation) =>
            ElementTransformUtils.MoveElement(doc, elmId, moveLocation);

        public static void Visualize(this Transform xForm, Document doc, double scale = 3)
        {
            var colors = new List<Color>()
            {
                new Color(255,0,0),
                new Color(0,255,0),
                new Color(0,0,255),
            };

            var colorToLines = Enumerable.Range(0, 3)
                .Select(xForm.get_Basis)
                .Select(x => Line.CreateBound(xForm.Origin, xForm.Origin + (x * scale)))
                .Zip(colors, (line, color) => (Line: line, Color: color))
                .ToList();

            foreach (var (line, color) in colorToLines)
            {

                var directShape = doc.CreateDirectShape(
                    new List<GeometryObject>() { line });

                var overideGraphics = new OverrideGraphicSettings();
                overideGraphics.SetProjectionLineColor(color);
                overideGraphics.SetProjectionLineWeight(4);

                doc.ActiveView.SetElementOverrides(directShape.Id, overideGraphics);
            }

            xForm.Origin.VisualizePosition(doc);
        }
    }
}
