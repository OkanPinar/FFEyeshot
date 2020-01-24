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

        private bool _SwapBufferRequired = false;

        private bool buttonPressed = false;

        protected override void OnMouseMove(MouseEventArgs e)
        {
            CurrentLocation = RenderContextUtility.ConvertPoint(e.GetPosition(this));
            CurrentIndex = GetEntityUnderMouseCursor(CurrentLocation);
            this.SetCurrentPoint();
            
            if (_isPickingEnable)
            {
                OnMouseMove_Picking(e);
            }

            if (_isSnapEnable)
            {
                OnMouseMove_Snapping(e);
            }

            if (_isDrawingEnable)
            {
                OnMouseMove_Drawing(e);
            }
            if (_SwapBufferRequired)
            {
                PaintBackBuffer();
                SwapBuffers();
            }
            _SwapBufferRequired = false;
            base.OnMouseMove(e);
        }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            if (_isPickingEnable)
            {
                OnMouseDown_Picking(e);
            }

            if (_isDrawingEnable)
            {
                OnMouseDown_Drawing(e);
            }

            base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_isPickingEnable)
            {
                OnMouseUp_Picking(e);
            }

            if (_isDrawingEnable)
            {
                OnMouseUp_Drawing(e);
            }

            base.OnMouseUp(e);
        }
    }
}
