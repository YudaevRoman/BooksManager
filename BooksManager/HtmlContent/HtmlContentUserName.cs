
using System;

namespace BooksManager
{
    public static partial class Program
    {
        public static async Task AddContent_UserName(this HtmlDocument html, HtmlTag parent, Person person)
        {
            List<BooksAddedByPerson> books;
            HtmlTag tagCurrent, tagBack, tagOutputArea, tagBody;

            tagCurrent = new HtmlTag();
            tagBack = new HtmlTag();

            parent.Tags.Add(tagCurrent);

            books = (List<BooksAddedByPerson>)(await person.GetBooks(dbManager));

            OutputArea_UserName(ref tagCurrent, ref tagBack, books.Count == 0);
            tagOutputArea = tagCurrent;
            foreach (var book in books)
            {
                tagBack = tagOutputArea;

                RecordBody_UserName(ref tagCurrent, ref tagBack);

                tagBody = tagCurrent;

                RecordImage_UserName(ref tagCurrent, ref tagBack, dbManager.Book.Find(book.BooksAddedByPerson_Book));

                tagBack = tagBody;

                (tagCurrent, tagBack) = await RecordInfo_UserName(tagCurrent, tagBack, dbManager.Book.Find(book.BooksAddedByPerson_Book));

                tagBack = tagBody;

                RecordAction_UserName(ref tagCurrent, ref tagBack, dbManager.Book.Find(book.BooksAddedByPerson_Book));
            }
        }

        private static void OutputArea_UserName(ref HtmlTag tagCurrent, ref HtmlTag tagBack, bool checkBooks)
        {
            tagCurrent.Header = "tr";
            tagCurrent.Value = "";
            tagCurrent.Attributes = new Dictionary<string, string>() { { "class", "HomeContextUserBook SeparatorLineBorder" } };
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

        private static void RecordBody_UserName(ref HtmlTag tagCurrent, ref HtmlTag tagBack)
        {
            tagCurrent.Header = "tr";
            tagCurrent.Value = "";
            tagCurrent.Attributes = new Dictionary<string, string>();
            tagCurrent.Tags = new List<HtmlTag>();
            tagCurrent.TagEnd = true;
            tagCurrent.DocType = false;

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>()
                    {
                        { "align", "center"}, {"class", "Parent HomeContentBookImage"}
                    }, new List<HtmlTag>(), true, false);
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

        private static void RecordImage_UserName(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Book book)
        {
            string path;

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("td", "", new Dictionary<string, string>()
                    {
                        { "align", "center"}, {"class", "Parent HomeContentBookImage"}
                    }, new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);


            if (book.ImageFile == null)
            {
                path = @"..\image\default.jpg";
            }
            else
            {
                path = book.ImageFile.Image_Path;
            }

            tagBack = tagCurrent;
            tagCurrent = new HtmlTag("img", "", new Dictionary<string, string>() { { "src", path }, { "class", "HomeBookImage" } },
                new List<HtmlTag>(), true, false);
            tagBack.Tags.Add(tagCurrent);
        }

        private static async Task<(HtmlTag, HtmlTag)> RecordInfo_UserName(HtmlTag tagCurrent, HtmlTag tagBack, Book book)
        {
            List<BooksCategory> categories;

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
            tagBack.Tags[0].Tags.Add(tagCurrent);

            tagCurrent = new HtmlTag("label", "Категории", new Dictionary<string, string>()
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
            categories = (List<BooksCategory>)(await book.GetCategories(dbManager));
            foreach (var category in categories)
            {
                tagCurrent = new HtmlTag("label", dbManager.Category.Find(category.BooksCategory_Category).Category_Name,
                    new Dictionary<string, string>()
                        {
                                {"class", "ContentLabel ContentLabelFontsizeText ContentLabelMarginDefault HomeBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);
            }

            return (tagCurrent, tagBack);
        }

        private static void RecordAction_UserName(ref HtmlTag tagCurrent, ref HtmlTag tagBack, Book book)
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
                    { "onclick", $"location.href=\'/books/{book.Book_Id}\'"}, {"value", "Подробнее"}
                }, new List<HtmlTag>(), false, false);
            tagBack.Tags.Add(tagCurrent);
        }
    }
}