using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using RevitCore.Extensions;
using RevitCore.Extensions.PointInPoly;
using RevitCore.Extensions.Selection;



namespace RevitCore.ResidentialApartments
{
    public class AreaRoomAssociation
    {
        //public static Document Doc;

        public AreaRoomAssociation(Area areaBoundary, List<Room>rooms) {
            AreaBoundary = areaBoundary;
            Rooms = rooms;

            //this.SpatialElements = new List<SpatialElement>();
            //this.SpatialElements.Add(areaBoundary);
            //this.SpatialElements.AddRange(rooms);
        }

        public Area AreaBoundary { get; }

        public List<Room> Rooms { get; }

        //public List<SpatialElement> SpatialElements { get; }

        public static AreaRoomAssociation CreateAreaRoomsAssociationBySelection(UIDocument uiDoc)
        {
            var areaRoomElements = uiDoc.PickElements((e) => e is SpatialElement,
                PickElementOptionFactory.CreateCurrentDocumentOption());

            //distinguish 
            var areas = areaRoomElements.Where(e=>e.GetType()==typeof(Area))
                .Cast<Area>()
                .ToList();

            if (areas.Count > 1)
                throw new Exception("Only one area boundary is allowed to select, you selected multiple");

            if (areas.Count == 0)
                throw new Exception("No area boundary is selected that defines the apartment");

            var rooms = areaRoomElements.Where(e => e is Room).Cast<Room>().ToList();

            return new AreaRoomAssociation(areas[0], rooms);
        }

        public static List<AreaRoomAssociation> GetCWOApartmentsInProject(Document doc,
            Func<Area,bool>AreaPassCondition)
        {
            //get all the spatial elements
            var elements = doc.GetElements<SpatialElement>().ToList();

            //get only areas
            var areas = elements.Where(e => e is Area).Cast<Area>().ToList();

            //get only rooms
            var rooms = elements.Where(e => e is Room).ToList();

            //get all rooms for each areas and create data
            List<AreaRoomAssociation> assList = [];
            foreach (var area in areas)
            {
                if (!AreaPassCondition(area))
                    continue;

                List<Room> apartmentRooms = [];

                foreach (var room in rooms)
                {
                    if (room.Location == null)
                        continue;

                    var roomLoc = room.Location as LocationPoint;

                    if (area.LevelId != room.LevelId)
                        continue;

                    if (area.AreaContains(roomLoc.Point))
                    {
                        apartmentRooms.Add(room as Room);
                    }
                }

                if (apartmentRooms.Count > 0)
                {
                    assList.Add(new AreaRoomAssociation(area, apartmentRooms));
                }

            }

            return assList;
        }
    }
}
