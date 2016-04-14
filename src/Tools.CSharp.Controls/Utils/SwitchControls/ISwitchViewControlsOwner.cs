using System.Windows.Forms;

namespace Tools.CSharp.SwitchControls
{
    public interface ISwitchViewControlsOwner
    {
        //---------------------------------------------------------------------
        Control.ControlCollection GetControls();
        //---------------------------------------------------------------------
    }
}
