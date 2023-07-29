﻿namespace MangaHomeService.Models
{
    public class Theme : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Title> Titles { get; set; }

        public Theme() { }
        public Theme(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }
}
