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

        public void InitializeView()
        {
            double halfWidth = _width * 0.5;
            double halfHeight = _height * 0.5;
            var pnts = new devDept.Geometry.Point3D[5];
            pnts[0] = new devDept.Geometry.Point3D(-halfWidth, 0, -halfHeight);
            pnts[1] = new devDept.Geometry.Point3D(-halfWidth, 0,  halfHeight);
            pnts[2] = new devDept.Geometry.Point3D( halfWidth, 0,  halfHeight);
            pnts[3] = new devDept.Geometry.Point3D( halfWidth, 0, -halfHeight);
            pnts[4] = new devDept.Geometry.Point3D(-halfWidth, 0, -halfHeight);

            var LinearPath = new devDept.Eyeshot.Entities.LinearPath(pnts);

            var region = new devDept.Eyeshot.Entities.Region(LinearPath);

            this.View = region.ExtrudeAsSolid(devDept.Geometry.Vector3D.AxisZ * this.Thickness, 0.01);
        }

        public void RefreshView()
        {

        }
    }
}
