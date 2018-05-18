#define COLOR_DEBUGX

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ZedGraph;

namespace AiCableForce.graphic
{
    public class WaveGraph
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        public IEnumerable<GraphData> Ptls;

        public Color ForeColor = Color.Black;

        public float FontSize = 13;

        public float FontSizeTitle = 15;

        public float LineWidth = 1f;

        public Color ArrowColor = Color.Red;

        public bool XAxisSync = true;

        public bool Marklinked = true;    // 光标联动

        public bool Rangemode;          // 区间选择模式(力窗)

        public int PointSelectRule = 0;   // 选点规则 0-x轴最近 1-距离最近

        public bool AutoScaleX = false; // X轴自动比例

        public bool AutoScaleY = false; // Y轴自动比例

        public TMarkType MarkType = TMarkType.Line;

        private readonly ZedGraphControl _zedGraphCtr;

        private Guid curPaneId;

        private KeyboardHook hook = new KeyboardHook();

        private const int VK_SHIFT = 160;

        private const int VK_ESC = 27;

        private const int VK_LEFT = 37;

        private const int VK_RIGHT = 39;

        private static readonly Color RangeColor = Color.Red;

        private const int RangeAlpha = 100;

        private int startPt = -1;

        private IntPtr oleParent;

        private bool bFullScreen;

        private Color[] debugCor = new[] { Color.FromArgb(50, 0, 255, 0), Color.FromArgb(50, 0, 0, 255), Color.FromArgb(50, 255, 0, 0), Color.FromArgb(50, 255, 0, 255) };

        public delegate void OnRangeChangeHandler(Range r);
        public event OnRangeChangeHandler OnRangeChange;

        public WaveGraph(ZedGraphControl zedGraphCtr, bool rangeMode = false)
        {
            _zedGraphCtr = zedGraphCtr;
            Rangemode = rangeMode;
            oleParent = zedGraphCtr.Parent.Handle;
            hook.KeyDownEvent += hook_KeyDownEvent;
            zedGraphCtr.Parent.HandleDestroyed += Parent_HandleDestroyed;
            InitZedGraph();
        }

        public void Show(TGraphLayout layout = TGraphLayout.T_COL)
        {
            if (Ptls == null || !Ptls.Any()) return;
            UpdateZedGraphProperties();
            _zedGraphCtr.MasterPane.PaneList.Clear();
            _zedGraphCtr.MasterPane.GraphObjList.Clear();
            foreach (var graphData in Ptls)
            {
                var pane = new GraphPane();
                graphData.pane = pane;
                pane.Tag = graphData.id;
                pane.Title.Text = graphData.title;
                standardPane(pane);
                var scale = GetScale(graphData.ptls);

                pane.XAxis.Title.Text = string.Format(@"{0} {1}", graphData.xname, string.IsNullOrEmpty(graphData.xunit) ? "" : "(" + graphData.xunit + ")");
                pane.YAxis.Title.Text = string.Format(@"{0} {1}", graphData.yname, string.IsNullOrEmpty(graphData.yunit) ? "" : "(" + graphData.yunit + ")");
                if (!AutoScaleX)
                {
                    pane.XAxis.Scale.Min = scale.Xmin;
                    pane.XAxis.Scale.Max = scale.Xmax;
                }
                if (!AutoScaleY)
                {
                    pane.YAxis.Scale.Min = scale.Ymin;
                    pane.YAxis.Scale.Max = scale.Ymax;
                }

                graphData.lines.Clear();
                foreach (var pts in graphData.ptls)
                {
                    var line = pane.AddCurve("", pts, ForeColor, SymbolType.None);
                    line.Line.Width = LineWidth;
                    graphData.lines.Add(line);
                }
                _zedGraphCtr.MasterPane.Add(pane);
                _zedGraphCtr.ZoomOut(pane);

                graphData.mark.arrow = new ArrowObj(ArrowColor, 12F, pane.XAxis.Scale.Min, pane.YAxis.Scale.Min,
                    pane.XAxis.Scale.Min, pane.YAxis.Scale.Max)
                {
                    IsArrowHead = false,
                    Line = { Width = 1.0F }
                };
                if (MarkType == TMarkType.Triangle)
                {
                    graphData.mark.arrow.IsArrowHead = true;
                    graphData.mark.arrow.Size = 15;
                    graphData.mark.arrow.Line.Width = 2.0f;
                }
                pane.GraphObjList.Add(graphData.mark.arrow);
                updateLegend(graphData, 0);
                if (Rangemode) updateRange(graphData);

                pane.AxisChangeEvent += pane_AxisChangeEvent;
            }

            _zedGraphCtr.MasterPane.InnerPaneGap = 0f;
            using (Graphics g = _zedGraphCtr.CreateGraphics())
            {
                setLayout(_zedGraphCtr.MasterPane, g, layout, Ptls.Count());
                _zedGraphCtr.MasterPane.IsCommonScaleFactor = true;
            }
            _zedGraphCtr.AxisChange();
            _zedGraphCtr.Invalidate();
            _zedGraphCtr.Refresh();
        }

