using Core.Interfaces;
namespace Core.Models.Plumberlab.DbItem
{
    public class TemplateModel : IDbItem
    {
        public int Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Modified { get; set; }
        public DateTime? Deleted { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }

        public TemplateModel(string name)
        {
            Name = name;
        }
    }
}