using System;
using System.Collections.Generic;
using System.Text;

namespace FFEyeshot.Common
{
    public interface INotifyTransformation
    {
        event TransformingEventHandler OnTransforming;

        void NotifyTransformation(object sender, TransformingEventArgs e);
    }

    public interface INotifyEntityChanged
    {
        event EntityChangedEventHandler OnEntityChanged;

    }
}