        void pane_AxisChangeEvent(GraphPane pane)
        {
        }

        void markPosChanged(GraphData graph)
        {
            if (Marklinked)
            {
                foreach (var gd in Ptls)
                {
                    if (gd.id == graph.id) continue;
                    updateLegend(gd, graph.mark.index);
                }
            }
        }

        void activePaneChanged()
        {
        }

        void updateLegend(GraphData graph, int index)
        {
            if (graph == null || !graph.lines.Any()) return;
            var line = graph.lines[0];
            if (line.NPts <= index || index < 0 || (PointSelectRule == 0 && !getScaleRange(graph).Contains(index))) return;
            graph.mark.index = index;
            if (MarkType == TMarkType.Line)
            {
                graph.mark.arrow.Location.X = line[index].X;
                graph.mark.arrow.Location.Y = graph.pane.YAxis.Scale.Min;
                graph.mark.arrow.Location.Height = graph.pane.YAxis.Scale.Max - graph.pane.YAxis.Scale.Min;
            }
            else if (MarkType == TMarkType.Triangle)
            {
                graph.mark.arrow.Location.X = line[index].X;
                graph.mark.arrow.Location.Y = line[index].Y + 1;
                graph.mark.arrow.Location.Height = -1;
            }
            line.Label.Text = string.Format(@"{0}
-------------
PNo.={1}
{2}={3:0.######} {4}
{5}={6:0.######} {7}
", graph.legend_fix, graph.mark.index, graph.xname, line[graph.mark.index].X, graph.xunit, graph.yname, line[graph.mark.index].Y, graph.yunit);
        }

        void updateRange(GraphData graph)
        {
            if (graph.lines.Count == 0) return;
            var range = graph.range;
            var pane = graph.pane;
            var curve = graph.lines[0];
            if (range.Left < 0 || range.Left >= curve.NPts) return;
            if (range.Right < 0 || range.Right >= curve.NPts) return;

            if (range.isEmpty())
            {
                if (range.Line != null)
                {
                    range.Line.Clear();
                    range.Line_low.Clear();
                }
            }
            else if (range.Line == null)
            {
                range.Line = pane.AddCurve("", new[] { curve[range.Left].X, curve[range.Right].X },
                    new[] { pane.YAxis.Scale.Max, pane.YAxis.Scale.Max }, RangeColor, SymbolType.None);
                range.Line.Line.Width = 10;
                range.Line.Line.Fill = new Fill(Color.FromArgb(RangeAlpha, RangeColor));
                range.Line_low = pane.AddCurve("", new[] { curve[range.Left].X, curve[range.Right].X },
                    new[] { pane.YAxis.Scale.Min, pane.YAxis.Scale.Min }, RangeColor, SymbolType.None);
                range.Line_low.Line.Width = 0;
                range.Line_low.Line.Fill = new Fill(Color.FromArgb(RangeAlpha, RangeColor));
            }
            else
            {
                range.Line.Points = new PointPairList(new[] { curve[range.Left].X, curve[range.Right].X },
                    new[] { pane.YAxis.Scale.Max, pane.YAxis.Scale.Max });
                range.Line_low.Points = new PointPairList(new[] { curve[range.Left].X, curve[range.Right].X },
                    new[] { pane.YAxis.Scale.Min, pane.YAxis.Scale.Min });
            }
        }

        public void SetRangeLeft(GraphData gp, int left)
        {
            if (gp == null) return;
            if (Rangemode)
            {
                gp.range.Left = left;
                updateRange(gp);
                _zedGraphCtr.Refresh();
            }
        }

        public void SetRangeRight(GraphData gp, int right)
        {
            if (gp == null) return;
            if (Rangemode)
            {
                gp.range.Right = right;
                updateRange(gp);
                _zedGraphCtr.Refresh();
            }
        }

        #region common
        private void InitZedGraph()
        {
            UpdateZedGraphProperties();
            _zedGraphCtr.MouseDownEvent += _zedGraphCtr_MouseDownEvent;
            _zedGraphCtr.MouseUpEvent += _zedGraphCtr_MouseUpEvent;
            _zedGraphCtr.MouseMoveEvent += _zedGraphCtr_MouseMoveEvent;
            _zedGraphCtr.MouseDoubleClick += _zedGraphCtr_MouseDoubleClick;
            _zedGraphCtr.Enter += _zedGraphCtr_Enter;
            _zedGraphCtr.Leave += _zedGraphCtr_Leave;
        }

        private void UpdateZedGraphProperties()
        {
            _zedGraphCtr.IsEnableHPan = false;          // 禁止水平移动
            _zedGraphCtr.IsEnableVPan = false;          // 禁止垂直移动
            _zedGraphCtr.IsEnableVZoom = false;         // 禁止垂直放缩
            if (Rangemode)
                _zedGraphCtr.IsEnableHZoom = false;     // 禁止水平放缩
            _zedGraphCtr.IsEnableWheelZoom = false;     // 禁止滚轮放缩 
            _zedGraphCtr.IsShowCopyMessage = false;     // 不显示复制信息
            _zedGraphCtr.IsShowHScrollBar = false;      // 不显示水平滚动条
            _zedGraphCtr.IsShowPointValues = false;     // 默认-不显示数据值
            _zedGraphCtr.IsShowVScrollBar = false;      // 不显示垂直滚动条
            _zedGraphCtr.IsSynchronizeXAxes = XAxisSync;    // 不强制X轴同步
            _zedGraphCtr.IsSynchronizeYAxes = false;    // 不强制Y轴同步
            _zedGraphCtr.IsShowContextMenu = false;
        }

        void _zedGraphCtr_Leave(object sender, EventArgs e)
        {
            hook.UnHook();
        }

        void _zedGraphCtr_Enter(object sender, EventArgs e)
        {
            hook.SetHook();
        }

        void _zedGraphCtr_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            bFullScreen = !bFullScreen;
            fullscreen(bFullScreen);
        }

        void fullscreen(bool flag)
        {
            if (flag)
            {
                _zedGraphCtr.Dock = DockStyle.None;
                _zedGraphCtr.Left = 0;
                _zedGraphCtr.Top = 0;
                _zedGraphCtr.Width = Screen.PrimaryScreen.Bounds.Width;
                _zedGraphCtr.Height = Screen.PrimaryScreen.Bounds.Height;
                SetParent(_zedGraphCtr.Handle, IntPtr.Zero);
            }
            else
            {
                SetParent(_zedGraphCtr.Handle, oleParent);
                _zedGraphCtr.Dock = DockStyle.Fill;
            }
        }

        bool _zedGraphCtr_MouseDownEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            startPt = -1;
            var pane = _zedGraphCtr.MasterPane.FindPane(e.Location);
            if (pane == null) return false;
            var graph = findGraphData(pane);
            if (graph == null || !graph.lines.Any()) return false;
            if (graph.id != curPaneId)
            {
                curPaneId = graph.id;
                activePaneChanged();
            }

            if (e.Button == MouseButtons.Left)
            {
                var iNearest = FindNearestPt(graph, e.Location);
                if (iNearest == -1) return false;
                updateLegend(graph, iNearest);
                markPosChanged(graph);
                if (Rangemode)
                {
                    startPt = iNearest;
                }
            }
            _zedGraphCtr.Refresh();
            return false;
        }

        bool _zedGraphCtr_MouseMoveEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            if(!Rangemode) return false;
            if (e.Button == MouseButtons.Left && startPt != -1)
            {
                var pane = _zedGraphCtr.MasterPane.FindPane(e.Location);
                if (pane == null) return false;
                var graph = findGraphData(pane);
                if (graph == null || !graph.lines.Any() || graph.id != curPaneId) return false;

                var iNearest = FindNearestPt(graph, e.Location);
                if (iNearest == -1) return false;
                if (iNearest != startPt)
                {
                    updateLegend(graph, iNearest);

                    graph.range.Left = startPt;
                    graph.range.Right = iNearest;
                    graph.range.Sort();
                    updateRange(graph);

                    if (OnRangeChange != null)
                        OnRangeChange(graph.range);
                    _zedGraphCtr.Refresh();
                }
            }
            return false;
        }

