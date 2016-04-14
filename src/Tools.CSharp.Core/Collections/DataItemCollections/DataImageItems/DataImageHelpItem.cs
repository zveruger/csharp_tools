using System.Drawing;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Collections
{
    public class DataImageHelpItem<TData> : DataImageItem<TData>
    {
        #region private
        private readonly string _help;
        private readonly string _description;
        #endregion
        public DataImageHelpItem(TData data, string name, Icon icon, Size imageSize, string help = "", string description = "")
            : this(data, name, icon.ToBitmap(imageSize), help, description)
        { }
        public DataImageHelpItem(TData data, string name, Icon icon, int imageWidth, int imageHeigth, string help = "", string description = "")
            : this(data, name, icon.ToBitmap(imageWidth, imageHeigth), help, description)
        { }
        public DataImageHelpItem(TData data, string name, Image image, string help = "", string description = "")
            : base(data, name, image)
        {
            _help = help;
            _description = description;
        }

        //---------------------------------------------------------------------
        public string Help
        {
            get { return _help; }
        }
        public string Description
        {
            get { return _description; }
        }
        //---------------------------------------------------------------------
    }
}