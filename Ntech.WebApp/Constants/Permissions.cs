using System.Collections.Generic;

namespace Ntech.WebApp.Constants
{
    public static class Permissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.Create",
                $"Permissions.{module}.View",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
            };
        }

        public static class Asset
        {
            public const string View = "Permissions.Asset.View";
            public const string Create = "Permissions.Asset.Create";
            public const string Edit = "Permissions.Asset.Edit";
            public const string Delete = "Permissions.Asset.Delete";
        }
    }
}