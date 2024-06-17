
using Autodesk.Revit.DB.Architecture;
using RevitCore.Extensions;
using RevitCore.Extensions.PointInPoly;
using RevitCore.GeometryUtils;
using RevitCore.GeometryUtils.BoundingBox;
using RevitCore.ResidentialApartments.Rooms;
using System.Text;


namespace RevitCore.ResidentialApartments.Validation
{
    public class DimensionValidation : ISpatialValidation
    {
        public DimensionValidation(Solid spatialElementSolid, List<XYZ> solidBasePoints,
            double requiredMinWidth, Type spatialType)
        {
            this.RoomSolid = spatialElementSolid;
            SolidBasePoints = solidBasePoints;
            RequiredMinWidth = requiredMinWidth;
            SpatialType = spatialType;
            if (this.RoomSolid != null)
            {
                _isRectilinear = solidBasePoints.IsPolygonRectilinear();
            }

        }


        bool _isRectilinear = false;

        public Type SpatialType { get; }

        public Solid RoomSolid { get; }
        public List<XYZ> SolidBasePoints { get; }
        public string ApartmentNumber { get; }
        public double RequiredMinWidth { get; }
        public double AchievedMinWidth { get; private set; } = -1;

        public List<Solid> PartitionSolids { get; private set; } = [];

        public bool ValidationSuccess { get; private set; } = false;

        public Solid ResultSolid { get; private set; }

        public void Bake(Document doc)
        {
            if (this.ResultSolid == null && this.RoomSolid!=null)
            {
               this.RoomSolid.Visualize(doc);

                return;
            }

            this.PartitionSolids.ForEach(s=> s.Visualize(doc));
        }

        public string GetValidationReport()
        {
            StringBuilder report = new StringBuilder();

            if (this.ValidationSuccess)
            {
                return string.Empty;
            }

            if (!_isRectilinear)
            {
                report.Append($"Error: Room boundary is not a Rectilinear Polygon or it is self intersecting.");
            }
            else if (this.ResultSolid != null && this.RequiredMinWidth > this.AchievedMinWidth)
            {
                string message = message = $"Error: Required width is greater than achieved width.";

                report.AppendLine(message);
            }

            return report.ToString();
        }

        public void Validate()
        {
            if (!_isRectilinear)
                return;
            try
            {

                if (this.RoomSolid == null)
                    return;

                var boundaryPoints = new List<XYZ>(this.SolidBasePoints);

                var partitionPoints = this.RoomSolid.PartitionRoom(boundaryPoints, out List<Solid> partitionSolids);

                if (partitionSolids == null || partitionSolids.Count == 0)
                    return;

                this.PartitionSolids = partitionSolids.OrderByDescending(p => p.SurfaceArea).ToList();
                this.ResultSolid = PartitionSolids.First();

                var bbx = this.ResultSolid.GetBoundingBox().ToAdvanced();

                this.AchievedMinWidth = bbx.Width < bbx.Length ? bbx.Width.ToMeters() : bbx.Length.ToMeters();

                this.ValidationSuccess = this.AchievedMinWidth >= this.RequiredMinWidth;

            }
            catch (Exception)
            {

                return;
            }

        }
    }
}
