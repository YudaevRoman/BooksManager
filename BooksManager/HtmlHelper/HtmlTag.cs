using System.Text;

namespace BooksManager
{
    public class HtmlTag
    {
        public string Header;
        public string Value;
        public bool TagEnd;
        public bool DocType;
        public Dictionary<string, string> Attributes;
        public List<HtmlTag> Tags;

        public HtmlTag()
        {
            Header = "";
            Value = "";
            Attributes = new Dictionary<string, string>();
            Tags = new List<HtmlTag>();
            TagEnd = true;
            DocType = false;
        }

        public HtmlTag(string header, string value, Dictionary<string, string> attributes, List<HtmlTag> tags, bool tagEnd, bool docType)
        {
            Header = header;
            Value = value;
            Attributes = attributes;
            Tags = tags;
            TagEnd = tagEnd;
            DocType = docType;
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();

            str.Append($"<{Header}");

            foreach (var attribut in Attributes)
            {
                str.Append($" {attribut.Key}=\"{attribut.Value}\"");
            }
            if (TagEnd)
            {
                str.Append($">{Value}");

                foreach (var tag in Tags)
                {
                    str.Append(tag.ToString());
                }
                str.Append($"</{Header}>");
            } else
            {
                str.Append($"/>");
            }

            return str.ToString();
        }

        public object Clone ()
        {
            var clone = new HtmlTag();

            clone.Header = Header;
            clone.Value = Value;
            clone.TagEnd = TagEnd;
            clone.DocType = DocType;
            clone.Attributes = new Dictionary<string, string>(Attributes);

            if (Tags.Count > 0)
            {
                foreach (var tag in Tags)
                {
                    clone.Tags.Add((HtmlTag)tag.Clone());
                }
            }

            return clone;
        }
    }
}
