using System;
using System.Collections.Generic;
using System.Text;

namespace FFEyeshot.Common
{
    public class TransformingEventArgs
    {
        public devDept.Geometry.Transformation Xform { get; private set; }

        public TransformingEventArgs(devDept.Geometry.Transformation xform)
        {
            this.Xform = xform;
        }
    }

    public class TransformedEventArgs
    {
        public object Old { get; private set; }

        public TransformedEventArgs(object old)
        {
            this.Old = old;
        }
    }

    public class EntityChangingEventArgs
    {
        //devDept.Eyeshot.CompileParams data { get; set; }
        public EntityChangingEventArgs(devDept.Eyeshot.CompileParams data)
        {

        }
    }
    /// <summary>
    /// Notification for transformation of an entity
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void TransformingEventHandler(object sender, TransformingEventArgs e);

    public delegate void TransformedEventHandler(object sender, TransformedEventArgs e);

    /// <summary>
    /// Notification for changing of an entity
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EntityChangingEventHandler(object sender, EntityChangingEventArgs e);
}
