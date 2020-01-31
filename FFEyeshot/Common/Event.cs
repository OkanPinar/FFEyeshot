using System;
using System.Collections.Generic;
using System.Text;

namespace FFEyeshot.Common
{
    public class TransformingEventArgs
    {
        public devDept.Geometry.Transformation TData { get; private set; }

        public TransformingEventArgs(devDept.Geometry.Transformation tData)
        {
            this.TData = tData;
        }
    }


    public class EntityChangedEventArgs
    {
        //devDept.Eyeshot.Entities.Entity View { get; set; }

        public EntityChangedEventArgs()
        {

        }
    }

    /// <summary>
    /// Notification for transformation of an entity
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TransformingEventHandler(object sender, TransformingEventArgs e);

    public delegate void TransformedEventHandler(object sender, TransformingEventArgs e);

    /// <summary>
    /// Notification for changing of an entity
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>

    public delegate void EntityChangedEventHandler(object sender, EntityChangedEventArgs e);
}
