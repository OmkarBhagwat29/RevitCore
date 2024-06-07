

using Autodesk.Revit.DB.Architecture;

namespace RevitCore.ResidentialApartments.Rooms
{
    public enum BedroomType
    {
        SingleBed,
        DoubleBed
    }
    public class Bedroom(Room room ) : RoomBase
    {
        public override Room Room { get; } = room;

        public BedroomType BedroomType { get; private set; }

        public void SetBedRoomType(BedroomType type) => this.BedroomType = type;
    }
}
