using RevitCore.Extensions;
using RevitCore.GeometryUtils.BoundingBox;


namespace RevitCore.GeometryUtils
{
    public static class BoundingBoxXYZExtensions
    {
        public static AdvancedBoundingBoxXYZ ToAdvanced(this BoundingBoxXYZ box)
        {
            return AdvancedBoundingBoxXYZ.Create(box);
        }
        public static List<XYZ> GetVertices(this BoundingBoxXYZ box)
        {
            return Enumerable.Range(0, 2).Select(i => box.Transform.OfPoint(box.get_Bounds(i))).ToList();
        }
        public static AdvancedBoundingBoxXYZ Merge(this IEnumerable<BoundingBoxXYZ> boxes)
        {
            var vertices = boxes.SelectMany(b => b.GetVertices()).ToList();
            return new AdvancedBoundingBoxXYZ()
            {
                Min = vertices.GetMinByCoordinates(),
                Max = vertices.GetMaxByCoordinates()
            };
        }
    }
}
