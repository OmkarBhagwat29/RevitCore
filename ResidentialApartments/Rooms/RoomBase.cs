
using Autodesk.Revit.DB.Architecture;
using RevitCore.Extensions;
using RevitCore.Extensions.PointInPoly;
using RevitCore.GeometryUtils;
using RevitCore.ResidentialApartments.Validation;
using System.Windows.Controls;

namespace RevitCore.ResidentialApartments.Rooms
{
    public abstract class RoomBase
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public double MinimumArea { get; set; }
        public double MinimumWidth { get; set; }

        public abstract Room Room { get; }

        public List<ElementId> DoorIds { get; } = [];

        public List<ElementId> WindowIds { get; } = [];


        public List<ISpatialValidation> RoomValidationData { get; private set; } = [];

        public virtual void AddValidationData(ISpatialValidation apartmentValidation) => this.RoomValidationData.Add(apartmentValidation);


        public Solid ComputeRoomSolid(SpatialElementBoundaryOptions options, out List<XYZ> roomBoundaryPoints)
        {
            try
            {
                roomBoundaryPoints = this.GetRoomBoundaryPoints(options);
                CurveLoop curveLoop = new CurveLoop();

                //var boundarySegs = this.Room.GetBoundarySegments(options);
                var dupPoints = new List<XYZ>(roomBoundaryPoints);

                dupPoints.RemoveAt(dupPoints.Count - 1);

                for (int i = 0; i < dupPoints.Count; i++)
                {
                    int nextIndex = (i + 1) % dupPoints.Count;
                    Line ln = Line.CreateBound(dupPoints[i], dupPoints[nextIndex]);
                    curveLoop.Append(ln);
                }

                bool open = curveLoop.IsOpen();
                var len = curveLoop.GetExactLength();

                if (!curveLoop.IsValidObject || open)
                    return null;

                // Create a profile from the loop
                List<CurveLoop> profile = new List<CurveLoop> { curveLoop };

                // Create a solid from the profile by extruding
                SolidOptions solidOptions = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

                var geom = GeometryCreationUtilities.CreateExtrusionGeometry(profile, XYZ.BasisZ, 20, solidOptions);

                return geom;
            }
            catch (Exception)
            {
                roomBoundaryPoints = null;
                return null;
            }

        }

        private List<XYZ> GetRoomBoundaryPoints(SpatialElementBoundaryOptions options)
        {

            var baseVertices = this.Room.GetBoundaryPointsOfRoom(options);

            if (baseVertices.IsPolygonClosed())
            {
                if (baseVertices.IsPolygonClockwise())
                {
                    baseVertices.Reverse();
                }
            }

            baseVertices = baseVertices.RemoveCollinearVertices();
            baseVertices.Add(baseVertices.First());

            return baseVertices;

        }

        public bool IsElementInsideRoom(
    FamilyInstance element, double brickTolerance = 1.64042) //brick tolerance in feet
        {
            if (element.Location == null)
                return false;

            var loc = element.Location as LocationPoint;

            if (loc == null) return false;

            var pCenter = loc.Point;
            
            var pY_pos = loc.Point + (XYZ.BasisY * brickTolerance);
            var pY_neg = loc.Point - (XYZ.BasisY * brickTolerance);

            var pX_pos = loc.Point + (XYZ.BasisX * brickTolerance);
            var pX_neg = loc.Point - (XYZ.BasisX * brickTolerance);

            if (this.Room.IsPointInRoom(pCenter))
                return true;

            if (this.Room.IsPointInRoom(pY_pos))
                return true;

            if (this.Room.IsPointInRoom(pY_neg))
                return true;

            if (this.Room.IsPointInRoom(pX_pos))
                return true;

            if (this.Room.IsPointInRoom(pX_neg))
                return true;

            return false;
        }

    }
}
