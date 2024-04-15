
namespace RevitCore.Extensions
{
    public static class CurveExtension
    {
        public static void Visualize(this Curve cv, Document doc) 
            => DocumentExtension.CreateDirectShape(doc, new List<GeometryObject>() { cv });

        public static DetailCurve CreateDetailCurve(this Curve cv, Document doc, View view)
        {
            // if (view is View3D view3D) throw new ArgumentException("Detail curve can not be created in 3d view");

            return doc.Create.NewDetailCurve(view, cv);
        }
    }
}
