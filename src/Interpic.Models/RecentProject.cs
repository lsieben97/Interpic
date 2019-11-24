namespace Interpic.Models
{
    public class RecentProject
    {
        public string Name { get; set;}
        public string Path { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}
