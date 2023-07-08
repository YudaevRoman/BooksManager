using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{
    public partial class AuthorBooks
    {
        [Key]
        public int? AuthorBooks_Id { get; set; }

        public int AuthotBooks_Author { get; set; }

        public int AuthorBooks_Book { get; set; }

        public virtual Author Author { get; set; }

        public virtual Book Book { get; set; }
    }
}
