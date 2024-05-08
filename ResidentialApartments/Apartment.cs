using RevitCore.Extensions;
using RevitCore.ResidentialApartments.Rooms;
using RevitCore.ResidentialApartments.Validation;


namespace RevitCore.ResidentialApartments
{
    public abstract class Apartment
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public abstract ApartmentType Type { get; }

        public Area AreaBoundary { get; set; }
        public List<RoomBase> Rooms { get; } = [];

        public List<ISpatialValidation> ApartmentValidationData { get; } = [];

        public abstract void Validate();

        public virtual List<Element> Bake(Document doc)
        {
            var bakedElms = doc.UseTransaction(() =>
            {

                var elements = new List<Element>();
                foreach (var apartment in ApartmentValidationData)
                {
                    var elms = apartment.Bake(doc).ToList();
                    elements.AddRange(elms);
                }

                foreach (var room in Rooms)
                {
                    foreach (var validationData in room.RoomValidationData)
                    {
                        var elms = validationData.Bake(doc).ToList();
                        elements.AddRange(elms);
                    }
                }

                return elements;
            }, "Apartment Validation Data Baked");

            return bakedElms;
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
