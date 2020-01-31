using devDept.Geometry;
using FFEyeshot.ViewLay.ThreeD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FFEyeshot.Helper
{
    public class ProfileHelper : ViewLay.ThreeD.IJigable
    {
        private static ProfileHelper _item = new ProfileHelper();

        public static ProfileHelper Item
        {
            get
            {
                return _item;
            }
        }

        public static devDept.Geometry.Plane DrawPlane = devDept.Geometry.Plane.XZ;

        public devDept.Geometry.Point3D StartPoint { get; set; }
        public devDept.Geometry.Point3D EndPoint { get; set; }

        private int _state = 0;

        public ProfileHelper()
        {

        }

        public void NotifyMouseMove(object sender, MouseEventArgs e)
        {
            if (_state == 1)
            {
                var VP = sender as ViewPort3D;
                VP.PaintBackBufferForJigging();
            }
        }

        public void NotifyMouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }

        public void NotifyMouseUp(object sender, MouseButtonEventArgs e)
        {
            var VP = sender as ViewPort3D;
            if (e.ChangedButton == MouseButton.Left)
            {
                if (_state == 0)
                {
                    this.StartPoint = DrawPlane.PointAt(DrawPlane.Project(VP.CurrentPoint));
                    _state++;
                }

                else if (_state == 1)
                {
                    EndPoint = DrawPlane.PointAt(DrawPlane.Project(VP.CurrentPoint));
                    VP.Entities.Add(new devDept.Eyeshot.Entities.Line(StartPoint, EndPoint));
                    VP.ReleaseDrawing<ProfileHelper>();
                }
            }

            else if (e.ChangedButton == MouseButton.Right)
            {
                VP.ReleaseDrawing<ProfileHelper>();
                VP.Entities.Clear();
            }
        }

        public void OnJigging(object sender, JiggingEventArgs e)
        {
            if (_state == 1)
            {
                var VP = sender as ViewPort3D;

                Point3D screenStart = VP.WorldToScreen(StartPoint);
                Point3D screenCurrent = VP.WorldToScreen(DrawPlane.PointAt(DrawPlane.Project(VP.CurrentPoint)));

                VP.renderContext.DrawLine(screenStart, screenCurrent);
            }
        }
    }
}
