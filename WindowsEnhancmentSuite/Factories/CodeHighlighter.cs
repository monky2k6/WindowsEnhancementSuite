using System.Drawing;
using System.Windows.Forms;
using ScintillaNET;

namespace WindowsEnhancementSuite.Factories
{
    public static class CodeHighlighter
    {
        public static Scintilla Create(string text, Control Parent)
        {
            var size = Parent.ClientSize;
            return Create(text, Parent, size);
        }

        public static Scintilla Create(string text, Control Parent, Size Size)
        {
            var scintilla = new Scintilla
            {
                AllowDrop = false,
                Text = text,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom,
                Lexer = Lexer.Null,
                Size = Size,
                Parent = Parent
            };
            scintilla.Margins[0].Width = 33;
            scintilla.Margins[0].Cursor = MarginCursor.ReverseArrow;
            scintilla.Margins[1].Width = 0;
            scintilla.Margins[2].Width = 0;
            scintilla.Margins[3].Width = 0;

            scintilla.AddContextMenuStrip();
            scintilla.SetNone();

            Parent.Controls.Add(scintilla);

            return scintilla;
        }

        private static void AddContextMenuStrip(this Scintilla scintilla)
        {
            var contextMenuStrip = new ContextMenuStrip();
            contextMenuStrip.Items.Add("Select all", null, (sender, args) => scintilla.SelectAll());
            contextMenuStrip.Items.Add("-");
            contextMenuStrip.Items.Add("Cut", null, (sender, args) => scintilla.Cut());
            contextMenuStrip.Items.Add("Copy", null, (sender, args) => scintilla.Copy());
            contextMenuStrip.Items.Add("Paste", null, (sender, args) => scintilla.Paste());
            contextMenuStrip.Items.Add("-");

            // Syntax-Highlight-Sprachen hinzufügen
            var lexerSelectorMenu = new ToolStripComboBox("Syntax") {DropDownStyle = ComboBoxStyle.DropDownList};
            lexerSelectorMenu.Items.Add("None");
            lexerSelectorMenu.Items.Add("C# / C++");
            lexerSelectorMenu.Items.Add("Delphi");
            lexerSelectorMenu.Items.Add("JavaScript");

            lexerSelectorMenu.SelectedIndex = 0;
            lexerSelectorMenu.DropDownClosed += (sender, args) =>
            {
                switch (lexerSelectorMenu.SelectedIndex)
                {
                    case 0:
                        scintilla.SetNone();
                        break;
                    case 1:
                        scintilla.SetCpp();
                        break;
                    case 2:
                        scintilla.SetDelphi();
                        break;
                    case 3:
                        scintilla.SetJs();
                        break;
                }
            };

            contextMenuStrip.Items.Add(lexerSelectorMenu);
            scintilla.ContextMenuStrip = contextMenuStrip;
        }

        private static void ResetLexer(this Scintilla scintilla)
        {
            scintilla.StyleResetDefault();
            scintilla.Styles[Style.Default].Font = "Consolas";
            scintilla.Styles[Style.Default].Size = 10;
            scintilla.Styles[Style.Default].ForeColor = Color.Black;
            scintilla.StyleClearAll();
        }

        private static void ConfigureLexer(this Scintilla scintilla, LexerStyle lexerStyle)
        {
            switch (lexerStyle)
            {
                case LexerStyle.CSharp:
                    scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Black;
                    scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0);
                    scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0);
                    scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128);
                    scintilla.Styles[Style.Cpp.Number].ForeColor = Color.LightSeaGreen;
                    scintilla.Styles[Style.Cpp.Word].ForeColor = Color.Blue;
                    scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.LightSkyBlue;
                    scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                    scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                    scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Crimson;
                    break;
                case LexerStyle.Delphi:
                    scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Black;
                    scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0);
                    scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0);
                    scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128);
                    scintilla.Styles[Style.Cpp.Number].ForeColor = Color.LightSeaGreen;
                    scintilla.Styles[Style.Cpp.Word].ForeColor = Color.DarkBlue;
                    scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Blue;
                    scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                    scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                    scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Crimson;
                    break;
                case LexerStyle.JavaScript:
                    scintilla.Styles[Style.Cpp.Default].ForeColor = Color.Black;
                    scintilla.Styles[Style.Cpp.Comment].ForeColor = Color.FromArgb(0, 128, 0);
                    scintilla.Styles[Style.Cpp.CommentLine].ForeColor = Color.FromArgb(0, 128, 0);
                    scintilla.Styles[Style.Cpp.CommentLineDoc].ForeColor = Color.FromArgb(128, 128, 128);
                    scintilla.Styles[Style.Cpp.Number].ForeColor = Color.LightSeaGreen;
                    scintilla.Styles[Style.Cpp.Word].ForeColor = Color.DarkOrange;
                    scintilla.Styles[Style.Cpp.Word2].ForeColor = Color.Gold;
                    scintilla.Styles[Style.Cpp.String].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.Character].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.Verbatim].ForeColor = Color.FromArgb(163, 21, 21);
                    scintilla.Styles[Style.Cpp.StringEol].BackColor = Color.Pink;
                    scintilla.Styles[Style.Cpp.Operator].ForeColor = Color.Purple;
                    scintilla.Styles[Style.Cpp.Preprocessor].ForeColor = Color.Crimson;
                    break;
            }
            
        }

        private static void SetNone(this Scintilla scintilla)
        {
            scintilla.ResetLexer();
            scintilla.Lexer = Lexer.Null;
        }

        private static void SetCpp(this Scintilla scintilla)
        {
            scintilla.ConfigureLexer(LexerStyle.CSharp);
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, "abstract as base bool break byte case catch char checked class const continue default delegate do double else enum event explicit extern false finally fixed float for foreach goto if implicit in int interface internal is lock long namespace new null object operator out override params private protected public readonly ref return sbyte sealed short sizeof stackalloc static string struct switch this throw true try typeof uint ulong unchecked unsafe ushort using virtual void volatile while");
            scintilla.SetKeywords(1, "decimal var");
        }

        private static void SetDelphi(this Scintilla scintilla)
        {
            scintilla.ConfigureLexer(LexerStyle.Delphi);
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, "and array as begin case class const constructor destructor div do downto else end except file finally for function goto if implementation in inherited interface is mod not object of on or packed procedure program property raise record repeat set shl shr then threadvar to try type unit until uses var while with xor");
            scintilla.SetKeywords(1, "bool byte double float integer string");
        }

        private static void SetJs(this Scintilla scintilla)
        {
            scintilla.ConfigureLexer(LexerStyle.JavaScript);
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, "abstract arguments boolean break byte case catch char class const continue debugger default delete do double else enum eval export extends false final finally float for function goto if implements import in instanceof int interface let long native new null package private protected public return short static super switch synchronized this throw throws transient true try typeof var void volatile while with yield");
            scintilla.SetKeywords(1, "array date hasOwnProperty infinity isFinite isNaN isPrototypeOf length math NaN name number object prototype string toString undefined valueOf");
        }

        private enum LexerStyle
        {
            CSharp,
            Delphi,
            JavaScript
        }
    }
}
