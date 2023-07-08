using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("PersonRole")]
    public partial class PersonRole
    {
        public PersonRole()
        {
            Person = new List<Person>();
        }

        [Key]
        public int? Role_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Role_Name { get; set; }

        public virtual ICollection<Person> Person { get; set; }
    }
}
