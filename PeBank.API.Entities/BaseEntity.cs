using System.ComponentModel.DataAnnotations;
namespace PeBank.API.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
