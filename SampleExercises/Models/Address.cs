namespace SimpleDataManagement.Models
{
    public class Address
    {
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
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
