using System.ComponentModel.DataAnnotations.Schema;

namespace EBookStore.Models.Filters
{
    public class RolePermission
    {
        public int ID { get; set; }
        public int RoleID { get; set; }

        [ForeignKey(nameof(RoleID))]
        public UserType? UserType { get; set; }

        public int ActionRouteID { get; set; }

        [ForeignKey(nameof(ActionRouteID))]
        public ActionRoute? ActionRoute { get; set; }
    }
}
