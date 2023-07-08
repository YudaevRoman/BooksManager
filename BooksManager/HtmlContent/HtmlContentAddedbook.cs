
using Microsoft.EntityFrameworkCore;
using System;
using System.Net;

namespace BooksManager
{
    public static partial class Program
    {
        public static async Task AddContent_Addedbook_Categories(this HtmlDocument html, HtmlTag parent)
        {
            HtmlTag tagCurrent, tagBack;

            tagBack = parent.Tags[0];

            int i = 1;

            await dbManager.Category.ForEachAsync((category) =>
            {
                tagCurrent = new HtmlTag("div", "",
                    new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("label", category.Category_Name,
                    new Dictionary<string, string>()
                        {
                                {"class", "ContentLabel ContentLabelFontsizeText AddedBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("input", "", new Dictionary<string, string>()
                    {
                        {"type", "checkbox"}, {"class", "CheckBoxContent" }, { "id", $"category{category.Category_Id}" },
                        {"onclick",
                        $"const a = document.getElementById('_category{category.Category_Id}'); " +
                        $"const b = document.getElementById('category{category.Category_Id}'); " +
                        $"a.value = b.checked;" }
                    }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("input", "", new Dictionary<string, string>()
                    {
                        { "id", $"_category{category.Category_Id}"}, { "style", "display:none"}, { "name", $"_category{category.Category_Id}_"}
                    }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                i++;
            });
        }

        public static async Task AddContent_Addedbook_Authors(this HtmlDocument html, HtmlTag parent)
        {
            HtmlTag tagCurrent, tagBack;

            tagBack = parent.Tags[0];

            int i = 1;
            await dbManager.Author.ForEachAsync((author) => {
                tagCurrent = new HtmlTag("div", "",
                    new Dictionary<string, string>(), new List<HtmlTag>(), true, false);
                tagBack.Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("label", author.Author_LastName,
                    new Dictionary<string, string>()
                        {
                                {"class", "ContentLabel ContentLabelFontsizeText AddedBookCategory"}
                        }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("input", "", new Dictionary<string, string>()
                    {
                        {"type", "checkbox"}, {"class", "CheckBoxContent" }, { "id", $"author{author.Author_Id}" },
                        {"onclick",
                        $"const a = document.getElementById('_author{author.Author_Id}'); " +
                        $"const b = document.getElementById('author{author.Author_Id}'); " +
                        $"a.value = b.checked;" }
                }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                tagCurrent = new HtmlTag("input", "", new Dictionary<string, string>()
                    {
                        { "id", $"_author{author.Author_Id}"}, { "name", $"_author{author.Author_Id}_"}, { "style", "display:none"}
                    }, new List<HtmlTag>(), true, false);
                tagBack.Tags[i].Tags.Add(tagCurrent);

                i++;
            });
        }
    }
}