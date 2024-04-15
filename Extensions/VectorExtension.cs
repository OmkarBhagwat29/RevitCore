

namespace RevitCore.Extensions
{
    public static class VectorExtension
    {
        public static Line AsCurve(this XYZ vector, XYZ origin = null, double? length = null)
        {
            origin = origin ?? XYZ.Zero;
            length = length ?? vector.GetLength();

            return Line.CreateBound(origin, origin.MoveAlongVector(vector,length.GetValueOrDefault()));
        }

        public static void VisualizePosition(this XYZ Vector, Document doc)
        {
            Point pt = Point.Create(Vector);

            DocumentExtension.CreateDirectShape(doc, new List<GeometryObject> { pt }); 
        }

        public static XYZ MoveAlongVector(this XYZ pointToMove, XYZ vector, double distance)
        {
            vector = vector.Normalize();
            return  pointToMove.Add(vector * distance);
        }
    }
}