        bool _zedGraphCtr_MouseUpEvent(ZedGraphControl sender, MouseEventArgs e)
        {
            var pane = _zedGraphCtr.MasterPane.FindPane(e.Location);
            if (pane != null)
            {
                if (e.Button == MouseButtons.Right)
                {
                    zoomOut(pane);
                    var graph = findGraphData(pane);
                    if (graph != null && graph.id != curPaneId)
                        pane_AxisChangeEvent(pane);
                }
            }
            startPt = -1;
            _zedGraphCtr.Refresh();
            return false;
        }

        void hook_KeyDownEvent(object sender, KeyEventArgs e)
        {
            var graph = findGraphData(curPaneId);
            if (graph == null || !graph.lines.Any()) return;
            var key = e.KeyValue;
            if (key == VK_LEFT || key == VK_RIGHT)
            {
                var iNearest = graph.mark.index;
                if (iNearest == -1) return;
                if (iNearest > 0 && key == VK_LEFT)
                {
                    iNearest--;
                }
                else if (iNearest < graph.lines[0].NPts - 1 && key == VK_RIGHT)
                {
                    iNearest++;
                }
                updateLegend(graph, iNearest);
                markPosChanged(graph);
            }
            if (key == VK_ESC && bFullScreen)
            {
                fullscreen(false);
                bFullScreen = false;
            }
            _zedGraphCtr.Refresh();
        }

