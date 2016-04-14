using System;
using System.Globalization;

namespace Tools.CSharp.Uniques
{
    public class UniqueObjectsIdWithName : UniqueObjectsId
    {
        #region private
        private readonly string _leftAdditive;
        private readonly bool _isLeftAdditiveAvailable;
        //---------------------------------------------------------------------
        private readonly string _rightAdditive;
        private readonly bool _isRightAdditiveAvailable;
        //---------------------------------------------------------------------
        private readonly string _uniqueName;
        private readonly bool _isUniqueNameAvailable;
        //---------------------------------------------------------------------
        private bool _getUniqueIdOfName(string name, out int uniqueId)
        {
            uniqueId = -1;
            if (!string.IsNullOrWhiteSpace(name))
            {
                if (!_isUniqueNameAvailable || name.StartsWith(_uniqueName, StringComparison.OrdinalIgnoreCase))
                {
                    var idWidthAdditives = name.Substring(_uniqueName.Length, name.Length - _uniqueName.Length);

                    if (string.IsNullOrWhiteSpace(idWidthAdditives))
                    { uniqueId = 1; }
                    else
                    {
                        var idStr = idWidthAdditives;
                        if (_isLeftAdditiveAvailable || _isRightAdditiveAvailable)
                        {
                            if ((_isLeftAdditiveAvailable && idWidthAdditives.StartsWith(_leftAdditive, StringComparison.OrdinalIgnoreCase))
                                || (idWidthAdditives.EndsWith(_rightAdditive, StringComparison.OrdinalIgnoreCase)))
                            { idStr = idWidthAdditives.Substring(_leftAdditive.Length, idStr.Length - _leftAdditive.Length - _rightAdditive.Length); }
                        }

                        int.TryParse(idStr, out uniqueId);
                    }
                }
            }

            return uniqueId != -1;
        }
        #endregion
        public UniqueObjectsIdWithName(string uniqueName, string leftAdditive = " ", string rightAdditive = "")
        {
            _uniqueName = uniqueName;
            _isUniqueNameAvailable = !string.IsNullOrEmpty(_uniqueName);

            _leftAdditive = leftAdditive;
            _isLeftAdditiveAvailable = !string.IsNullOrEmpty(leftAdditive);

            _rightAdditive = rightAdditive;
            _isRightAdditiveAvailable = !string.IsNullOrEmpty(rightAdditive);
        }

        //---------------------------------------------------------------------
        public string CreateUniqueName()
        {
            var uniqueId = CreateUniqueId();
            return uniqueId == 1 ? _uniqueName : string.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3}", _uniqueName, _leftAdditive, uniqueId.ToString(), _rightAdditive);
        }
        //---------------------------------------------------------------------
        public bool AddIdWithName(string name)
        {
            int uniqueId;
            if (_getUniqueIdOfName(name, out uniqueId))
            {
                Add(uniqueId);
                return true;
            }
            return false;
        }
        public bool RemoveIdWithName(string name)
        {
            int uniqueId;
            if (_getUniqueIdOfName(name, out uniqueId))
            {
                Remove(uniqueId);
                return true;
            }
            return false;
        }
        //---------------------------------------------------------------------
    }
}