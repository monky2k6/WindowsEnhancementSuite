using System;
using System.Windows.Forms;

namespace WindowsEnhancementSuite.Bases
{
    public class DraggableBaseForm : Form, IDataObject
    {
        public const string DRAG_FORMAT = "draggableForm";
        public const string DRAG_TYPE_TEXT = "draggableFormText";
        public const string DRAG_TYPE_IMAGE = "draggableFormImage";
        public const string DRAG_TYPE_FILELIST = "draggableFormFilelist";

        protected readonly Control dragControl;
        public object GetData(string format, bool autoConvert)
        {
            if (format == DRAG_FORMAT)
            {
                if (autoConvert)
                    return dragControl;
                else
                    return this;
            }

            return null;
        }

        public object GetData(string format)
        {
            return this.GetData(format, true);
        }

        public object GetData(Type format)
        {
            return this.GetData(format.ToString());
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            if (autoConvert)
                return (format == DRAG_FORMAT && this.dragControl != null);
            else
                return (format == DRAG_FORMAT);
        }

        public bool GetDataPresent(string format)
        {
            return this.GetDataPresent(format, true);
        }

        public bool GetDataPresent(Type format)
        {
            return this.GetDataPresent(format.ToString());
        }

        public string[] GetFormats(bool autoConvert)
        {
            return new string[] { DRAG_FORMAT };
        }

        public string[] GetFormats()
        {
            return this.GetFormats(true);
        }

        public void SetData(string format, bool autoConvert, object data)
        {
            return;
        }

        public void SetData(string format, object data)
        {
            return;
        }

        public void SetData(Type format, object data)
        {
            return;
        }

        public void SetData(object data)
        {
            return;
        }
    }
}
