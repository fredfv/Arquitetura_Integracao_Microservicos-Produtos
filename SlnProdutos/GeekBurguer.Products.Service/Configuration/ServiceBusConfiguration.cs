namespace GeekBurguer.Products.Service.Configuration
{
    public class ServiceBusConfiguration
    {
        public ServiceBusConfiguration()
        {
            
        }

        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string TenantId { get; set; }
        public string SubscriptionId { get; set; }
        public string ResourceGroup { get; set; }
        public string NamespaceName { get; set; }
        public string ConnectionString { get; set; }
    }
}