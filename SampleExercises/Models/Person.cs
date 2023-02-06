namespace SimpleDataManagement.Models
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public DateTime? DateOfBirth { get; set; }
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
