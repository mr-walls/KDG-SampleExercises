namespace SimpleDataManagement.Models
{
    public class Vehicle
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public int Year { get; set; }
        public string VehicleType { get; set; }
        public string PlateNumber { get; set; }
        public string State { get; set; }
        public string Vin { get; set; }
        public string EntityId { get; set; }

        IList<Association>? _entities;
        public IList<Association> Associations
        {
            get
            {
                if (_entities == null)
                    _entities = new List<Association>();

                return _entities;
            }
            set
            {
                _entities = value;
            }
        }
    }
}
