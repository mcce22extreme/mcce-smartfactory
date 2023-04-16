using Amazon.DynamoDBv2.DataModel;

namespace Mcce22.SmartFactory.Controller.Entities
{
    [DynamoDBTable("mcce22-smart-factory")]
    public class DeviceState
    {
        [DynamoDBHashKey()]
        public string Topic { get; set; }

        [DynamoDBProperty]
        public bool S1 { get; set; }

        [DynamoDBProperty]
        public bool S21 { get; set; }

        [DynamoDBProperty]
        public bool S2 { get; set; }

        [DynamoDBProperty]
        public bool S3 { get; set; }

        [DynamoDBProperty]
        public bool S22 { get; set; }

        [DynamoDBProperty]
        public bool B1 { get; set; }

        [DynamoDBProperty]
        public bool B2 { get; set; }

        [DynamoDBProperty]
        public bool B3 { get; set; }

        [DynamoDBProperty]
        public bool B4 { get; set; } = true;

        [DynamoDBProperty]
        public bool B5 { get; set; }

        [DynamoDBProperty]
        public bool Q1 { get; set; }

        [DynamoDBProperty]
        public bool Q2 { get; set; }

        [DynamoDBProperty]
        public bool Q3 { get; set; }

        [DynamoDBProperty]
        public bool Q4 { get; set; }

        [DynamoDBProperty]
        public bool F1 { get; set; }
    }
}
