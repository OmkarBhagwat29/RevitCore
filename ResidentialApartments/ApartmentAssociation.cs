using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using RevitCore.Extensions;
using RevitCore.Extensions.PointInPoly;
using RevitCore.Extensions.Selection;
using RevitCore.GeometryUtils;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;



namespace RevitCore.ResidentialApartments
{
    public class ApartmentAssociation
    {
        //public static Document Doc;

        public ApartmentAssociation(Area areaBoundary, List<Room> rooms)
        {
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
            var areas = areaRoomElements.Where(e => e.GetType() == typeof(Area))
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
            Func<Area, bool> AreaPassCondition)
        {
            //get all the spatial elements
            var doc = uiApp.ActiveUIDocument.Document;
            var elements = new List<SpatialElement>();//doc.GetElements<SpatialElement>().ToList();

            foreach (Document linkedDoc in uiApp.Application.Documents)
            {

                var spatialElements = linkedDoc.GetElements<SpatialElement>().ToList();

                if (spatialElements.Count > 0)
                {
                    elements.AddRange(spatialElements);
                }

            }

            //get only areas
            var areas = elements.Where(e => e is Area).Cast<Area>()
                .Where(AreaPassCondition)
                .OrderBy(e => e.Level.Elevation)
                .ToList();

            //get only rooms
            var rooms = elements.Where(e => e is Room)
                .Cast<Room>()
                .Where(r => r.Location != null)
                .OrderBy(r => r.Level.Elevation)
                .ToList();

            //get all rooms for each areas and create data
            List<ApartmentAssociation> assList = [];

            foreach (var areaBoundary in areas)
            {
                List<Room> apartmentRooms = [];

                var areaSolid = areaBoundary.GetSolidFromAreaBoundary(5, new SpatialElementBoundaryOptions
                    ()
                {
                    SpatialElementBoundaryLocation = SpatialElementBoundaryLocation.Center
                });

                if (areaSolid == null)
                {
                    continue;
                }

                foreach (var room in rooms)
                {
                    var lp = room.Location as LocationPoint;

                    bool intersects = areaSolid.IntersectsWithElement(lp.Point,0.1);

                    if (intersects)
                    {
                        apartmentRooms.Add(room);
                    }
                }

                if (apartmentRooms.Count > 0)
                {
                    assList.Add(new ApartmentAssociation(areaBoundary, apartmentRooms));

                    apartmentRooms.ForEach(r => { 
                    
                        var index =  rooms.IndexOf(r);

                        if (index != -1)
                        {
                            rooms.RemoveAt(index);
                        }

                    });
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
