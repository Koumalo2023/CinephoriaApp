namespace CinephoriaServer.Configurations
{
    public static class RoleConfigurations
    {
        public const string GlobalAdmin = "GlobalAdmin";

        public const string CineAdmin = "CineAdmin";

        public const string Employee = "Employee";

        public const string User = "User";


        public const string GlobalAdminCineAdmin = "GlobalAdmin,CineAdmin";
        public const string GlobalAdminCineAdminEmployee = "GlobalAdmin,CineAdmin, Employee";
        public const string GlobalAdminCineAdminEmployeeUser = "GlobalAdmin,CineAdmin, Employee,User";
    }
}
