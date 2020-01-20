using System;
using System.Collections.Generic;
using System.Text;

namespace FFEyeshot.Common
{
    public interface INotifyTransformation
    {
        event TransformingEventHandler OnTransforming;
    }

    public interface INotifyEntityChanged
    {
        event EntityChangingEventHandler OnEntityChanging;

        void NotifyEntityChanged();
    }
}
