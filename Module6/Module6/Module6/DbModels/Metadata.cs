namespace Module6.DbModels
{
    public class Metadata
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public decimal FileSize { get; set; }
        public string FileUrl { get; set; }
        public string ContentType { get; set; }
    }
}