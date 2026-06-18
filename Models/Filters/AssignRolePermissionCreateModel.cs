namespace EBookStore.Models.Filters
{
    public class AssignRolePermissionCreateModel
    {
        public UserType? Role { get; set; }

        public IEnumerable<ActionRoute>? ActionRoutes { get; set; }
        public IEnumerable<RolePermission>? RolePermissions { get; set; }
    }
}
