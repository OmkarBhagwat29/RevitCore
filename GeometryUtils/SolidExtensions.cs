using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using RevitCore.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace RevitCore.GeometryUtils
{
    public static class SolidExtensions
    {
        public static void Visualize(this Solid solid, Document document)
        {
            document.CreateDirectShape(solid);
        }
        public static Solid Union(
            this IEnumerable<Solid> solids)
        {
            return solids
                .Where(x => x.HasVolume())
                .Aggregate((x, y) => BooleanOperationsUtils.ExecuteBooleanOperation(
                        x, y, BooleanOperationsType.Union));
        }
        public static Solid CreateTransformed(
            this Solid solid, Transform transform)
        {
            return SolidUtils.CreateTransformed(solid, transform);
        }

        public static bool HasVolume(
            this Solid solid) => solid.Volume > 0;

        public static bool HasFaces(
            this Solid solid) => solid.Faces.Size > 0;


        public static List<Face> GetFaces(
            this Solid solid)
        {
            return solid.Faces.OfType<Face>().ToList();

        }

        public static List<Curve> GetCurves(
            this Solid solid)
        {
            return solid.GetFaces()
                .SelectMany(x => x.GetEdgesAsCurveLoops())
                .SelectMany(x => x)
                .ToList();
        }

        public static List<XYZ> GetVertices(
            this Solid solid)
        {
            return solid.GetCurves()
                .SelectMany(x => x.Tessellate()).ToList();
        }

        public static Solid GetSolidFromSpatialElement(this SpatialElement element, Document doc, SpatialElementBoundaryOptions sebOptions = null)
        {

            if (sebOptions == null)
            {
                sebOptions = new SpatialElementBoundaryOptions
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
                };
            }


                SpatialElementGeometryCalculator cal = new SpatialElementGeometryCalculator(doc, sebOptions);
                SpatialElementGeometryResults results = cal.CalculateSpatialElementGeometry(element);
                Solid roomSolid = results.GetGeometry();

                return roomSolid;
        }

        public static Solid GetSolidFromAreaBoundary(this Area areaBoundary,double extrusion, SpatialElementBoundaryOptions sebOptions = null)
        {
            try
            {
                if (sebOptions == null)
                {
                    sebOptions = new SpatialElementBoundaryOptions
                    {
                        SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Finish,
                    };
                }

                var segments = areaBoundary.GetBoundarySegments(sebOptions);

                // Create a profile from the loop
                List<CurveLoop> profile = new List<CurveLoop> { };

                foreach (var item in segments)
                {
                    var cvLoop = new CurveLoop();
                    foreach (var seg in item)
                    {
                        cvLoop.Append(seg.GetCurve());
                    }

                    profile.Add(cvLoop);
                }

                // Create a solid from the profile by extruding
                SolidOptions solidOptions = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);

                var areaGeom = GeometryCreationUtilities.CreateExtrusionGeometry(profile, XYZ.BasisZ, extrusion, solidOptions);

                return areaGeom;
            }
            catch
            {
                return null;
            }

        }

        public static bool IntersectsWithElement(this Solid areaSolid, XYZ elementLocation, double tol = 0.5)
        {

            var s1 = elementLocation - (XYZ.BasisZ * tol);
            var s2 = elementLocation + (XYZ.BasisZ * tol);

            var roomLine = Line.CreateBound(s1, s2);

            SolidCurveIntersection cvs = areaSolid.IntersectWithCurve(roomLine, new SolidCurveIntersectionOptions()
            { ResultType = SolidCurveIntersectionMode.CurveSegmentsInside });

            var isInside = cvs.SegmentCount > 0;

            return isInside;
        }
    }

}
