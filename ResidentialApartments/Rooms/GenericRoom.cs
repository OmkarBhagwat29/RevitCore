using Autodesk.Revit.DB.Architecture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitCore.ResidentialApartments.Rooms
{
    public class GenericRoom(Room room) : RoomBase
    {
        public override Room Room { get; } = room;
    }
}
