using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("BooksAddedByPerson")]
    public partial class BooksAddedByPerson
    {
        [Key]
        public int? BooksAddedByPerson_Id { get; set; }

        public int BooksAddedByPerson_Person { get; set; }

        public int BooksAddedByPerson_Book { get; set; }

        public virtual Book Book { get; set; }

        public virtual Person Person { get; set; }
    }
}
