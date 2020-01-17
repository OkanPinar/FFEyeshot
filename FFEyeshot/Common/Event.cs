using System;
using System.Collections.Generic;
using System.Text;

namespace FFEyeshot.Common
{
    public class TransformationEventArgs
    {
        public devDept.Geometry.Transformation Xform { get; private set; }

        public TransformationEventArgs(devDept.Geometry.Transformation xform)
        {
            this.Xform = Xform;
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
    public delegate void TransformationEventHandler(object sender, TransformationEventArgs e);

    /// <summary>
    /// Notification for changing of an entity
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EntityChangingEventHandler(object sender, EntityChangingEventArgs e);
}
