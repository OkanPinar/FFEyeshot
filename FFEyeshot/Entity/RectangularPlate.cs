using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFEyeshot.Entity
{
    public class RectangularPlate
    {
        public devDept.Eyeshot.Entities.Solid View { get; set; }

        private double _width;

        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private double _height;

        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        private double _thickness;

        public double Thickness
        {
            get { return _thickness; }
            set { _thickness = value; }
        }

        private PointT[] _points = new PointT[4];

        public RectangularPlate()
        {
             
        }

        public RectangularPlate(double width, double height, double thickness)
        {
            _width = width;
            _height = height;
            _thickness = thickness;
            this.InitializeView();
        }

        private void InitializePoints()
        {
            double halfWidth = _width * 0.5;
            double halfHeight = _height * 0.5;
            _points[0] = new PointT(-halfWidth, 0, -halfHeight);
            _points[1] = new PointT(-halfWidth, 0, halfHeight);
            _points[2] = new PointT(halfWidth, 0, halfHeight);
            _points[3] = new PointT(halfWidth, 0, -halfHeight);
            foreach (var point in _points)
            {
                point.OnTransformed += NotifyPointIsChanged;
            }
        }

        private void NotifyPointIsChanged(object sender, Common.TransformingEventArgs e)
        {
            var pnts = this._points.ToList<devDept.Geometry.Point3D>();
            pnts.Add(pnts[0]);

            var LinearPath = new devDept.Eyeshot.Entities.LinearPath(pnts);

            var region = new devDept.Eyeshot.Entities.Region(LinearPath);

            var temp = region.ExtrudeAsSolid(devDept.Geometry.Vector3D.AxisZ * this.Thickness, 0.01);

            
        }

        public void InitializeView()
        {
            var pnts = this._points.ToList<devDept.Geometry.Point3D>();
            pnts.Add(pnts[0]);

            var LinearPath = new devDept.Eyeshot.Entities.LinearPath(pnts);

            var region = new devDept.Eyeshot.Entities.Region(LinearPath);

            this.View = region.ExtrudeAsSolid(devDept.Geometry.Vector3D.AxisZ * this.Thickness, 0.01);
        }

        public void RefreshView()
        {

        }
    }
}
