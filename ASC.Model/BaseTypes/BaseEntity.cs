namespace ASC.Model.BaseTypes
{
    public class BaseEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public BaseEntity()
        {

        }
    }
}
