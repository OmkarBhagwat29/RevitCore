using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.GeometryUtils.BoundingBox
{
    public interface IGeometryShape
    {
        Solid ToSolid(
            AdvancedBoundingBoxXYZ advancedBoundingBoxXYZ);
    }
}
