using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("ImageFile")]
    public partial class ImageFile
    {
        public ImageFile()
        {
            Author = new List<Author>();
            Book = new List<Book>();
            Person = new List<Person>();
        }

        [Key]
        public int? Image_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Image_Path { get; set; }

        public virtual ICollection<Author> Author { get; set; }

        public virtual ICollection<Book> Book { get; set; }

        public virtual ICollection<Person> Person { get; set; }
    }
}
