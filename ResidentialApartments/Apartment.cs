using RevitCore.Extensions;
using RevitCore.ResidentialApartments.Rooms;
using RevitCore.ResidentialApartments.Validation;


namespace RevitCore.ResidentialApartments
{

    public abstract class Apartment
    {

        public string Name { get; set; }


        public string Description { get; set; }


        public int Occupancy { get; set; }


        public abstract ApartmentType Type { get; }


        public Area AreaBoundary { get; set; }


        public List<RoomBase> Rooms { get; } = [];

        public  List<FamilyInstance> ApartmentElements { get; } = [];


        public List<ISpatialValidation> ApartmentValidationData { get; } = [];

        public virtual void Validate()
        {
            //apartment level validation
            this.ApartmentValidationData.ForEach(d => d.Validate());


            foreach (var rm in this.Rooms)
            {
                foreach (var vD in rm.RoomValidationData)
                {
                   vD.Validate();
                }
            }
        }

        public virtual void Bake(Document doc)
        {
                foreach (var apartment in ApartmentValidationData)
                {
                    apartment.Bake(doc);
                }

                foreach (var room in Rooms)
                {
                    foreach (var validationData in room.RoomValidationData)
                    {
                        validationData.Bake(doc);
                    }
                }
        }

        public virtual void AddValidationData(ISpatialValidation apartmentValidation) => this.ApartmentValidationData.Add(apartmentValidation);

        public virtual RoomBase AddRoom(RoomBase roomBase, Func<RoomBase, bool> validate = null)
        {
            validate ??= (r) => true;

            if (validate(roomBase))
            {
                this.Rooms.Add(roomBase);
                return this.Rooms.Last();
            }
            return null;
        }

        public virtual RoomBase FindRoom(Func<List<RoomBase>, RoomBase> find)
        {
            return find(this.Rooms);
        }


    }
}