        void Parent_HandleDestroyed(object sender, EventArgs e)
        {
            _zedGraphCtr.Dispose();
        }

        private Scale GetScale(IEnumerable<PointPairList> ptls)
        {
            double xmax = double.MinValue;
            double xmin = double.MaxValue;
            double ymin = double.MaxValue;
            double ymax = double.MinValue;
            foreach (var pointList in ptls)
            {
                var xmx = pointList.Max(p => p.X);
                var xmn = pointList.Min(p => p.X);
                var ymx = pointList.Max(p => p.Y);
                var ymn = pointList.Min(p => p.Y);
                if (xmx > xmax) xmax = xmx;
                if (xmn < xmin) xmin = xmn;
                if (ymx > ymax) ymax = ymx;
                if (ymn < ymin) ymin = ymn;
            }

            return new Scale()
            {
                Xmax = xmax,
                Xmin = xmin,
                Ymax = ymax + Math.Abs(ymax) * 0.2,
                Ymin = ymin - Math.Abs(ymin) * 0.2
            };
        }

        void standardPane(GraphPane pane)
        {
            pane.Title.FontSpec.FontColor = ForeColor;
            pane.XAxis.Title.FontSpec.FontColor = ForeColor;
            pane.YAxis.Title.FontSpec.FontColor = ForeColor;
            pane.XAxis.Scale.FontSpec.FontColor = ForeColor;
            pane.YAxis.Scale.FontSpec.FontColor = ForeColor;
            pane.XAxis.Title.FontSpec.FontColor = ForeColor;
            pane.YAxis.Title.FontSpec.FontColor = ForeColor;
            pane.Title.FontSpec.Size = FontSizeTitle;
            pane.XAxis.Title.FontSpec.Size = FontSize;
            pane.YAxis.Title.FontSpec.Size = FontSize;
            pane.XAxis.Scale.FontSpec.Size = FontSize;
            pane.YAxis.Scale.FontSpec.Size = FontSize;
            pane.Legend.FontSpec.Size = FontSize;
            pane.XAxis.Title.FontSpec.Size = FontSize;
            pane.YAxis.Title.FontSpec.Size = FontSize;

            pane.XAxis.Scale.MaxAuto = AutoScaleX;
            pane.XAxis.Scale.MinAuto = AutoScaleX;
            pane.YAxis.Scale.MaxAuto = AutoScaleY;
            pane.YAxis.Scale.MinAuto = AutoScaleY;

            pane.XAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsVisible = true;
            pane.YAxis.MajorGrid.IsZeroLine = true;
            pane.Legend.IsHStack = false;
            pane.Legend.Position = LegendPos.Right;
            pane.Legend.IsShowLegendSymbols = false;
            pane.Legend.Border.IsVisible = false;
            pane.Legend.Gap = 0;
            pane.Legend.Fill = new Fill(Color.Transparent);
#if COLOR_DEBUG
            pane.Legend.Fill = new Fill(debugCor[1]);
            pane.Fill = new Fill(debugCor[0]);
            pane.Chart.Fill=new Fill(debugCor[2]);
#endif
            pane.Legend.IsVisible = true;
        }

