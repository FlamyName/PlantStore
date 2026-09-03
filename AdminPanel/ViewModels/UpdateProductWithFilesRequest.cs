namespace AdminPanel.ViewModels
{
    public class UpdateProductWithFilesRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Price { get; set; }
        public int CategoryId { get; set; }
        public int UnitId { get; set; }
        public int Count { get; set; }

        public List<string> CurrentImages { get; set; }
        public List<string> TempImages { get; set; }
    }
}
