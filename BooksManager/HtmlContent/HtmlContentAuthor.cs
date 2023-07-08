
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace BooksManager
{
    public static partial class Program
    {
        public static async Task AddContent_Author_Categories(this HtmlDocument html, HtmlTag parent, Author author)
        {
            HtmlTag tagCurrent, tagBack;
            tagBack = parent.Tags[0];

            List<BooksCategory> categories = (List<BooksCategory>)(await author.GetCategories(dbManager));
            foreach (var category in categories)
            {
                tagCurrent = new HtmlTag("div", "",
                    new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("label", (await dbManager.Category.FindAsync(category.BooksCategory_Category)).Category_Name,
                    new Dictionary<string, string>()
                        {
                                {"class", "ContentLabel ContentLabelFontsizeText AddedBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);
            }
        }

        public static async Task AddContent_Author_Books(this HtmlDocument html, HtmlTag parent, Author author)
        {
            HtmlTag tagCurrent, tagBack;
            tagBack = parent.Tags[0];

            int i = 1;
            List<AuthorBooks> books = (List<AuthorBooks>)(await author.GetBooks(dbManager));
            foreach (var book in books)
            {
                tagCurrent = new HtmlTag("div", "",
                    new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("label", (await dbManager.Book.FindAsync(book.AuthorBooks_Book)).Book_Name,
                    new Dictionary<string, string>()
                        {
                                {"class", "ContentLabel ContentLabelFontsizeText AddedBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("input", "", new Dictionary<string, string>()
                    {
                        {"type", "button"}, {"class", "ContentButton ContentButtonFontsizeText HomeBookAction" },
                        { "onclick", $"location.href='/books/{book.AuthorBooks_Book}'"}, { "value", "Подробнее"}
                    }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                i++;
            }
        }
      }
}