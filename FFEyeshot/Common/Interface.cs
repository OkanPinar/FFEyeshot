using System;
using System.Collections.Generic;
using System.Text;

namespace FFEyeshot.Common
{
    public interface ITransformable
    {
        void NotifyTransformation(object sender, TransformationEventArgs data);
    }

    public interface INotifyEntityChanged
    {
        event EntityChangingEventHandler OnEntityChanging;

        void NotifyEntityChanged();
    }
}
