namespace ContactQueueService.Configuration
{
    public class RabbitMQSettings
    {
        public string ConnectionString { get; set; }
        public QueueSettings Queues { get; set; } = new QueueSettings();
        
        public class QueueSettings
        {
            public string ContactCreate { get; set; } = "contact.create";
            public string ContactUpdate { get; set; } = "contact.update";
            public string ContactDelete { get; set; } = "contact.delete";
        }

        public string GetConnectionString()
        {
            return ConnectionString;
        }
    }
}