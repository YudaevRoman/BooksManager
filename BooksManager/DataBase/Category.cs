using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("Category")]
    public partial class Category
    {
        public Category()
        {
            BooksCategory = new List<BooksCategory>();
        }

        [Key]
        public int? Category_Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Category_Name { get; set; }

        public virtual ICollection<BooksCategory> BooksCategory { get; set; }

        public async Task<ICollection<BooksCategory>> GetBooks(DbBooksManager dbManager)
        {
            BooksCategory = new List<BooksCategory>();
            await dbManager.BooksCategory.ForEachAsync((book) =>
            {
                if (book.BooksCategory_Category == Category_Id)
                {
                    BooksCategory.Add(book);
                }
            });


            BooksCategory = BooksCategory.Distinct().ToList();
            return BooksCategory;
        }
    }
}
