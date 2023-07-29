namespace MangaHomeService.Models
{
    public class Volume : BaseModel
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public Title Title { get; set; }

        public Volume() { }
        public Volume(string number, string name, Title title)
        {
            Number = number;
            Name = name;
            Title = title;
        }
    }
}
