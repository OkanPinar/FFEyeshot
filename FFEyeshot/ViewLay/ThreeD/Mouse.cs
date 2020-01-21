using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

using devDept.Geometry;
using devDept.Graphics;

namespace FFEyeshot.ViewLay.ThreeD
{
    public partial class ViewPort3D
    {
        public Point3D CurrentPoint;
        public System.Drawing.Point CurrentLocation { get; set; }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            CurrentLocation = RenderContextUtility.ConvertPoint(e.GetPosition(this));
            CurrentIndex = GetEntityUnderMouseCursor(CurrentLocation);

            if (_isPickingEnable)
            {
                OnMouseMove_Picking(e);
            }

            if (_isSnapEnable)
            {
                OnMouseMove_Snapping(e);
            }
        }
    }
}
