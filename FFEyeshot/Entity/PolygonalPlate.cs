using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using devDept.Eyeshot.Entities;

namespace FFEyeshot.Entity
{
    public class AreaPosition
    {
        private Vector3D _V3;

        public Vector3D V3
        {
            get { return _V3; }
            set { _V3 = value; }
        }

    }

    public class PolygonalPlate: Solid
    {
        public List<PointT> Points { get; set; }

        private double _thickness;

        public double Thickness
        {
            get { return _thickness; }
            set {
                if (value != _thickness)
                {
                    _thickness = value;
                }                
            }
        }

        public AreaPosition position { get; set; }

        public PolygonalPlate()
        {

        }

        public PolygonalPlate(double thickness, params PointT[] pnts)
        {
            Points = pnts.ToList();
            _thickness = thickness;
        }

        public static PolygonalPlate Create(double thickness, params PointT[] points)
        {
            var tempPoint = points.ToList();
            tempPoint.Add(points[0]);
            var path = new LinearPath(tempPoint.ToArray());
            var reg = new Region(path);
            PolygonalPlate plate = reg.ExtrudeAsSolid<PolygonalPlate>(thickness, 0.1);
            plate.Points = points.ToList();
            plate.Thickness = thickness;
            return plate; 
        }
    }
}
