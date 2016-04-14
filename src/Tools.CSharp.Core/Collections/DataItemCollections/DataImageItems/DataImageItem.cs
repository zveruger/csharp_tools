using System.Drawing;
using Tools.CSharp.Extensions;

namespace Tools.CSharp.Collections
{
    public class DataImageItem<TData> : DataItem<TData>
    {
        #region private
        private readonly Image _image;
        #endregion
        public DataImageItem(TData data, string name, Icon icon, Size imageSize)
            : this(data, name, icon.ToBitmap(imageSize))
        { }
        public DataImageItem(TData data, string name, Icon icon, int imageWidth, int imageHeigth)
           : this(data, name, icon.ToBitmap(imageWidth, imageHeigth))
        { }
        public DataImageItem(TData data, string name, Image image)
            : base(data, name)
        {
            _image = image;
        }

        //---------------------------------------------------------------------
        public Image Image
        {
            get { return _image; }
        }
        //---------------------------------------------------------------------
    }
}
