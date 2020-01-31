using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

        /*private void NotifyEntityChanged(object sender, Common.EntityChangedEventArgs e)
        {
            var entity = sender as devDept.Eyeshot.Entities.Entity;
            entity.Regen(0.0);
            entity.RegenMode = devDept.Eyeshot.Entities.regenType.RegenAndCompile;
            entity.Compile(new CompileParams(this));
            Refresh();
            Invalidate();
        }*/

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

        public void NotifyEntityChanged(object sender, Common.EntityChangedEventArgs e)
        {
            if (sender is devDept.Eyeshot.Entities.Entity entity)
            {
                entity.Regen(0.0);
                entity.Compile(new CompileParams(this));
                Entities.UpdateBoundingBox();
                Invalidate();
            }

            else if (sender is Block block)
            {
                var compileParam = new CompileParams(this);
                block.Compile(compileParam);
                var blockRefs = Entities.Where(item => item is devDept.Eyeshot.Entities.BlockReference);
                

                string blockName = Blocks.First(item => item.Value == block).Key;

                foreach (var blockRef in blockRefs)
                {
                    if (((devDept.Eyeshot.Entities.BlockReference)blockRef).BlockName == blockName)
                    {
                        blockRef.Compile(compileParam);
                        blockRef.UpdateBoundingBox(new TraversalParams(null, this));
                    }
                }
                
                Entities.UpdateBoundingBox();
                Camera.UpdateLocation();
                Invalidate();
            }

            else
            {
                throw new NotImplementedException();
            }
        }

    }
}
