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

            // Syntax-Highlight-Sprachen hinzufügen
            contextMenuStrip.Items.Add("None", null, (sender, args) => scintilla.SetNone());
            contextMenuStrip.Items.Add("C# / C++", null, (sender, args) => scintilla.SetCpp());
            contextMenuStrip.Items.Add("Delphi", null, (sender, args) => scintilla.SetDelphi());

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

        private static void ConfigureLexer(this Scintilla scintilla)
        {
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
        }

        private static void SetNone(this Scintilla scintilla)
        {
            scintilla.ResetLexer();
            scintilla.Lexer = Lexer.Null;
        }

        private static void SetCpp(this Scintilla scintilla)
        {
            scintilla.ConfigureLexer();
            scintilla.Lexer = Lexer.Cpp;

            scintilla.SetKeywords(0, "abstract as base break case catch checked continue default delegate do else enum event explicit extern false finally fixed for foreach goto if implicit in interface internal is lock namespace new null object operator out override params private protected public readonly ref return sealed sizeof stackalloc static switch this throw true try typeof unchecked unsafe using virtual void while");
            scintilla.SetKeywords(1, "bool byte char class const decimal double float int long sbyte short static string struct uint ulong ushort var");
        }

        private static void SetDelphi(this Scintilla scintilla)
        {
            scintilla.ConfigureLexer();
            scintilla.Lexer = Lexer.Cpp;
            //scintilla.LexerLanguage = "pascal";

            scintilla.SetKeywords(0, "begin break case checked continue create default delegate do else end except false finally for goto if implementation in interface is nil of overload private protected public published return self true try type unit uses var with");
            scintilla.SetKeywords(1, "array bool byte char class const decimal double float integer string");
        }
    }
}
