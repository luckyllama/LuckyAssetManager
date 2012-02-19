using System;

namespace Lucky.AssetManager {
    public static class IE {

        public enum Equality {
            EqualTo,
            LessThan,
            LessThanOrEqualTo,
            GreaterThan,
            GreaterThanOrEqualTo
        }

        public enum Version {
            All,
            IE,
            IE5,
            IE50,
            IE55,
            IE6,
            IE7,
            IE8,
            IE9,
            IE10
        }

        public static string AsString(this Version version) {
            switch (version) {
                case Version.All:
                    return "";
                case Version.IE:
                    return "IE";
                case Version.IE5:
                    return "IE 5";
                case Version.IE50:
                    return "IE 5.0";
                case Version.IE55:
                    return "IE 5.5";
                case Version.IE6:
                    return "IE 6";
                case Version.IE7:
                    return "IE 7";
                case Version.IE8:
                    return "IE 8";
                case Version.IE9:
                    return "IE 9";
                case Version.IE10:
                    return "IE 10";
                default:
                    throw new ArgumentOutOfRangeException("version");
            }
        }

        public static string AsString(this Equality equality) {
            switch (equality) {
                case Equality.EqualTo:
                    return "";
                case Equality.LessThan:
                    return "lt";
                case Equality.LessThanOrEqualTo:
                    return "lte";
                case Equality.GreaterThan:
                    return "gt";
                case Equality.GreaterThanOrEqualTo:
                    return "gte";
                default:
                    throw new ArgumentOutOfRangeException("equality");
            }
        }
    }
}