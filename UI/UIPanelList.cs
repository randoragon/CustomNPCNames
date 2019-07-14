using Terraria.UI;
using Terraria.ModLoader;
using Terraria.GameContent.UI.Elements;
using System.Collections.Generic;
using System;

namespace CustomNPCNames.UI
{ 
    class UIPanelList : UIElement
    {
        protected UIScrollbar scrollBar;
        protected List<UIPanel> panels;
        public bool autoSort;
        public Comparison<UIPanel> ComparePanel { get; set; }
        protected readonly int panelHeight;

        public UIPanelList(int panelHeight = 30)
        {
            this.panelHeight = panelHeight;
            OverflowHidden = true;

            scrollBar = new UIScrollbar();
            scrollBar.HAlign = 1f;
            scrollBar.Left.Set(0, 1);
            scrollBar.Top.Set(0, 0);
            //scrollBar.Width.Set(20, 0);
            scrollBar.Height.Set(0, 1);
            Append(scrollBar);

            panels = new List<UIPanel>();

            ComparePanel = delegate(UIPanel p1, UIPanel p2) { return 0; };
        }

        public virtual void Add(UIPanel panel)
        {
            panels.Add(panel);
            panel.Left.Set(0, 0);
            panel.Top.Set(panels.Count * panelHeight, 0);
            panel.Height.Set(panelHeight, 0);
            Append(panel);
            if (autoSort) { Sort(); }
        }

        public virtual void Sort(Comparison<UIPanel> sortMethod)
        {
            panels.Sort(sortMethod);
        }

        public virtual void Sort()
        {
            panels.Sort(ComparePanel);
        }
    }
}