using FFEyeshot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using devDept.Geometry;

namespace FFEyeshot.Entity
{
    public class SegmentT : devDept.Geometry.Segment3D, INotifyTransformation
    {
        public event TransformingEventHandler OnTransforming;

        public PointT P0{ get; set; }
        public PointT P1{ get; set; }

        public SegmentT()
        {

        }

        public SegmentT(Point3D P0, Point3D P1): base(P0, P1)
        {
            this.P0 = new PointT(P0);
            this.P1 = new PointT(P1);
            Initialize();
        }
               
        public SegmentT(PointT P0, PointT P1)
        {
            this.P0 = P0;
            this.P1 = P1;
            Initialize();
        }

        public void Initialize()
        {
            this.P0.OnTransforming += NotifyBasePointChanging;
            this.P1.OnTransforming += NotifyBasePointChanging;
        }

        private void NotifyBasePointChanging(object sender, TransformingEventArgs e)
        {
            try
            {
                PointT point = sender as PointT;
                if (point == P0)
                {

                }

                else if (point == P1)
                {

                }

                else
                {
                    throw new ArgumentException("There is something wrong on parameter");
                }
            }
            catch (Exception)
            {
                throw new InvalidCastException();
            }            
        }

    }
}
