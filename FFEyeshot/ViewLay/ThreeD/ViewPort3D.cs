using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

using devDept.Eyeshot;

namespace FFEyeshot.ViewLay.ThreeD
{
    [ToolboxItem(true)]
    public partial class ViewPort3D: ViewportLayout
    {
        
        public ViewPort3D()
        {
            this.Unlock("ULTWPF-0216-6751N-0SDLH-THEY0");

            this.SelectionFilterMode = selectionFilterType.Entity;
            this.IsInFrustumMode = Camera.perspectiveFitType.Accurate;

            this.SelectionColor = System.Drawing.Color.GreenYellow;
        }

        public override void EndInit()
        {
            this.Camera.ProjectionMode = devDept.Graphics.projectionType.Orthographic;
            base.EndInit();
        }
    }
}
