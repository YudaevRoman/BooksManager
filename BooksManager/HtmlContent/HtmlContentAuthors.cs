
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace BooksManager
{
    public static partial class Program
    {
        public static async Task AddContent_Authors(this HtmlDocument html, HtmlTag parent)
        {
            HtmlTag tagCurrent, tagBack, tagOutputArea, tagBody;

            tagCurrent = new HtmlTag();
            tagBack = new HtmlTag();

            parent.Tags.Add(tagCurrent);

            OutputArea_Authors(ref tagCurrent, ref tagBack, (await dbManager.Author.CountAsync()) == 0);
            tagOutputArea = tagCurrent;

            foreach (var author in dbManager.Author)
            {
                tagBack = tagOutputArea;

                RecordBody_Authors(ref tagCurrent, ref tagBack);

                tagBody = tagCurrent;

                RecordImage_Authors(ref tagCurrent, ref tagBack, author);

                tagBack = tagBody;

                (tagCurrent, tagBack) = await RecordInfo_Authors(tagCurrent, tagBack, author);

                tagBack = tagBody;

                RecordAction_Authors(ref tagCurrent, ref tagBack, author);
            }
        }

        private static void OutputArea_Authors(ref HtmlTag tagCurrent, ref HtmlTag tagBack, bool checkBooks)
        {
            tagCurrent.Header = "tr";
            tagCurrent.Value = "";
            tagCurrent.Attributes = new Dictionary<string, string>();
            tagCurrent.Tags = new List<HtmlTag>();
            tagCurrent.TagEnd = true;
            tagCurrent.DocType = false;

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("fieldset", "", new Dictionary<string, string>()
            {
                {"class", "Parent ContentRegion"}
            }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            if (checkBooks)
            {
                tagCurrent = new HtmlTag("label", "Нет добавленных авторов", new Dictionary<string, string>()
                {
                    {"class", "ContentLabel ContentLabelFontsizeSubhead ContentLabelMarginDefault"}
                }, new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);
                return;
            }
            else
            {
                tagCurrent = new HtmlTag("table", "", new Dictionary<string, string>() { { "class", "Parent" } },
                    new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);
            }
        }

        private static void RecordBody_Authors(ref HtmlTag tagCurrent, ref HtmlTag tagBack)
        {
            tagCurrent = new HtmlTag("tr", "", new Dictionary<string, string>() 
            { 
                { "class", "HomeContextUserBook SeparatorLineBorder" } 
            }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("table", "", new Dictionary<string, string>() { { "class", "Parent" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagCurrent = new HtmlTag("hr", "", new Dictionary<string, string>() { { "class", "SeparatorLineBorder" } },
                new List<HtmlTag>(), false, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagBack.Tags[0];
            tagCurrent = new HtmlTag("tr", "", new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);
        }

        private static void RecordImage_Authors(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Author author)
        {
            string path;

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>()
                    {
                        { "align", "center"}, {"class", "Parent HomeContentBookImage"}
                    }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);


            if (author.ImageFile == null) {
                path = @"..\image\default.jpg";
            } else {
                path = author.ImageFile.Image_Path;
            }

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("img", "", new Dictionary<string, string>() { { "src", path }, { "class", "HomeBookImage" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);
            }

        private static async Task<(HtmlTag, HtmlTag)> RecordInfo_Authors(HtmlTag tagCurrent, HtmlTag tagBack, Author author)
        {
            RecordInfoArea_Authors(ref tagCurrent, ref tagBack, author);
            (tagCurrent, tagBack)  = await RecordInfoCategories_Authors(tagCurrent, tagBack, author);
            (tagCurrent, tagBack)  = await RecordInfoBooks_Authors(tagCurrent, tagBack, author);
            return (tagCurrent, tagBack);
        }

        private static void RecordInfoArea_Authors(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Author author)
        {
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>() { { "class", "HomeBookInfo" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("label", author.Author_LastName, new Dictionary<string, string>()
                {
                    {"class", "ContentLabel ContentLabelFontsizeSubhead ContentLabelMarginDefault"}
                }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagCurrent = new HtmlTag("br", "", new Dictionary<string, string>(), new List<HtmlTag>(), false, false);
            tagBack.Tags.Add(tagCurrent);

            tagCurrent = new HtmlTag("table", "", new Dictionary<string, string>() { { "class", "Parent" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("tr", "", new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>() { { "align", "left" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);
        }

        private static async Task<(HtmlTag, HtmlTag)> RecordInfoCategories_Authors(HtmlTag tagCurrent, HtmlTag tagBack, Author author)
        {
            List<BooksCategory> categories;

            tagCurrent = new HtmlTag("label", "Категории", new Dictionary<string, string>()
                {
                    {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault"}
                }, new List<HtmlTag>(), true, false);
            tagBack.Tags[0].Tags.Add(tagCurrent);

            tagCurrent = new HtmlTag("fieldset", "", new Dictionary<string, string>()
                {
                    {"class", "Parent HomeBookCategorys ContentLabelMarginDefault ContentScroll"}
                }, new List<HtmlTag>(), true, false);
            tagBack.Tags[0].Tags.Add(tagCurrent);

            categories = (List<BooksCategory>)(await author.GetCategories(dbManager));

            foreach (var category in categories)
            {
                tagCurrent = new HtmlTag("label", (await dbManager.Category.FindAsync(category.BooksCategory_Category)).Category_Name,
                    new Dictionary<string, string>()
                        {
                            {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault HomeBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags[0].Tags[1].Tags.Add(tagCurrent);
            }

            return (tagCurrent, tagBack);
        }

        private static async Task<(HtmlTag, HtmlTag)> RecordInfoBooks_Authors(HtmlTag tagCurrent, HtmlTag tagBack, Author author)
        {
            List<AuthorBooks> books;

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>() { { "align", "left" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("label", "Книги", new Dictionary<string, string>()
                {
                    {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault"}
                }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagCurrent = new HtmlTag("fieldset", "", new Dictionary<string, string>()
                {
                    {"class", "Parent HomeBookCategorys ContentLabelMarginDefault ContentScroll"}
                }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            books = (List<AuthorBooks>)(await author.GetBooks(dbManager));
            if (books.Count > 0) {
                foreach (var book in books)
                {
                    tagCurrent = new HtmlTag("label", (await dbManager.Book.FindAsync(book.AuthorBooks_Book)).Book_Name,
                        new Dictionary<string, string>()
                            {
                            {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault HomeBookCategory"}
                            }, new List<HtmlTag>(), true, false);
                    tagBack.Tags.Add(tagCurrent);
                }
            } else {
                tagCurrent = new HtmlTag("label", "Книги не указаны",
                    new Dictionary<string, string>()
                        {
                            {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault HomeBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);
            }

            return (tagCurrent, tagBack);
        }

        private static void RecordAction_Authors(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Author author)
        {
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>(),
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("fieldset", "", new Dictionary<string, string>() { { "class", "Parent" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("input", "", new Dictionary<string, string>()
                {
                    { "type", "button" }, { "class", "ContentButton ContentButtonFontsizeText HomeBookAction"},
                    { "onclick", $"location.href='/books/{author.Author_Id}'"}, {"value", "Подробнее"}
                }, new List<HtmlTag>(), false, false);
            tagBack.Tags.Add(tagCurrent);
        }
    }
}