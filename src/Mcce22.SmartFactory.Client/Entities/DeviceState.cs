using Amazon.DynamoDBv2.DataModel;

namespace Mcce22.SmartFactory.Client.Entities
{
    [DynamoDBTable("mcce22-smart-factory")]
    public class DeviceState
    {
        [DynamoDBHashKey()]
        public string Topic { get; set; }

        [DynamoDBProperty]
        public bool S3 { get; set; }

        [DynamoDBProperty]
        public bool S22 { get; set; }

        [DynamoDBProperty]
        public bool B3 { get; set; }

        [DynamoDBProperty]
        public bool B4 { get; set; } = true;

        [DynamoDBProperty]
        public bool B5 { get; set; }

        [DynamoDBProperty]
        public bool Q3 { get; set; }

        [DynamoDBProperty]
        public bool Q4 { get; set; }
    }
}
