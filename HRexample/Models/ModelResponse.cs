namespace HRexample.Models
{
    public class ModelResponse
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public List<string> skills { get; set; } = new List<string>();
        public string Education { get; set; } = string.Empty;
        public string Experience { get; set; } = string.Empty;
        public int PercentageFitForJob { get; set; } = 0;
    }
}
