using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using devDept.Eyeshot.Entities;
using devDept.Geometry;

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

    public class PolygonalPlate: Solid, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public object Parent { get; set; }

        public PointTGroup Points { get; set; } = new PointTGroup();

        private double _thickness;

        public double Thickness
        {
            get { return _thickness; }
            set {
                if (value != _thickness)
                {
                    _thickness = value;
                    NotifyThicknessChanged();
                }                
            }
        }

        public AreaPosition Position { get; private set; } = new AreaPosition();

        public PolygonalPlate()
        {

        }

        public void InitPoints()
        {
            foreach (var point in Points)
            {
                point.OnTransformed += NotifyReferancePointChanged;
            }
        }

        private void NotifyReferancePointChanged(object sender, Common.TransformingEventArgs e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            var tempPoint = new List<PointT>(Points);
            tempPoint.Add(Points[0]);
            var path = new LinearPath(tempPoint.ToArray());
            var reg = new Region(path);
            Solid plate = reg.ExtrudeAsSolid<Solid>(_thickness, 0.1);
            this.Portions.Clear();

            foreach (var port in plate.Portions)
            {
                Portions.Add(port);
            }
            Regen(0);
            this.UpdateBoundingBox(new devDept.Eyeshot.TraversalParams());
        }

        private void NotifyThicknessChanged()
        {
            RefreshView();
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Thickness"));
        }

        public PolygonalPlate(double thickness, params PointT[] pnts)
        {
            Points = new PointTGroup(pnts);
            _thickness = thickness;
        }

        public static PolygonalPlate Create(double thickness, Vector3D Normal, params PointT[] points)
        {
            var tempPoint = points.ToList();
            tempPoint.Add(points[0]);
            var path = new LinearPath(tempPoint.ToArray());
            var reg = new Region(path);
            PolygonalPlate plate = reg.ExtrudeAsSolid<PolygonalPlate>(Normal * thickness, 0.1);
            plate.Points = new PointTGroup(points);
            plate.Thickness = thickness;
            plate.Position.V3 = Normal;
            plate.InitPoints();
            return plate;
        }
    }
}
