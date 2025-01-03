using System;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Tidy.Core
{
    /// <summary>
    ///     Error/informational message reporter.
    ///     You should only need to edit the file TidyMessages.properties
    ///     to localize HTML tidy.
    ///     (c) 1998-2000 (W3C) MIT, INRIA, Keio University
    ///     Derived from
    ///     <a href="http://www.w3.org/People/Raggett/tidy">
    ///         HTML Tidy Release 4 Aug 2000
    ///     </a>
    /// </summary>
    /// <author>Dave Raggett &lt;dsr@w3.org&gt;</author>
    /// <author>Andy Quick &lt;ac.quick@sympatico.ca&gt; (translation to Java)</author>
    /// <author>Seth Yates &lt;seth_yates@hotmail.com&gt; (translation to C#)</author>
    /// <version>1.0, 1999/05/22</version>
    /// <version>1.0.1, 1999/05/29</version>
    /// <version>1.1, 1999/06/18 Java Bean</version>
    /// <version>1.2, 1999/07/10 Tidy Release 7 Jul 1999</version>
    /// <version>1.3, 1999/07/30 Tidy Release 26 Jul 1999</version>
    /// <version>1.4, 1999/09/04 DOM support</version>
    /// <version>1.5, 1999/10/23 Tidy Release 27 Sep 1999</version>
    /// <version>1.6, 1999/11/01 Tidy Release 22 Oct 1999</version>
    /// <version>1.7, 1999/12/06 Tidy Release 30 Nov 1999</version>
    /// <version>1.8, 2000/01/22 Tidy Release 13 Jan 2000</version>
    /// <version>1.9, 2000/06/03 Tidy Release 30 Apr 2000</version>
    /// <version>1.10, 2000/07/22 Tidy Release 8 Jul 2000</version>
    /// <version>1.11, 2000/08/16 Tidy Release 4 Aug 2000</version>
    internal class Report
    {
        /* used to point to Web Accessibility Guidelines */
        public const string ACCESS_URL = "http://www.w3.org/WAI/GL";

        /* error codes for entities */
        public const int MISSING_SEMICOLON = 1;
        public const int UNKNOWN_ENTITY = 2;
        public const int UNESCAPED_AMPERSAND = 3;

        /* error codes for element messages */
        public const int MISSING_ENDTAG_FOR = 1;
        public const int MISSING_ENDTAG_BEFORE = 2;
        public const int DISCARDING_UNEXPECTED = 3;
        public const int NESTED_EMPHASIS = 4;
        public const int NON_MATCHING_ENDTAG = 5;
        public const int TAG_NOT_ALLOWED_IN = 6;
        public const int MISSING_STARTTAG = 7;
        public const int UNEXPECTED_ENDTAG = 8;
        public const int USING_BR_INPLACE_OF = 9;
        public const int INSERTING_TAG = 10;
        public const int SUSPECTED_MISSING_QUOTE = 11;
        public const int MISSING_TITLE_ELEMENT = 12;
        public const int DUPLICATE_FRAMESET = 13;
        public const int CANT_BE_NESTED = 14;
        public const int OBSOLETE_ELEMENT = 15;
        public const int PROPRIETARY_ELEMENT = 16;
        public const int UNKNOWN_ELEMENT = 17;
        public const int TRIM_EMPTY_ELEMENT = 18;
        public const int COERCE_TO_ENDTAG = 19;
        public const int ILLEGAL_NESTING = 20;
        public const int NOFRAMES_CONTENT = 21;
        public const int CONTENT_AFTER_BODY = 22;
        public const int INCONSISTENT_VERSION = 23;
        public const int MALFORMED_COMMENT = 24;
        public const int BAD_COMMENT_CHARS = 25;
        public const int BAD_XML_COMMENT = 26;
        public const int BAD_CDATA_CONTENT = 27;
        public const int INCONSISTENT_NAMESPACE = 28;
        public const int DOCTYPE_AFTER_TAGS = 29;
        public const int MALFORMED_DOCTYPE = 30;
        public const int UNEXPECTED_END_OF_FILE = 31;
        public const int DTYPE_NOT_UPPER_CASE = 32;
        public const int TOO_MANY_ELEMENTS = 33;

        /* error codes used for attribute messages */
        public const int UNKNOWN_ATTRIBUTE = 1;
        public const int MISSING_ATTRIBUTE = 2;
        public const int MISSING_ATTR_VALUE = 3;
        public const int BAD_ATTRIBUTE_VALUE = 4;
        public const int UNEXPECTED_GT = 5;
        public const int PROPRIETARY_ATTR_VALUE = 6;
        public const int REPEATED_ATTRIBUTE = 7;
        public const int MISSING_IMAGEMAP = 8;
        public const int XML_ATTRIBUTE_VALUE = 9;
        public const int UNEXPECTED_QUOTEMARK = 10;
        public const int ID_NAME_MISMATCH = 11;

        /* accessibility flaws */
        public const int MISSING_IMAGE_ALT = 1;
        public const int MISSING_LINK_ALT = 2;
        public const int MISSING_SUMMARY = 4;
        public const int MISSING_IMAGE_MAP = 8;
        public const int USING_FRAMES = 16;
        public const int USING_NOFRAMES = 32;

        /* presentation flaws */
        public const int USING_SPACER = 1;
        public const int USING_LAYER = 2;
        public const int USING_NOBR = 4;
        public const int USING_FONT = 8;
        public const int USING_BODY = 16;

        /* character encoding errors */
        public const int WINDOWS_CHARS = 1;
        public const int NON_ASCII = 2;
        public const int FOUND_UTF16 = 4;

        private static readonly ResourceManager Res;

        static Report()
        {
            try
            {
                Res = new ResourceManager("TidyNet.TidyMessages", typeof(Report).GetTypeInfo().Assembly);
            }
            catch (MissingManifestResourceException e)
            {
                throw new Exception(e.Message, e);
            }
        }

        public static short OptionErrors { get; set; }

        private static string Tag(Node tag)
        {
            if (tag != null)
            {
                if (tag.Type == Node.START_TAG)
                {
                    return "<" + tag.Element + ">";
                }
                if (tag.Type == Node.END_TAG)
                {
                    return "</" + tag.Element + ">";
                }
                if (tag.Type == Node.DOC_TYPE_TAG)
                {
                    return "<!DOCTYPE>";
                }
                if (tag.Type == Node.TEXT_NODE)
                {
                    return "plain text";
                }
                return tag.Element;
            }
            return String.Empty;
        }

        /* lexer is not defined when this is called */

        public static void BadArgument(string option)
        {
            OptionErrors++;
            throw new InvalidOperationException(String.Format(GetMessage("bad_argument"), option));
        }

        public static void EncodingError(Lexer lexer, short code, int c)
        {
            if (code == WINDOWS_CHARS)
            {
                lexer.BadChars |= WINDOWS_CHARS;
                AddMessage(lexer, String.Format(GetMessage("illegal_char"), c), MessageLevel.Warning);
            }
        }

        public static void EntityError(Lexer lexer, short code, string entity, int c)
        {
            if (code == MISSING_SEMICOLON)
            {
                AddMessage(lexer, String.Format(GetMessage("missing_semicolon"), entity), MessageLevel.Warning);
            }
            else if (code == UNKNOWN_ENTITY)
            {
                AddMessage(lexer, String.Format(GetMessage("unknown_entity"), entity), MessageLevel.Warning);
            }
            else if (code == UNESCAPED_AMPERSAND)
            {
                AddMessage(lexer, GetMessage("unescaped_ampersand"), MessageLevel.Warning);
            }
        }

        public static void AttrError(Lexer lexer, Node node, string attr, short code)
        {
            /* keep quiet after 6 errors */
            if (lexer.Messages.Errors > 6)
            {
                return;
            }

            /* on end of file adjust reported position to end of input */
            if (code == UNEXPECTED_END_OF_FILE)
            {
                lexer.Lines = lexer.Input.CursorLine;
                lexer.Columns = lexer.Input.CursorColumn;
            }

            if (code == UNKNOWN_ATTRIBUTE)
            {
                AddMessage(lexer, String.Format(GetMessage("unknown_attribute"), attr), MessageLevel.Warning);
            }
            else if (code == MISSING_ATTRIBUTE)
            {
                AddMessage(lexer, String.Format(GetMessage("missing_attribute"), Tag(node), attr),
                           MessageLevel.Warning);
            }
            else if (code == MISSING_ATTR_VALUE)
            {
                AddMessage(lexer, String.Format(GetMessage("missing_attr_value"), Tag(node), attr),
                           MessageLevel.Warning);
            }
            else if (code == MISSING_IMAGEMAP)
            {
                AddMessage(lexer, String.Format(GetMessage("missing_imagemap"), Tag(node)), MessageLevel.Warning);

                lexer.BadAccess |= MISSING_IMAGE_MAP;
            }
            else if (code == BAD_ATTRIBUTE_VALUE)
            {
                AddMessage(lexer, String.Format(GetMessage("bad_attribute_value"), Tag(node), attr),
                           MessageLevel.Warning);
            }
            else if (code == XML_ATTRIBUTE_VALUE)
            {
                AddMessage(lexer, String.Format(GetMessage("xml_attribute_value"), Tag(node), attr),
                           MessageLevel.Warning);
            }
            else if (code == UNEXPECTED_GT)
            {
                AddMessage(lexer, String.Format(GetMessage("unexpected_gt"), Tag(node)), MessageLevel.Error);
            }
            else if (code == UNEXPECTED_QUOTEMARK)
            {
                AddMessage(lexer, String.Format(GetMessage("unexpected_quotemark"), Tag(node)),
                           MessageLevel.Warning);
            }
            else if (code == REPEATED_ATTRIBUTE)
            {
                AddMessage(lexer, String.Format(GetMessage("repeated_attribute"), Tag(node)),
                           MessageLevel.Warning);
            }
            else if (code == PROPRIETARY_ATTR_VALUE)
            {
                AddMessage(lexer, String.Format(GetMessage("proprietary_attr_value"), Tag(node), attr),
                           MessageLevel.Warning);
            }
            else if (code == UNEXPECTED_END_OF_FILE)
            {
                AddMessage(lexer, GetMessage("unexpected_end_of_file"), MessageLevel.Error);
            }
            else if (code == ID_NAME_MISMATCH)
            {
                AddMessage(lexer, String.Format(GetMessage("id_name_mismatch"), Tag(node)), MessageLevel.Warning);
            }

            if (code == UNEXPECTED_GT)
            {
                AddMessage(lexer, String.Format(GetMessage("unexpected_gt"), Tag(node)), MessageLevel.Error);
            }
        }

        public static void Warning(Lexer lexer, Node element, Node node, short code)
        {
            TagCollection tt = lexer.Options.TagTable;

            /* keep quiet after 6 errors */
            if (lexer.Messages.Errors > 6)
            {
                return;
            }

            /* on end of file adjust reported position to end of input */
            if (code == UNEXPECTED_END_OF_FILE)
            {
                lexer.Lines = lexer.Input.CursorLine;
                lexer.Columns = lexer.Input.CursorColumn;
            }

            if (code == MISSING_ENDTAG_FOR)
            {
                AddMessage(lexer, String.Format(GetMessage("missing_endtag_for"), element.Element), MessageLevel.Warning);
            }
            else if (code == MISSING_ENDTAG_BEFORE)
            {
                AddMessage(lexer, String.Format(GetMessage("missing_endtag_before"), element.Element, Tag(node)),
                           MessageLevel.Warning);
            }
            else if (code == DISCARDING_UNEXPECTED)
            {
                AddMessage(lexer, String.Format(GetMessage("discarding_unexpected"), Tag(node)),
                           MessageLevel.Warning);
            }
            else if (code == NESTED_EMPHASIS)
            {
                AddMessage(lexer, String.Format(GetMessage("nested_emphasis"), Tag(node)), MessageLevel.Warning);
            }
            else if (code == COERCE_TO_ENDTAG)
            {
                AddMessage(lexer, String.Format(GetMessage("coerce_to_endtag"), element.Element), MessageLevel.Warning);
            }
            else if (code == NON_MATCHING_ENDTAG)
            {
                AddMessage(lexer, String.Format(GetMessage("non_matching_endtag_1"), Tag(node), element.Element),
                           MessageLevel.Warning);
            }
            else if (code == TAG_NOT_ALLOWED_IN)
            {
                AddMessage(lexer, String.Format(GetMessage("tag_not_allowed_in"), Tag(node), element.Element),
                           MessageLevel.Warning);
            }
            else if (code == DOCTYPE_AFTER_TAGS)
            {
                AddMessage(lexer, GetMessage("doctype_after_tags"), MessageLevel.Warning);
            }
            else if (code == MISSING_STARTTAG)
            {
                AddMessage(lexer, String.Format(GetMessage("missing_starttag"), node.Element), MessageLevel.Warning);
            }
            else if (code == UNEXPECTED_ENDTAG)
            {
                string message = element != null
                                     ? String.Format(GetMessage("unexpected_endtag_suffix"), node.Element,
                                                     element.Element)
                                     : String.Format(GetMessage("unexpected_endtag"), node.Element);

                AddMessage(lexer, message, MessageLevel.Warning);
            }
            else if (code == TOO_MANY_ELEMENTS)
            {
                string message = element != null
                                     ? String.Format(GetMessage("too_many_elements_suffix"), node.Element,
                                                     element.Element)
                                     : String.Format(GetMessage("too_many_elements"), node.Element);

                AddMessage(lexer, message, MessageLevel.Warning);
            }
            else if (code == USING_BR_INPLACE_OF)
            {
                AddMessage(lexer, GetMessage("using_br_inplace_of") + Tag(node), MessageLevel.Warning);
            }
            else if (code == INSERTING_TAG)
            {
                AddMessage(lexer, String.Format(GetMessage("inserting_tag"), node.Element), MessageLevel.Warning);
            }
            else if (code == CANT_BE_NESTED)
            {
                AddMessage(lexer, String.Format(GetMessage("cant_be_nested"), Tag(node)), MessageLevel.Warning);
            }
            else if (code == PROPRIETARY_ELEMENT)
            {
                AddMessage(lexer, String.Format(GetMessage("proprietary_element"), Tag(node)),
                           MessageLevel.Warning);

                if (node.Tag == tt.TagLayer)
                {
                    lexer.BadLayout |= USING_LAYER;
                }
                else if (node.Tag == tt.TagSpacer)
                {
                    lexer.BadLayout |= USING_SPACER;
                }
                else if (node.Tag == tt.TagNobr)
                {
                    lexer.BadLayout |= USING_NOBR;
                }
            }
            else if (code == OBSOLETE_ELEMENT)
            {
                string message;
                if (element.Tag != null && (element.Tag.Model & ContentModel.OBSOLETE) != 0)
                {
                    message = String.Format(GetMessage("obsolete_element"), Tag(node), Tag(node));
                }
                else
                {
                    message = String.Format(GetMessage("replacing_element"), Tag(node), Tag(node));
                }

                AddMessage(lexer, message, MessageLevel.Warning);
            }
            else if (code == TRIM_EMPTY_ELEMENT)
            {
                AddMessage(lexer, String.Format(GetMessage("trim_empty_element"), Tag(node)),
                           MessageLevel.Warning);
            }
            else if (code == MISSING_TITLE_ELEMENT)
            {
                AddMessage(lexer, GetMessage("missing_title_element"), MessageLevel.Warning);
            }
            else if (code == ILLEGAL_NESTING)
            {
                AddMessage(lexer, String.Format(GetMessage("illegal_nesting"), Tag(node)), MessageLevel.Warning);
            }
            else if (code == NOFRAMES_CONTENT)
            {
                AddMessage(lexer, String.Format(GetMessage("noframes_content"), Tag(node)), MessageLevel.Warning);
            }
            else if (code == INCONSISTENT_VERSION)
            {
                AddMessage(lexer, GetMessage("inconsistent_version"), MessageLevel.Warning);
            }
            else if (code == MALFORMED_DOCTYPE)
            {
                AddMessage(lexer, GetMessage("malformed_doctype"), MessageLevel.Warning);
            }
            else if (code == CONTENT_AFTER_BODY)
            {
                AddMessage(lexer, GetMessage("content_after_body"), MessageLevel.Warning);
            }
            else if (code == MALFORMED_COMMENT)
            {
                AddMessage(lexer, GetMessage("malformed_comment"), MessageLevel.Warning);
            }
            else if (code == BAD_COMMENT_CHARS)
            {
                AddMessage(lexer, GetMessage("bad_comment_chars"), MessageLevel.Warning);
            }
            else if (code == BAD_XML_COMMENT)
            {
                AddMessage(lexer, GetMessage("bad_xml_comment"), MessageLevel.Warning);
            }
            else if (code == BAD_CDATA_CONTENT)
            {
                AddMessage(lexer, GetMessage("bad_cdata_content"), MessageLevel.Warning);
            }
            else if (code == INCONSISTENT_NAMESPACE)
            {
                AddMessage(lexer, GetMessage("inconsistent_namespace"), MessageLevel.Warning);
            }
            else if (code == DTYPE_NOT_UPPER_CASE)
            {
                AddMessage(lexer, GetMessage("dtype_not_upper_case"), MessageLevel.Warning);
            }
            else if (code == UNEXPECTED_END_OF_FILE)
            {
                AddMessage(lexer, GetMessage("unexpected_end_of_file") + Tag(node), MessageLevel.Warning);
            }
        }

        public static void Error(Lexer lexer, Node element, Node node, short code)
        {
            /* keep quiet after 6 errors */
            if (lexer.Messages.Errors > 6)
            {
                return;
            }

            if (code == SUSPECTED_MISSING_QUOTE)
            {
                AddMessage(lexer, GetMessage("suspected_missing_quote"), MessageLevel.Error);
            }
            else if (code == DUPLICATE_FRAMESET)
            {
                AddMessage(lexer, GetMessage("duplicate_frameset"), MessageLevel.Error);
            }
            else if (code == UNKNOWN_ELEMENT)
            {
                AddMessage(lexer, String.Format(GetMessage("unknown_element"), Tag(node)), MessageLevel.Error);
            }
            else if (code == UNEXPECTED_ENDTAG)
            {
                if (element != null)
                {
                    string message = String.Format(GetMessage("unexpected_endtag_suffix"), element.Element);
                    AddMessage(lexer, message, MessageLevel.Error);
                }
            }
        }

        public static void ErrorSummary(Lexer lexer)
        {
            /* adjust badAccess to that its null if frames are ok */
            if ((lexer.BadAccess & (USING_FRAMES | USING_NOFRAMES)) != 0)
            {
                if (!(((lexer.BadAccess & USING_FRAMES) != 0) && ((lexer.BadAccess & USING_NOFRAMES) == 0)))
                {
                    lexer.BadAccess &= ~(USING_FRAMES | USING_NOFRAMES);
                }
            }

            if (lexer.BadChars != 0)
            {
                if ((lexer.BadChars & WINDOWS_CHARS) != 0)
                {
                    AddMessage(lexer, GetMessage("badchars_summary"), MessageLevel.Info);
                }
            }

            if (lexer.BadForm != 0)
            {
                AddMessage(lexer, GetMessage("badform_summary"), MessageLevel.Info);
            }

            if (lexer.BadAccess != 0)
            {
                if ((lexer.BadAccess & MISSING_SUMMARY) != 0)
                {
                    AddMessage(lexer, GetMessage("badaccess_missing_summary"), MessageLevel.Info);
                }

                if ((lexer.BadAccess & MISSING_IMAGE_ALT) != 0)
                {
                    AddMessage(lexer, GetMessage("badaccess_missing_image_alt"), MessageLevel.Info);
                }

                if ((lexer.BadAccess & MISSING_IMAGE_MAP) != 0)
                {
                    AddMessage(lexer, GetMessage("badaccess_missing_image_map"), MessageLevel.Info);
                }

                if ((lexer.BadAccess & MISSING_LINK_ALT) != 0)
                {
                    AddMessage(lexer, GetMessage("badaccess_missing_link_alt"), MessageLevel.Info);
                }

                if (((lexer.BadAccess & USING_FRAMES) != 0) && ((lexer.BadAccess & USING_NOFRAMES) == 0))
                {
                    AddMessage(lexer, GetMessage("badaccess_frames"), MessageLevel.Info);
                }

                var msg2 = new TidyMessage(lexer, String.Format(GetMessage("badaccess_summary"), ACCESS_URL),
                                           MessageLevel.Info);
                lexer.Messages.Add(msg2);
            }

            if (lexer.BadLayout != 0)
            {
                if ((lexer.BadLayout & USING_LAYER) != 0)
                {
                    AddMessage(lexer, GetMessage("badlayout_using_layer"), MessageLevel.Info);
                }

                if ((lexer.BadLayout & USING_SPACER) != 0)
                {
                    AddMessage(lexer, GetMessage("badlayout_using_spacer"), MessageLevel.Info);
                }

                if ((lexer.BadLayout & USING_FONT) != 0)
                {
                    AddMessage(lexer, GetMessage("badlayout_using_font"), MessageLevel.Info);
                }

                if ((lexer.BadLayout & USING_NOBR) != 0)
                {
                    AddMessage(lexer, GetMessage("badlayout_using_nobr"), MessageLevel.Info);
                }

                if ((lexer.BadLayout & USING_BODY) != 0)
                {
                    AddMessage(lexer, GetMessage("badlayout_using_body"), MessageLevel.Info);
                }
            }
        }

        public static void NeedsAuthorIntervention(Lexer lexer)
        {
            AddMessage(lexer, GetMessage("needs_author_intervention"), MessageLevel.Info);
        }

        public static void MissingBody(Lexer lexer)
        {
            AddMessage(lexer, GetMessage("missing_body"), MessageLevel.Info);
        }

        public static void ReportNumberOfSlides(Lexer lexer, int count)
        {
            AddMessage(lexer, String.Format(GetMessage("slides_found"), count), MessageLevel.Info);
        }

        public static void ReportVersion(Lexer lexer, Node doctype)
        {
            int state = 0;
            string vers = lexer.HtmlVersionName();
            var cc = new MutableInteger();

            if (doctype != null)
            {
                var docTypeStr = new StringBuilder();

                int i;
                for (i = doctype.Start; i < doctype.End; ++i)
                {
                    int c = doctype.Textarray[i];

                    /* look for UTF-8 multibyte character */
                    if (c < 0)
                    {
                        i += PPrint.GetUtf8(doctype.Textarray, i, cc);
                        c = cc.Val;
                    }

                    if (c == '"')
                    {
                        ++state;
                    }
                    else if (state == 1)
                    {
                        docTypeStr.Append((char)c);
                    }
                }

                lexer.Messages.Add(new TidyMessage(lexer, String.Format(GetMessage("doctype_given"), docTypeStr),
                                                   MessageLevel.Info));
            }

            lexer.Messages.Add(new TidyMessage(lexer,
                                               String.Format(GetMessage("report_version"),
                                                             (vers ?? "HTML proprietary")),
                                               MessageLevel.Info));
        }

        public static void ReportNumWarnings(Lexer lexer)
        {
            if ((lexer.Messages.Warnings > 0) && (lexer.Messages.Errors > 0))
            {
                lexer.Messages.Add(new TidyMessage(lexer,
                                                   String.Format(GetMessage("num_warnings_errors"),
                                                                 lexer.Messages.Warnings, lexer.Messages.Errors),
                                                   MessageLevel.Info));
            }
            else if (lexer.Messages.Errors > 0)
            {
                lexer.Messages.Add(new TidyMessage(lexer, String.Format(GetMessage("num_errors"), lexer.Messages.Errors),
                                                   MessageLevel.Info));
            }
            else if (lexer.Messages.Warnings > 0)
            {
                lexer.Messages.Add(new TidyMessage(lexer,
                                                   String.Format(GetMessage("num_warnings"), lexer.Messages.Warnings),
                                                   MessageLevel.Info));
            }
            else
            {
                lexer.Messages.Add(new TidyMessage(lexer, GetMessage("no_warnings"), MessageLevel.Info));
            }
        }

        public static void AddMessage(Lexer lexer, string message, MessageLevel level)
        {
            lexer.Messages.Add(new TidyMessage(lexer, message, level));
        }

        public static void BadTree(Lexer lexer)
        {
            throw new InvalidOperationException(GetMessage("bad_tree"));
        }

        public static string GetMessage(string key)
        {
            try
            {
                return key;
                // return Res.GetString(key);
            }
            catch (MissingManifestResourceException)
            {
                return key + " resource key missing";// e.ToString();
            }
        }
    }
}