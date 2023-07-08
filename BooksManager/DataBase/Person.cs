using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BooksManager
{

    [Table("Person")]
    public partial class Person
    {
        public Person()
        {
            BooksAddedByPerson = new List<BooksAddedByPerson>();
        }

        [Key]
        public int? Person_Id { get; set; }

        public int Person_Role { get; set; }

        [Required]
        [StringLength(20)]
        public string Person_Name { get; set; }

        [StringLength(20)]
        public string Person_Password { get; set; }

        public int? Person_Image { get; set; }

        public virtual ICollection<BooksAddedByPerson> BooksAddedByPerson { get; set; }

        public virtual ImageFile ImageFile { get; set; }

        public virtual PersonRole PersonRole { get; set; }

        public async Task<ImageFile> GetImage(DbBooksManager dbManager)
        {
            ImageFile = await dbManager.ImageFile.FindAsync(Person_Image);
            return ImageFile;
        }

        public async Task<PersonRole> GetRole(DbBooksManager dbManager)
        {
            PersonRole = await dbManager.PersonRole.FindAsync(Person_Role);
            return PersonRole;
        }

        public async Task<ICollection<BooksAddedByPerson>> GetBooks(DbBooksManager dbManager)
        {
            BooksAddedByPerson = new List<BooksAddedByPerson>();
            await dbManager.BooksAddedByPerson.ForEachAsync((book) =>
            {
                if (book.BooksAddedByPerson_Person == Person_Id)
                {
                    BooksAddedByPerson.Add(book);
                }
            });

            BooksAddedByPerson = BooksAddedByPerson.Distinct().ToList();
            return BooksAddedByPerson;
        }
    }
}
