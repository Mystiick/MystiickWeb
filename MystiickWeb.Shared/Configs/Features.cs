using System.Reflection;

namespace MystiickWeb.Shared.Configs
{
    public class Features
    {
        public static string FeaturesKey = "Features";
        public bool UserRegistration { get; set; }

        public bool IsFeatureEnabled(string feature)
        {
            PropertyInfo? prop = typeof(Features).GetProperty(feature);

            if (prop != null)
                return (bool)(prop.GetValue(this) ?? false);

            return false;
        }
    }
}
