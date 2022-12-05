using System.Drawing;
using System.Windows.Forms;

namespace AndroidSideloader
{
    public class Transparenter
    {
        public static void MakeTransparent(Control control, Graphics g)
        {
            Control parent = control.Parent;
            if (parent == null)
            {
                return;
            }

            Rectangle bounds = control.Bounds;
            Control.ControlCollection siblings = parent.Controls;
            int index = siblings.IndexOf(control);
            Bitmap behind = null;
            for (int i = siblings.Count - 1; i > index; i--)
            {
                Control c = siblings[i];
                if (!c.Bounds.IntersectsWith(bounds))
                {
                    continue;
                }

                if (behind == null)
                {
                    behind = new Bitmap(control.Parent.ClientSize.Width, control.Parent.ClientSize.Height);
                }

                c.DrawToBitmap(behind, c.Bounds);
            }
            if (behind == null)
            {
                return;
            }

            g.DrawImage(behind, control.ClientRectangle, bounds, GraphicsUnit.Pixel);
            behind.Dispose();
        }
    }
}