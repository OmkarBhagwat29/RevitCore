using Autodesk.Revit.DB.Architecture;


namespace RevitCore.Extensions.PointInPoly
{
    public static class PointInPolyExtension
    {
        

        /// <summary>
        /// Add new point to list, unless already present.
        /// </summary>
        private static void AddPoint(
          List<XYZ> XYZarray,
          XYZ p1)
        {
            var p = XYZarray.Where(
              c => Math.Abs(c.X - p1.X) < 0.001
                && Math.Abs(c.Y - p1.Y) < 0.001)
              .FirstOrDefault();

            if (p == null)
            {
                XYZarray.Add(p1);
            }
        }

        /// <summary>
        /// Return a list of boundary 
        /// points for the given room.
        /// </summary>
        public static List<XYZ> GetBoundaryPointsOfRoom(
          this Room room, SpatialElementBoundaryOptions opt = null)
        {
            if (opt==null)
            {
                opt = new SpatialElementBoundaryOptions()
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center
                };
            }

            var boundaries = room.GetBoundarySegments(
              opt);

            return GetBoundaryPoints(boundaries);
        }

        /// <summary>
        /// Return a list of boundary points 
        /// for the given boundary segments.
        /// </summary>
        private static List<XYZ> GetBoundaryPoints(
          IList<IList<BoundarySegment>> boundaries)
        {
            List<XYZ> puntArray = new List<XYZ>();
            foreach (var bl in boundaries)
            {
                foreach (var s in bl)
                {
                    Curve c = s.GetCurve();
                    AddPoint(puntArray, c.GetEndPoint(0));
                    AddPoint(puntArray, c.GetEndPoint(1));
                }
            }
            puntArray.Add(puntArray.First());
            return puntArray;
        }

        /// <summary>
        /// Return a list of boundary 
        /// points for the given area.
        /// </summary>
        private static List<XYZ> GetBoundaryPointsOfArea(
          Area area, SpatialElementBoundaryOptions opt = null)
        {
            if (opt == null)
            {
                opt = new SpatialElementBoundaryOptions();

                opt.SpatialElementBoundaryLocation
                  = SpatialElementBoundaryLocation.Center;
            }


            var boundaries = area.GetBoundarySegments(
              opt);

            if (boundaries == null || boundaries.Count == 0)
                return null;

            return GetBoundaryPoints(boundaries);
        }

        /// <summary>
        /// Check whether this area contains a given point.
        /// </summary>
        public static bool AreaContains(this Area a, XYZ p1, SpatialElementBoundaryOptions opt = null)
        {
            bool ret = false;
            var p = GetBoundaryPointsOfArea(a, opt);

            if(p == null)
                return false;

            PointInPoly pp = new PointInPoly();
            ret = pp.PolyGonContains(p, p1);
            return ret;
        }

        /// <summary>
        /// Check whether this room contains a given point.
        /// </summary>
        public static bool RoomContains(this Room r, XYZ p1, SpatialElementBoundaryOptions opt = null)
        {
            bool ret = false;
            var p = GetBoundaryPointsOfRoom(r,opt);
            PointInPoly pp = new PointInPoly();
            ret = pp.PolyGonContains(p, p1);
            return ret;
        }

        /// <summary>
        /// Project an XYZ point to a UV one in the 
        /// XY plane by simply dropping the Z coordinate.
        /// </summary>
        public static UV TOUV(this XYZ point)
        {
            UV ret = new UV(point.X, point.Y);
            return ret;
        }

    }
}