        GraphData findGraphData(GraphPane pane)
        {
            return Ptls.FirstOrDefault(p => p.id == (pane.Tag as Guid?));
        }

        GraphData findGraphData(Guid id)
        {
            return Ptls.FirstOrDefault(p => p.id == id);
        }

        void setLayout(MasterPane masterPane, Graphics g, TGraphLayout layout, int nCount)
        {
            switch (layout)
            {
                case TGraphLayout.T_COL:
                    masterPane.SetLayout(g, PaneLayout.SingleColumn);
                    break;
                case TGraphLayout.T_SQUARE:
                    masterPane.SetLayout(g, PaneLayout.ForceSquare);
                    break;
                case TGraphLayout.T_2COL:
                    masterPane.SetLayout(g, nCount / 2, 2);
                    break;
                case TGraphLayout.T_211:
                    masterPane.SetLayout(g, false, new[] { 2, 1, 1 }, new[] { 3f, 5f, 4f });
                    break;
            }
        }

        int FindNearestPt(GraphData graph, Point location)
        {
            var iNearest = -1;
            if (PointSelectRule == 0)
            {
                double x, y;
                graph.pane.ReverseTransform(location, out x, out y);
                iNearest = GetNearestPtFromX(graph.lines[0], x);
            }
            else if (PointSelectRule == 1)
            {
                CurveItem nearestCurve;
                graph.pane.FindNearestPoint(location, graph.pane.CurveList, out nearestCurve, out iNearest);
            }
            return iNearest;
        }

        int GetNearestPtFromX(LineItem line, double x)
        {
            int res = -1;
            var len = line.Points.Count;
            for (int i = 1; i < len; i++)
            {
                if (x <= line.Points[i].X)
                {
                    res = line.Points[i].X - x > x - line.Points[i - 1].X ? i - 1 : i;
                    break;
                }
            }
            return res;
        }

        void zoomOut(GraphPane pane)
        {
            _zedGraphCtr.ZoomOutAll(pane);
            /*
            foreach (var tpane in _zedGraphCtr.MasterPane.PaneList)
            {
                _zedGraphCtr.ZoomOutAll(tpane);
            }
             */
            //_zedGraphCtr.RestoreScale(pane);
            /*
            var gp = findGraphData(pane);
            if (gp == null) return;
            var scale = GetScale(gp.ptls);
            pane.XAxis.Scale.Max = scale.Xmax;
            pane.XAxis.Scale.Min = scale.Xmin; 
             */
        }

        Range getScaleRange(GraphData graph)
        {
            var line = graph.lines[0];
            var len = line.Points.Count;
            var rag = new Range(0, len - 1);
            for (var i = 0; i < len; i++)
            {
                if (graph.pane.XAxis.Scale.Min <= line.Points[i].X)
                {
                    rag.Left = i;
                    break;
                }
            }
            for (var i = len - 1; i >= 0; i--)
            {
                if (graph.pane.XAxis.Scale.Max >= line.Points[i].X)
                {
                    rag.Right = i;
                    break;
                }
            }
            return rag;
        }
        #endregion
    }
}
