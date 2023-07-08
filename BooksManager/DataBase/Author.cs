using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("Author")]
    public partial class Author
    {
        public Author()
        {
            AuthorBooks = new List<AuthorBooks>();
        }

        [Key]
        public int? Author_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Author_LastName { get; set; }

        [StringLength(20)]
        public string Author_FirstName { get; set; }

        public int? Author_Image { get; set; }

        public virtual ImageFile ImageFile { get; set; }

        public virtual ICollection<AuthorBooks> AuthorBooks { get; set; }

        public async Task<ImageFile> GetImage(DbBooksManager dbManager)
        {
            if (Author_Image != null)
            {
                ImageFile = await dbManager.ImageFile.FindAsync(Author_Image);
                return ImageFile;
            }
            return null;
        }

        public async Task<ICollection<AuthorBooks>> GetBooks(DbBooksManager dbManager)
        {
            AuthorBooks = new List<AuthorBooks>();
            await dbManager.AuthorBooks.ForEachAsync((book) =>
            {
                if (book.AuthotBooks_Author == Author_Id)
                {
                    AuthorBooks.Add(book);
                }
            });

            AuthorBooks = AuthorBooks.Distinct().ToList();
            return AuthorBooks;
        }

        public async Task<ICollection<BooksCategory>> GetCategories(DbBooksManager dbManager)
        {
            List<BooksCategory> authorCategories = new List<BooksCategory>();

            if (AuthorBooks == null) {
                GetBooks(dbManager);
            }
            foreach (var book in AuthorBooks)
            {
                authorCategories.AddRange(
                    await (await dbManager.Book.FindAsync(book.AuthorBooks_Book))
                    .GetCategories(dbManager)
                    );
            }
            authorCategories = authorCategories.Distinct().ToList();

            return authorCategories;
        }

    }
}
