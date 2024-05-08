

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

        public static XYZ MoveAlongVector(this XYZ pointToMove, XYZ vector) => pointToMove.Add(vector);

        /// <summary>
        /// Get a vector from one point to another
        /// </summary>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <returns></returns>
        public static XYZ ToVector(
            this XYZ firstPoint, XYZ secondPoint)
        {
            return (secondPoint - firstPoint);
        }

        public static XYZ GetMinByCoordinates(
    this IEnumerable<XYZ> points)
        {
            var minPoint = new XYZ(
                points.Min(x => x.X),
                points.Min(x => x.Y),
                points.Min(x => x.Z));
            return minPoint;
        }

        public static XYZ GetMaxByCoordinates(
    this IEnumerable<XYZ> points)
        {
            var minPoint = new XYZ(
                points.Max(x => x.X),
                points.Max(x => x.Y),
                points.Max(x => x.Z));
            return minPoint;
        }
    }
}
