namespace TeamMerge.Services.Models
{
    public class SolutionModel
    {
        public SolutionModel(string fullName)
        {
            FullName = fullName;
        }

        public string FullName { get; set; }
    }
}
