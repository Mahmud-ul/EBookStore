namespace EBookStore.Models.Filters
{
    public class ActionRoute
    {
        public int ID { get; set; }
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public bool Status { get; set; } //Active, Inactive
    }
}
