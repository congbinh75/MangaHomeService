﻿namespace MangaHomeService.Models
{
    public class GroupPermission : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<GroupRole> Roles { get; set; }
    }
}
