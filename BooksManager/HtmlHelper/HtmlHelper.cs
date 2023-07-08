
namespace BooksManager
{
    public static class HtmlHelper
    {
        public static void WriteToFile(HtmlDocument html, string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.Write(html.ToString());
            }
        }

        public static HtmlDocument FromHtmlFile(string filePath)
        {
            var html = new HtmlDocument();
            string str;

            using (StreamReader reader = new StreamReader(filePath))
            {
                List<HtmlTag> tags = new List<HtmlTag>();
                HtmlTag tag = new HtmlTag();
                string attributName = "", attributValue = "";
                EnumTagTask task = EnumTagTask.EXPECTATION_TAG_START;
                int indexStart = 0;
                str = reader.ReadToEnd();

                for (int i = 0; i < str.Length; i++)
                {
                    switch (task)
                    {
                        case EnumTagTask.DOCTYPE:
                            tag.Header = "!doctype html";
                            tag.DocType = true;
                            tag.TagEnd = false;
                            tags.Add(tag);
                            task = EnumTagTask.EXPECTATION_TAG_START;
                            break;
                        case EnumTagTask.EXPECTATION_TAG_START:
                            if (OnExpectationTagStart(ref task, str[i]))
                            {
                                tag = new HtmlTag();
                            }
                            break;
                        case EnumTagTask.EXPECTATION_TAG_END:
                            if (OnExpectationTagEnd(ref task, str[i]))
                            {
                                tag.TagEnd = false;
                                task = EnumTagTask.EXPECTATION_TAG_INCLUDE;
                            }
                            break;
                        case EnumTagTask.EXPECTATION_TAG_INCLUDE:
                            HtmlTag parent = tags.Last();
                            if (!parent.DocType)
                            {
                                parent.Tags.Add(tag);
                            }
                            task = EnumTagTask.EXPECTATION_TAG_START;
                            break;
                        case EnumTagTask.PASS_TAG_END:
                            if (OnPassTagEnd(ref task, str[i]))
                            {
                                HtmlTag paren = tags[tags.Count - 2];
                                if (!paren.DocType)
                                {
                                    tag = tags.Last();
                                    tags.Remove(tag);
                                    paren.Tags.Add(tag);
                                }
                            }
                            break;
                        case EnumTagTask.EXPECTATION_TAG_NAME:
                            if (OnExpectationTagName(ref task, str[i]))
                            {
                                indexStart = i;
                            }
                            break;
                        case EnumTagTask.FORMING_TAG_NAME:
                            if (OnFormingTagName(ref task, str[i]))
                            {
                                tag.Header = str.Substring(indexStart, i - indexStart);
                            }
                            break;
                        case EnumTagTask.EXPECTATION_ATTRIBUT_NAME:
                            if (OnExpectationAttributName(ref task, str[i]))
                            {
                                indexStart = i;
                            }
                            break;
                        case EnumTagTask.FORMING_ATTRIBUT_NAME:
                            if (OnFormingAttributName(ref task, str[i]))
                            {
                                attributName = str.Substring(indexStart, i - indexStart);
                            }
                            break;
                        case EnumTagTask.EXPECTATION_ATTRIBUT_VALUE:
                            if (OnExpectationAttributValue(ref task, str[i]))
                            {
                                indexStart = i + 1;
                            }
                            break;
                        case EnumTagTask.EXPECTATION_APOSTROPHE:
                            if (OnExpectationApostrophe(ref task, str[i]))
                            {
                                attributValue = str.Substring(indexStart, i - indexStart);
                                tag.Attributes.Add(attributName, attributValue);
                            }
                            break;
                        case EnumTagTask.EXPECTATION_QUOTATION_MARKS:
                            if (OnExpectationQuotationMarks(ref task, str[i]))
                            {
                                attributValue = str.Substring(indexStart, i - indexStart);
                                tag.Attributes.Add(attributName, attributValue);
                                if (attributValue == "elementCollection_Categories")
                                {
                                    int b = 10;
                                    b++;
                                }
                            }
                            break;
                        case EnumTagTask.EXPECTATION_TAG_VALUE:
                            indexStart = i--;
                            task = EnumTagTask.FORMING_TAG_VALUE;
                            break;
                        case EnumTagTask.FORMING_TAG_VALUE:
                            if (OnExpectationTagStart(ref task, str[i]))
                            {
                                tag.Value = str.Substring(indexStart, i - indexStart).Trim(new char[] { '\r', '\n', '\t', ' '});
                                tags.Add(tag);
                                tag = new HtmlTag();
                            }
                            break;
                    }
                }

                switch (tags.Count) {
                    case 2:
                        if (tags[0].Header == "!doctype html") {
                            html.SetHeaderDoctype(tags[0]);
                        }
                        if (tags[1].Header == "html") {
                            html.SetHeaderHtml(tags[1]);
                        }
                        break;
                    case 1:
                        if (tags[0].Header == "html") {
                            html.SetHeaderHtml(tags[0]);
                        }
                        break;
                }

                return html;
            }
        }
        private static bool OnExpectationTagStart(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case '<':
                    task = EnumTagTask.EXPECTATION_TAG_NAME;
                    return true;
            }
            return false;
        }
        private static bool OnExpectationTagEnd(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    return false;
                case '/':
                    task = EnumTagTask.PASS_TAG_END;
                    return true;
                default:
                    task = EnumTagTask.EXPECTATION_TAG_INCLUDE;
                    return true;
            }
        }
        private static bool OnPassTagEnd(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case '>':
                    task = EnumTagTask.EXPECTATION_TAG_START;
                    return true;
            }
            return false;
        }
        private static bool OnExpectationTagName(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    return false;
                case '!':
                    task = EnumTagTask.DOCTYPE;
                    return true;
                case '/':
                    task = EnumTagTask.PASS_TAG_END;
                    return true;
                default:
                    task = EnumTagTask.FORMING_TAG_NAME;
                    return true;
            }
        }
        private static bool OnFormingTagName(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    task = EnumTagTask.EXPECTATION_ATTRIBUT_NAME;
                    return true;
                case '/':
                    task = EnumTagTask.EXPECTATION_TAG_END;
                    return true;
                case '>':
                    task = EnumTagTask.EXPECTATION_TAG_VALUE;
                    return true;
            }
            return false;
        }
        private static bool OnExpectationAttributName(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case ' ':
                case '\r':
                case '\n':
                case '\t':
                    return false;
                case '/':
                    task = EnumTagTask.EXPECTATION_TAG_END;
                    return true;
                case '>':
                    task = EnumTagTask.EXPECTATION_TAG_VALUE;
                    return true;
                default:
                    task = EnumTagTask.FORMING_ATTRIBUT_NAME;
                    return true;
            }
        }
        private static bool OnFormingAttributName(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case ' ':
                case '=':
                case '\r':
                case '\n':
                case '\t':
                    task = EnumTagTask.EXPECTATION_ATTRIBUT_VALUE;
                    return true;
            }
            return false;
        }
        private static bool OnExpectationAttributValue(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case '\'':
                    task = EnumTagTask.EXPECTATION_APOSTROPHE;
                    return true;
                case '\"':
                    task = EnumTagTask.EXPECTATION_QUOTATION_MARKS;
                    return true;
            }
            return false;
        }
        private static bool OnExpectationApostrophe(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case '\'':
                    task = EnumTagTask.EXPECTATION_ATTRIBUT_NAME;
                    return true;
            }
            return false;
        }
        private static bool OnExpectationQuotationMarks(ref EnumTagTask task, char ch)
        {
            switch (ch)
            {
                case '\"':
                    task = EnumTagTask.EXPECTATION_ATTRIBUT_NAME;
                    return true;
            }
            return false;
        }

    }
}
