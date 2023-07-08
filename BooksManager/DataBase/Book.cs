using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("Book")]
    public partial class Book
    {
        public Book()
        {
            AuthorBooks = new List<AuthorBooks>();
            BooksAddedByPerson = new List<BooksAddedByPerson>();
            BooksCategory = new List<BooksCategory>();
        }

        [Key]
        public int Book_Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Book_Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Book_Date { get; set; }

        [StringLength(512)]
        public string Book_Description { get; set; }

        public int? Book_Image { get; set; }

        public virtual ICollection<AuthorBooks> AuthorBooks { get; set; }

        public virtual ImageFile ImageFile { get; set; }

        public virtual ICollection<BooksAddedByPerson> BooksAddedByPerson { get; set; }

        public virtual ICollection<BooksCategory> BooksCategory { get; set; }

        public async Task<ImageFile> GetImage(DbBooksManager dbManager)
        {
            ImageFile = await dbManager.ImageFile.FindAsync(Book_Image);

            return ImageFile;
        }

        public async Task<ICollection<AuthorBooks>> GetAuthors(DbBooksManager dbManager)
        {
            AuthorBooks = new List<AuthorBooks>();
            await dbManager.AuthorBooks.ForEachAsync((book) =>
            {
                if (book.AuthotBooks_Author == Book_Id)
                {
                    AuthorBooks.Add(book);
                }
            });

            AuthorBooks = AuthorBooks.Distinct().ToList();
            return AuthorBooks;
        }

        public async Task<ICollection<BooksAddedByPerson>> GetPersons(DbBooksManager dbManager)
        {
            BooksAddedByPerson = new List<BooksAddedByPerson>();
            await dbManager.BooksAddedByPerson.ForEachAsync((book) =>
            {
                if (book.BooksAddedByPerson_Book == Book_Id)
                {
                    BooksAddedByPerson.Add(book);
                }
            });

            BooksAddedByPerson = BooksAddedByPerson.Distinct().ToList();
            return BooksAddedByPerson;
        }

        public async Task<ICollection<BooksCategory>> GetCategories(DbBooksManager dbManager)
        {
            BooksCategory = new List<BooksCategory>();
            await dbManager.BooksCategory.ForEachAsync((book) =>
            {
                if (book.BooksCategory_Book == Book_Id)
                {
                    BooksCategory.Add(book);
                }
            });

            BooksCategory = BooksCategory.Distinct().ToList();
            return BooksCategory;
        }

    }
}
