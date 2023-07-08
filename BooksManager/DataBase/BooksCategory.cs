using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("BooksCategory")]
    public partial class BooksCategory
    {
        [Key]
        public int? BooksCategory_Id { get; set; }

        public int BooksCategory_Book { get; set; }

        public int BooksCategory_Category { get; set; }

        public virtual Book Book { get; set; }

        public virtual Category Category { get; set; }
    }
}
