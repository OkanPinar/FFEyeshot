using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using devDept.Eyeshot;
using devDept.Geometry;

namespace FFEyeshot.ViewLay.ThreeD
{
    

    [ToolboxItem(true)]
    public partial class ViewPort3D: ViewportLayout, INotifyPropertyChanging, INotifyPropertyChanged
    {
        public event PropertyChangingEventHandler PropertyChanging;
        public event PropertyChangedEventHandler PropertyChanged;

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
    
        public void AddEntity(devDept.Eyeshot.Entities.Entity entity)
        {
            if (entity is Common.INotifyEntityChanged e)
            {
                e.OnEntityChanged += NotifyEntityChanged;
            }
            this.Entities.Add(entity);
        }

        public void RemoveEntity(devDept.Eyeshot.Entities.Entity entity)
        {
            if (entity is Common.INotifyEntityChanged e)
            {
                e.OnEntityChanged -= NotifyEntityChanged;
            }
            this.Entities.Remove(entity);
        }

        public void AddRangeEntity(IList<devDept.Eyeshot.Entities.Entity> entities)
        {
            var notifieds = entities.OfType<Common.INotifyEntityChanged>();
            foreach (var e in notifieds)
            {
                e.OnEntityChanged += NotifyEntityChanged;
            }
            this.Entities.AddRange(entities);
        }
        
        public void RemoveRange(IList<devDept.Eyeshot.Entities.Entity> entities)
        {
            var notifieds = entities.OfType<Common.INotifyEntityChanged>();
            foreach (var e in notifieds)
            {
                e.OnEntityChanged -= NotifyEntityChanged;
            }
            foreach (var e in entities)
            {
                this.Entities.Remove(e);
            }
        }

        private void NotifyEntityChanged(object sender, Common.EntityChangedEventArgs e)
        {
            var entity = sender as devDept.Eyeshot.Entities.Entity;
            entity.Regen(0.0);
            entity.RegenMode = devDept.Eyeshot.Entities.regenType.RegenAndCompile;
            entity.Compile(new CompileParams(this));
            Refresh();
            Invalidate();
            //throw new NotImplementedException();
        }

        protected override void DrawOverlay(DrawSceneParams data)
        {
            if (IsSnapEnable)
            {
                DrawOverlay_Snapping(data);
            }
            if (IsPickingEnable)
            {
                DrawOverlay_Picking(data);
            }
            if (IsDrawingEnable)
            {
                DrawOverlay_Drawing(data);
            }
            base.DrawOverlay(data);
        }

        public void PaintBackBufferForJigging()
        {
            this.PaintBackBuffer();
            this.SwapBuffers();
        }

    }
}
