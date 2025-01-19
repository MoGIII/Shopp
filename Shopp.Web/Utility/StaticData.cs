namespace Shopp.Web.Utility
{
    public class StaticData
    {
        public const string TokenCookie = "JWTToken";
        public static string CouponAPIBase { get; set; }
        public static string AuthAPIBase { get; set; }
        public static string ProductAPIBase { get; set; }
        public static string CartAPIBase { get; internal set; }
        public static string OrderAPIBase { get; internal set; }

        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Completed = "Completed";
        public const string Status_Refunded = "Refunded";
        public const string Status_Cancelled = "Cancelled";

        public enum Roles
        {
            Admin,
            Customer
        }
        public enum ApiType
        {
            GET, 
            POST, 
            PUT, 
            DELETE 
        }

        public enum ContentType
        {
            Json,
            MultipartFormData
        }
    }
}
