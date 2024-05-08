

using Autodesk.Revit.DB.Architecture;

namespace RevitCore.ResidentialApartments.Rooms
{
    public class Bedroom(Room room) : RoomBase
    {
        public override Room Room { get; } = room;
    }
}
