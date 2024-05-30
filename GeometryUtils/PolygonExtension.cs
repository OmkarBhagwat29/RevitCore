

using Autodesk.Revit.DB.Architecture;
using RevitCore.Extensions;
using RevitCore.Extensions.PointInPoly;
using RevitCore.GeometryUtils.BoundingBox;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace RevitCore.GeometryUtils
{
    public enum PolygonVertexType
    {
        Collinear,
        Convex,
        Concave
    }
    public static class PolygonExtension
    {
        public static bool IsPolygonClosed(this List<XYZ> vertices)
        {
            // Check if the first and last vertices are the same
            return vertices.Count > 2 && vertices.First().IsAlmostEqualTo(vertices.Last());
        }

        public static bool IsPolygonClockwise(this List<XYZ> vertices)
        {
            double sum = 0.0;

            // Iterate over the edges
            for (int i = 0; i < vertices.Count; i++)
            {
                var current = vertices[i];
                var next = vertices[(i + 1) % vertices.Count]; // Wrap around to the first point

                sum += (next.X - current.X) * (next.Y + current.Y);
            }

            // If the sum is positive, the polygon is clockwise
            return sum > 0.0;
        }


        public static (int startIndex, int endIndex) ShortestSegmentOfPolygon(this List<XYZ> vertices)
        {
            double min = double.MaxValue;
            int startIndex = -1;
            int endIndex = -1;
            for (int i = 0; i < vertices.Count; ++i)
            {
                var currentIndex = i;
                var nextIndex = (i + 1) % vertices.Count;
                var current = vertices[currentIndex];
                var next = vertices[nextIndex]; // Wrap around to the first point

                double dist = current.DistanceTo(next);

                if (dist < min)
                {
                    min = dist;
                    startIndex = currentIndex;
                    endIndex = nextIndex;
                }
            }

            return (startIndex, endIndex);
        }


        public static (int startIndex, int endIndex) ShortestSegmentOfPolygon(this List<XYZ> vertices,
            PolygonVertexType vertexType)
        {
            double min = double.MaxValue;
            int startIndex = -1;
            int endIndex = -1;
            for (int i = 0; i < vertices.Count; ++i)
            {
                var currentIndex = i;
                var nextIndex = (i + 1) % vertices.Count;
                var current = vertices[currentIndex];
                var next = vertices[nextIndex]; // Wrap around to the first point

                if (vertices.GetPolygonVertexType(currentIndex) != vertexType ||
                    vertices.GetPolygonVertexType(nextIndex) != vertexType)
                    continue;

                double dist = current.DistanceTo(next);
                if (dist < min)
                {
                    min = dist;
                    startIndex = currentIndex;
                    endIndex = nextIndex;
                }
            }

            return (startIndex, endIndex);
        }


        /// <summary>
        /// Must be a counter-clockwise polygon
        /// </summary>
        /// <param name="vertexIndexToCheck"></param>
        /// <param name="_vertices"></param>
        /// <returns></returns>
        public static PolygonVertexType GetPolygonVertexType(this List<XYZ> _vertices, int vertexIndexToCheck)
        {
           var vertex = _vertices[vertexIndexToCheck];

            int prevIndex = (vertexIndexToCheck - 1 + _vertices.Count) % _vertices.Count;
            int nextIndex = (vertexIndexToCheck + 1) % _vertices.Count;

            XYZ prevVec = _vertices[vertexIndexToCheck] - _vertices[prevIndex];
            XYZ nextVec = _vertices[nextIndex] - _vertices[vertexIndexToCheck];

            double crossProduct = prevVec.CrossProduct(nextVec).Z;

            crossProduct = Math.Round(crossProduct,4);

            if (crossProduct.IsAlmostEqual(0,0.0001))
            {
                // Collinear vertex
                return PolygonVertexType.Collinear;
            }
            else if (crossProduct < 0)
            {
                return PolygonVertexType.Concave;
            }
            else
            {
                // Convex vertex
                return PolygonVertexType.Convex;
            }
        }

        public static bool IsPolygonRectilinear(this List<XYZ> _vertices)
        {

            for (int i = 0; i < _vertices.Count; i++)
            {
                int vertexIndexToCheck = i;

                var vertex = _vertices[vertexIndexToCheck];

                int prevIndex = (vertexIndexToCheck - 1 + _vertices.Count) % _vertices.Count;
                int nextIndex = (vertexIndexToCheck + 1) % _vertices.Count;

                XYZ prevVec = _vertices[vertexIndexToCheck] - _vertices[prevIndex];
                XYZ nextVec = _vertices[nextIndex] - _vertices[vertexIndexToCheck];

                double dot = prevVec.DotProduct(nextVec);

                if (!dot.IsAlmostEqual(0.0))
                    return false;
            }

            return true;

        }


        public static List<XYZ> RemoveCollinearVertices(this List<XYZ> vertices)
        {
            if (vertices == null || vertices.Count < 3)
            {
                return vertices;
            }
            //skip first
            List<XYZ> result = new List<XYZ>();
            result.Add(vertices[0]);
            for (int i = 1; i < vertices.Count; i++)
            {
                PolygonVertexType type = GetPolygonVertexType(vertices,i);

                if (type != PolygonVertexType.Collinear)
                {
                    result.Add(vertices[i]);
                }
            }

            return result;
        }



        public static ((int startIndex, int endIndex)firstChord, (int startIndex, int endIndex)lastChord)
            GetRectilinearPolygonAdjacent(this (int startIndex, int endIndex) chordPoints, int verticesCount)
        {

            //var data = new List<(int startIndex, int endIndex)>();

            int prevIndex = (chordPoints.startIndex - 1 + verticesCount) % verticesCount;

            int nextIndex = (chordPoints.endIndex + 1) % verticesCount;


            var firstChord = (chordPoints.startIndex,prevIndex);

            var lastChord = (chordPoints.endIndex, nextIndex);

            return (firstChord,lastChord);
        }


        public static (int startIndex, int endIndex) GetSmallestChord(this List<XYZ> _vertices, (int startIndex, int endIndex) chord1,
            (int startIndex, int endIndex) chord2)
        {
            var chord1_start = _vertices[chord1.startIndex];
            var chord1_end = _vertices[chord1.endIndex];

            var chord2_start = _vertices[chord2.startIndex];
            var chord2_End = _vertices[chord2.endIndex];

            double chord_1_Distance = chord1_start.DistanceTo(chord1_end);

            double chord_2_Distance = chord2_start.DistanceTo(chord2_End);

            return chord_1_Distance <= chord_2_Distance ? chord1 : chord2;
        }


        public static List<XYZ> CreateRectangularPartition(List<XYZ>_vertices, 
            (int startIndex, int endIndex) mainChord,
            (int startIndex, int endIndex) adjacentChord)
        {
            var rectangularPartition = new List<XYZ>();

            var mainChord_StartVertex = _vertices[mainChord.startIndex];
            var mainChord_EndVertex = _vertices[mainChord.endIndex];

            var adjacentChord_StartVertex = _vertices[adjacentChord.startIndex];
            var adjacentChord_EndVertex = _vertices[adjacentChord.endIndex];

            double width = mainChord_EndVertex.DistanceTo(mainChord_StartVertex);
            double Length = adjacentChord_EndVertex.DistanceTo(adjacentChord_StartVertex);

            //offset mainChord
            Line mainLine = Line.CreateBound(mainChord_StartVertex, mainChord_EndVertex);
            XYZ offsetVector = adjacentChord_EndVertex - adjacentChord_StartVertex;
            //offsetVector = offsetVector.Normalize();

            var offsetLine = mainLine.CreateTransformed(Transform.CreateTranslation(offsetVector)) as Line;

            rectangularPartition.Add(mainChord_StartVertex);
            rectangularPartition.Add(mainChord_EndVertex);
            rectangularPartition.Add(offsetLine.GetEndPoint(1));
            rectangularPartition.Add(offsetLine.GetEndPoint(0));

            return rectangularPartition;
        }


        public static List<XYZ> CreateNewRegion(this Solid currentRoomSolid, List<XYZ> rectangularPartition,
            out Solid modifiedSolid, out Solid rectangularRegionSolid)
        {

            var lines = new List<Line>();

            lines.Add(Line.CreateBound(rectangularPartition[0], rectangularPartition[1]));
            lines.Add(Line.CreateBound(rectangularPartition[1], rectangularPartition[2]));
            lines.Add(Line.CreateBound(rectangularPartition[2], rectangularPartition[3]));
            lines.Add(Line.CreateBound(rectangularPartition[3], rectangularPartition[0]));

            CurveLoop curveLoop = new CurveLoop();
            lines.ForEach(ln => curveLoop.Append(ln));

            // Create a profile from the loop
            List<CurveLoop> profile = new List<CurveLoop> { curveLoop };

            // Create a solid from the profile by extruding
            SolidOptions solidOptions = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
            rectangularRegionSolid = GeometryCreationUtilities.CreateExtrusionGeometry(profile, XYZ.BasisZ, 20);

            modifiedSolid = BooleanOperationsUtils.ExecuteBooleanOperation(currentRoomSolid, rectangularRegionSolid, BooleanOperationsType.Difference);

            if (modifiedSolid.Faces.Size == 0)
                modifiedSolid = rectangularRegionSolid;
   
            var newLoop = modifiedSolid.GetFaces()[1].GetEdgesAsCurveLoops().ToArray().FirstOrDefault().ToList();

            List<XYZ> newVertices = new List<XYZ>();

            for (int i = 0; i < newLoop.Count; i++)
            {
                var ln = newLoop[i] as Line;

                newVertices.Add(ln.GetEndPoint(0));
            }

            //close the polygon
            newVertices.Add(newVertices[0]);


            return newVertices;

        }


        public static List<List<XYZ>> PartitionRoom(this Solid roomSolid,List<XYZ>roomBoundaryPoints,
            out List<Solid> partitionSolids)
        {
            partitionSolids = [];
            List<List<XYZ>> partitions = [];

            //duplicate
            roomSolid = roomSolid.Clone();
            roomBoundaryPoints = new List<XYZ>(roomBoundaryPoints);
            int counter = 0;
            try
            {
                if (roomSolid.Faces.Size > 6)
                {
                    while (roomSolid.Faces.Size != 6 && counter < 10)
                    {
                        
                        if (roomBoundaryPoints.IsPolygonClosed())
                        {
                            if (roomBoundaryPoints.IsPolygonClockwise())
                            {
                                roomBoundaryPoints.Reverse();
                            }
                        }

                        roomBoundaryPoints = roomBoundaryPoints.RemoveCollinearVertices();

                        //remove last point as it matches to first
                        if (roomBoundaryPoints.First().IsAlmostEqualTo(roomBoundaryPoints.Last()))
                        {
                            roomBoundaryPoints.RemoveAt(roomBoundaryPoints.Count - 1);
                        }


                        var mainChord = roomBoundaryPoints.ShortestSegmentOfPolygon(PolygonVertexType.Convex);

                        if (mainChord.startIndex >= 0 && mainChord.endIndex >= 0)
                        {
                            var (firstChord, lastChord) = (mainChord.startIndex, mainChord.endIndex).GetRectilinearPolygonAdjacent(roomBoundaryPoints.Count);

                            var smallestChord = roomBoundaryPoints.GetSmallestChord(firstChord, lastChord);

                            var rectangularPartition = CreateRectangularPartition(roomBoundaryPoints, mainChord, smallestChord);

                            if (rectangularPartition == null)
                                break;

                            roomBoundaryPoints = CreateNewRegion(roomSolid, rectangularPartition,
                                out Solid modifiedSolid, out Solid partitionSolid);

                            roomSolid = modifiedSolid.Clone();

                            partitionSolids.Add(partitionSolid);

                            partitions.Add(rectangularPartition);

                            if (roomSolid.Faces.Size == 6)
                            {
                                partitionSolids.Add(roomSolid);

                                //remove last point as it matches to first
                                if (roomBoundaryPoints.First().IsAlmostEqualTo(roomBoundaryPoints.Last()))
                                {
                                    roomBoundaryPoints.RemoveAt(roomBoundaryPoints.Count - 1);
                                }
                                partitions.Add(roomBoundaryPoints);
                            }

                        }

                        counter++;
                    }
                }
                else
                {
                    partitionSolids.Add(roomSolid);

                    //remove last point as it matches to first
                    if (roomBoundaryPoints.First().IsAlmostEqualTo(roomBoundaryPoints.Last()))
                    {
                        roomBoundaryPoints.RemoveAt(roomBoundaryPoints.Count - 1);
                    }
                    partitions.Add(roomBoundaryPoints);
                }
            }
            catch (Exception)
            {
                return null;
            }

            return partitions;
        }


    }
}
