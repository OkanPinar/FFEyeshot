using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows.Input;

using devDept.Geometry;

namespace FFEyeshot.ViewLay.ThreeD
{
    public interface IJigable
    {
        void NotifyMouseMove(object sender, MouseEventArgs e);
        void NotifyMouseDown(object sender, MouseButtonEventArgs e);
        void NotifyMouseUp(object sender, MouseButtonEventArgs e);
        void OnJigging(object sender, JiggingEventArgs e);
    }

    public class JiggingEventArgs
    {
        public devDept.Eyeshot.ViewportLayout.DrawSceneParams Data { get; set; }
        public JiggingEventArgs(devDept.Eyeshot.ViewportLayout.DrawSceneParams data)
        {
            this.Data = data;
        }
    }

    public delegate void JiggingEventHandler(object sender, JiggingEventArgs e);

    public partial class ViewPort3D
    {
        private event MouseEventHandler MouseMove_Drawing;
        private event MouseButtonEventHandler MouseUp_Drawing;
        private event MouseButtonEventHandler MouseDown_Drawing;
        private event JiggingEventHandler DrawOverlay_Jigging;

        private static IJigable DrawHelper;

        private bool _isDrawingEnable;

        public bool IsDrawingEnable
        {
            get { return _isDrawingEnable; }
            set { _isDrawingEnable = value; }
        }
        
        public Plane SketchPlane { get; set; } = Plane.XY;

        public void SetDrawing<T>() where T : IJigable, new()
        {
            this._isPickingEnable = false;
            T item = new T();
            MouseMove_Drawing += item.NotifyMouseMove;
            MouseUp_Drawing += item.NotifyMouseUp;
            MouseDown_Drawing += item.NotifyMouseDown;
            DrawOverlay_Jigging += item.OnJigging;
            DrawHelper = item;
            IsDrawingEnable = true;
        }

        public void ReleaseDrawing<T>() where T : IJigable
        {
            T item = (T)DrawHelper;
            MouseMove_Drawing -= item.NotifyMouseMove;
            MouseUp_Drawing -= item.NotifyMouseUp;
            MouseDown_Drawing -= item.NotifyMouseDown;
            DrawOverlay_Jigging -= item.OnJigging;
            DrawHelper = null;
            IsDrawingEnable = false;
            this._isPickingEnable = true;
        }

        protected void OnMouseMove_Drawing(MouseEventArgs e)
        {
            MouseMove_Drawing?.Invoke(this, e);
        }

        protected void OnMouseUp_Drawing(MouseButtonEventArgs e)
        {
            MouseUp_Drawing?.Invoke(this, e);
        }

        protected void OnMouseDown_Drawing(MouseButtonEventArgs e)
        {
            MouseDown_Drawing?.Invoke(this, e);
        }
    
        protected void DrawOverlay_Drawing(DrawSceneParams data)
        {
            DrawOverlay_Jigging?.Invoke(this, new JiggingEventArgs(data));
        }
    }
}
