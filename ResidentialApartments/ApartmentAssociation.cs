using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using RevitCore.Extensions;
using RevitCore.Extensions.PointInPoly;
using RevitCore.Extensions.Selection;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;



namespace RevitCore.ResidentialApartments
{
    public class ApartmentAssociation
    {
        //public static Document Doc;

        public ApartmentAssociation(Area areaBoundary, List<Room>rooms) {
            AreaBoundary = areaBoundary;
            Rooms = rooms;
        }

        public Area AreaBoundary { get; }

        public List<Room> Rooms { get; }

        public static ApartmentAssociation CreateAreaRoomsAssociationBySelection(UIDocument uiDoc)
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

            return new ApartmentAssociation(areas[0], rooms);
        }

        public static List<ApartmentAssociation> GetAreaRoomAssociationInProject(UIApplication uiApp,
            Func<Area,bool>AreaPassCondition)
        {
            //get all the spatial elements
            var doc = uiApp.ActiveUIDocument.Document;
            var elements = doc.GetElements<SpatialElement>().ToList();

            foreach (Document linkedDoc in uiApp.Application.Documents)
            {
                if (linkedDoc.PathName == doc.PathName || !linkedDoc.IsValidObject)
                    continue;

                    var spatialElements = linkedDoc.GetElements<SpatialElement>().ToList();

                    if (spatialElements.Count > 0)
                    {
                        elements.AddRange(spatialElements);
                    }

            }

            //get only areas
            var areas = elements.Where(e => e is Area).Cast<Area>()
                .Where(AreaPassCondition)
                .GroupBy(a => a.LevelId)
                .ToList();

            //get only rooms
            var rooms = elements.Where(e => e is Room)
                .Cast<Room>()
                .Where(r => r.Location != null)
                .GroupBy(s => s.LevelId)
                .ToList();

            //get all rooms for each areas and create data
            List<ApartmentAssociation> assList = [];

            foreach (IGrouping < ElementId, Area > areaData in areas)
            {
                foreach (var areaBoundary in areaData)
                {
                    bool roomsOnSameLevel = false;
                    List<Room> apartmentRooms = [];
                    foreach (IGrouping<ElementId, Room> roomData in rooms)
                    {
                        foreach (var room in roomData)
                        {
                            if (areaBoundary.LevelId != room.LevelId)
                            {
                                var areaLevel = areaBoundary.Document.GetElement(areaBoundary.LevelId) as Level;
                                var roomLevel = room.Document.GetElement(room.LevelId) as Level;

                                var areaElevation = areaLevel.LookupParameter("Elevation").AsDouble();
                                var roomElevation = roomLevel.LookupParameter("Elevation").AsDouble();

                                if (!areaElevation.IsAlmostEqual(roomElevation))
                                {
                                    var diff = 0.82021; //this is feet = 250 mm

                                    var roomElevPos = roomElevation + diff;

                                    if (!areaElevation.IsAlmostEqual(roomElevPos))
                                    {
                                        var roomElevNeg = roomElevation - diff;

                                        if (!areaElevation.IsAlmostEqual(roomElevNeg))
                                        {
                                            break;
                                        }
                                    }

                                }
                            }

                            roomsOnSameLevel = true;
                            break;
                        }

                        if (!roomsOnSameLevel)
                        {
                            continue;
                        }

                        foreach (var room in roomData)
                        {
                            var roomLoc = room.Location as LocationPoint;

                            if (areaBoundary.AreaContains(roomLoc.Point))
                            {
                                apartmentRooms.Add(room);
                            }
                        }

                        roomsOnSameLevel = false;
                    }

                    if (apartmentRooms.Count > 0)
                    {
                        assList.Add(new ApartmentAssociation(areaBoundary, apartmentRooms));
                    }
                }

            }

            return assList;
        }

        public static List<FamilyInstance> GetDoors(UIApplication uiApp)
        {
           // var doc = uiApp.ActiveUIDocument.Document;
           var doors = new List<FamilyInstance>();  
            foreach (Document doc in uiApp.Application.Documents)
            {

                var fis = doc.GetElements<FamilyInstance>((e) =>
                {
                    if (e.LevelId == null)
                        return false;
#if REVIT2022
                    return (BuiltInCategory)e.Category.Id.IntegerValue == BuiltInCategory.OST_Doors;
#else
                    return e.Category.BuiltInCategory == BuiltInCategory.OST_Doors;
#endif
                });


                if (fis.Count() > 0)
                {
                    doors.AddRange(fis);    
                }

            }

            return doors;
        }

        public static List<FamilyInstance> GetWindows(UIApplication uiApp)
        {
            // var doc = uiApp.ActiveUIDocument.Document;
            var windows = new List<FamilyInstance>();
            foreach (Document doc in uiApp.Application.Documents)
            {

                var fis = doc.GetElements<FamilyInstance>((e) =>
                {
                    if (e.LevelId == null)
                        return false;

#if REVIT2022
                    return (BuiltInCategory)e.Category.Id.IntegerValue == BuiltInCategory.OST_Windows;
#else
                    return e.Category.BuiltInCategory == BuiltInCategory.OST_Windows;
#endif
                });


                if (fis.Count() > 0)
                {
                    windows.AddRange(fis);
                }

            }

            return windows;
        }
    }
}
