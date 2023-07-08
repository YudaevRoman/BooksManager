
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace BooksManager
{
    public static partial class Program
    {
        public static async Task AddContent_Books(this HtmlDocument html, HtmlTag parent)
        {
            HtmlTag tagCurrent, tagBack, tagOutputArea, tagBody;

            tagCurrent = new HtmlTag();
            tagBack = new HtmlTag();

            parent.Tags.Add(tagCurrent);

            OutputArea_Books(ref tagCurrent, ref tagBack, (await dbManager.Author.CountAsync()) == 0);
            tagOutputArea = tagCurrent;

            foreach (var book in dbManager.Book)
            {
                tagBack = tagOutputArea;

                RecordBody_Books(ref tagCurrent, ref tagBack);

                tagBody = tagCurrent;

                RecordImage_Books(ref tagCurrent, ref tagBack, book);

                tagBack = tagBody;

                (tagCurrent, tagBack) = await RecordInfo_Books(tagCurrent, tagBack, book);

                tagBack = tagBody;

                RecordAction_Books(ref tagCurrent, ref tagBack, book);
            }
        }

        private static void OutputArea_Books(ref HtmlTag tagCurrent, ref HtmlTag tagBack, bool checkBooks)
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
                tagCurrent = new HtmlTag("label", "Нет добавленных книг", new Dictionary<string, string>()
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

        private static void RecordBody_Books(ref HtmlTag tagCurrent, ref HtmlTag tagBack)
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

        private static void RecordImage_Books(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Book book)
        {
            string path;

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>()
                    {
                        { "align", "center"}, {"class", "Parent HomeContentBookImage"}
                    }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);


            if (book.ImageFile == null) {
                path = @"..\image\default.jpg";
            } else {
                path = book.ImageFile.Image_Path;
            }

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("img", "", new Dictionary<string, string>() { { "src", path }, { "class", "HomeBookImage" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);
            }

        private static async Task<(HtmlTag, HtmlTag)> RecordInfo_Books(HtmlTag tagCurrent, HtmlTag tagBack, Book book)
        {
            RecordInfoArea_Books(ref tagCurrent, ref tagBack, book);
            (tagCurrent, tagBack)  = await RecordInfoCategories_Books(tagCurrent, tagBack, book);
            (tagCurrent, tagBack)  = await RecordInfoAuthors_Books(tagCurrent, tagBack, book);
            return (tagCurrent, tagBack);
        }

        private static void RecordInfoArea_Books(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Book book)
        {
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>() { { "class", "HomeBookInfo" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("label", book.Book_Name, new Dictionary<string, string>()
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

        private static async Task<(HtmlTag, HtmlTag)> RecordInfoCategories_Books(HtmlTag tagCurrent, HtmlTag tagBack, Book book)
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

            categories = (List<BooksCategory>)(await book.GetCategories(dbManager));

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

        private static async Task<(HtmlTag, HtmlTag)> RecordInfoAuthors_Books(HtmlTag tagCurrent, HtmlTag tagBack, Book book)
        {
            List<AuthorBooks> authors;

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>() { { "align", "left" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("label", "Авторы", new Dictionary<string, string>()
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
            authors = (List<AuthorBooks>)(await book.GetAuthors(dbManager));
            if (authors.Count > 0)
            {
                foreach (var author in authors)
                {
                    tagCurrent = new HtmlTag("label", (await dbManager.Author.FindAsync(author.AuthotBooks_Author)).Author_LastName,
                        new Dictionary<string, string>()
                            {
                            {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault HomeBookCategory"}
                            }, new List<HtmlTag>(), true, false);
                    tagBack.Tags.Add(tagCurrent);
                }
            } else {
                tagCurrent = new HtmlTag("label", "Авторы не указаны",
                    new Dictionary<string, string>()
                        {
                            {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault HomeBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);
            }

            return (tagCurrent, tagBack);
        }

        private static void RecordAction_Books(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Book book)
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
                    { "onclick", $"location.href='/books/{book.Book_Id}'"}, {"value", "Подробнее"}
                }, new List<HtmlTag>(), false, false);
            tagBack.Tags.Add(tagCurrent);
        }
    }
}