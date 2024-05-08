using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.GeometryUtils.BoundingBox
{
    public class Cuboid : IGeometryShape
    {
        public Solid ToSolid(AdvancedBoundingBoxXYZ advancedBoundingBoxXYZ)
        {
            var curveLoop = advancedBoundingBoxXYZ.BaseCurveLoop;
            var solid = GeometryCreationUtilities.CreateExtrusionGeometry(
                new List<CurveLoop>() { curveLoop },
                advancedBoundingBoxXYZ.Transform.BasisZ,
                advancedBoundingBoxXYZ.Height);
            return solid;

        }
    }
}
