
using System;
using System.Net;

namespace BooksManager
{
    public static partial class Program
    {
        public static async Task AddContent_Book_Categories(this HtmlDocument html, HtmlTag parent, Book book)
        {
            HtmlTag tagCurrent, tagBack;
            tagBack = parent.Tags[0];

            List<BooksCategory> categories = (List<BooksCategory>)(await book.GetCategories(dbManager));
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

        public static async Task AddContent_Book_Books(this HtmlDocument html, HtmlTag parent, Book book)
        {
            HtmlTag tagCurrent, tagBack;
            tagBack = parent.Tags[0];

            int i = 1;
            List<AuthorBooks> authors = (List<AuthorBooks>)(await book.GetAuthors(dbManager));
            foreach (var author in authors)
            {
                tagCurrent = new HtmlTag("div", "",
                    new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("label", (await dbManager.Author.FindAsync(author.AuthotBooks_Author)).Author_LastName,
                    new Dictionary<string, string>()
                        {
                                {"class", "ContentLabel ContentLabelFontsizeText AddedBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("input", "", new Dictionary<string, string>()
                    {
                        {"type", "button"}, {"class", "ContentButton ContentButtonFontsizeText HomeBookAction" },
                        { "onclick", $"location.href='/authors/{author.AuthotBooks_Author}'"}, { "value", "Подробнее"}
                    }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                i++;
            }
        }
    }
}