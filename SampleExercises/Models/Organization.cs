namespace SimpleDataManagement.Models
{
    public class Organization
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int YearStarted { get; set; }
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
