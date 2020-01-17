using System;
using System.Collections.Generic;
using System.Text;

namespace FFEyeshot.Common
{
    public enum OnLCR
    {
        MIDDLE,
        LEFT,
        RIGHT
    }

    public enum OnRotation
    {
        TOP,
        FRONT,
        BACK,
        BELOW
    }

    public enum AtDepth
    {
        BEHIND,
        MIDDLE,
        FRONT
    }

    public enum ViewportPickState
    {
        Pick,
        Enclosed,
        Crossing
    };
}
