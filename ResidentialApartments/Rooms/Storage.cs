
using Autodesk.Revit.DB.Architecture;

namespace RevitCore.ResidentialApartments.Rooms
{
    public class Storage(Room room) : RoomBase
    {
        public override Room Room { get; } = room;
    }
}
