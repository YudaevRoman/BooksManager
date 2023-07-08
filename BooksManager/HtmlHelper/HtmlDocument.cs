using System.Text;
using System.Xml;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BooksManager
{
    public class HtmlDocument
    {
        private HtmlTag headerDoctype;
        private HtmlTag headerHtml;
        private List<HtmlTag> tags;
        public HtmlDocument() 
        { 
            headerDoctype = new HtmlTag();
            headerHtml = new HtmlTag();
            tags = new List<HtmlTag>();

            headerDoctype.Header = "!doctype html";
            headerDoctype.DocType = true;
            headerDoctype.TagEnd = false;

            headerHtml.Header = "html";
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append(headerDoctype.ToString());
            str.Append(headerHtml.ToString());

            return str.ToString();
        }

        public void SetHeaderDoctype(HtmlTag tag)
        {
            headerDoctype = tag;
        }

        public void SetHeaderHtml(HtmlTag tag)
        {
            headerHtml = tag;
            AddTagToList(headerHtml);
        }

        private void AddTagToList(HtmlTag tag)
        {
            tags.Add(tag);
            foreach (var _tag in tag.Tags)
            {
                AddTagToList(_tag);
            }
        }

        public HtmlTag GetTag(string nameAttribut)
        {
            foreach (HtmlTag tag in tags)
            {
                if (tag.Attributes.ContainsKey("name"))
                {
                    if (tag.Attributes["name"] == nameAttribut)
                    {
                        return tag;
                    }
                }
            }
            return new HtmlTag();
        }

        public HtmlTag GetTag(string header, string attribut, string value)
        {
            foreach (var tag in tags)
            {
                if (tag.Header == header)
                {
                    if (tag.Attributes.ContainsKey(attribut))
                    {
                        if (tag.Attributes[attribut] == value)
                        {
                            return tag;
                        }
                    }
                }
            }
            return new HtmlTag();
        }

        public void SetMethod(string method)
        {
            SetAttribut("form", "method", method);
        }

        public void SetAttribut(string header, string attribut, string value)
        {
            foreach (var tag in tags)
            {
                if (tag.Header == header)
                {
                    if (tag.Attributes.ContainsKey(attribut))
                    {
                        tag.Attributes[attribut] = value;
                    }
                    else
                    {
                        tag.Attributes.Add(attribut, value);
                    }
                    return;
                }
            }
        }

    }
}
