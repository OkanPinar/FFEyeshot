using FFEyeshot.Common;
using System;

namespace FFEyeshot.Entity
{
    public class PointT :devDept.Geometry.Point3D, Common.ITransformable
    {
        public event TransformationEventHandler OnTransforming;

        public PointT(): base()
        {

        }

        public PointT(double x, double y, double z): base(x, y, z)
        {

        }

        public PointT(devDept.Geometry.Point3D another): base(another)
        {

        }

        public PointT(PointT another): base(another.X, another.Y, another.Z)
        {

        }  

        public void NotifyTransformation(object sender, TransformationEventArgs data)
        {
            var entity = sender as devDept.Eyeshot.Entities.Entity;

            entity.TransformBy(data.Xform);
        }
    }
}
