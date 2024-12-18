using DevExpress.Utils.DragDrop;
using System;
using System.Windows.Forms;
using Tkn_Events;
using Tkn_ToolBox;

namespace YesiLdefter
{
    public partial class ms_Form : Form
    {
        //tEvents ev = new tEvents();
        public ms_Form()
        {
            InitializeComponent();
            //this.Load += new System.EventHandler(ev.myForm_Load);
            //this.Shown += new System.EventHandler(ev.myForm_Shown);
            this.Shown += new System.EventHandler(this.ms_Form_Shown);
            this.KeyPreview = true;
        }

        private void ms_Form_Shown(object sender, EventArgs e)
        {
            
            tToolBox t = new tToolBox();
            
            /// DragDrop viewler var mı kontrol et
            /// 
            Control cntrl1 = t.Find_Control(this, "DRAGDROP1");
            
            if (cntrl1 != null)
            {
                tEventsGrid evg = new tEventsGrid();

                ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).BeginInit();
                
                /// kaynak view hazırlanıyor
                Control viewControl1 = cntrl1.Controls[0];
                viewControl1.AllowDrop = false;

                object view1 = null;
                if (viewControl1.GetType().ToString() == "DevExpress.XtraGrid.GridControl")
                    view1 = ((DevExpress.XtraGrid.GridControl)viewControl1).MainView;

                ((DevExpress.XtraGrid.Views.Grid.GridView)view1).OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

                this.behaviorManager1.Attach<DragDropBehavior>(((DevExpress.XtraGrid.Views.Grid.GridView)view1), behavior =>
                {
                    behavior.DragDrop += evg.myBehavior_DragDropToGrid1;
                });

                /// Hedef view hazırlanıyor
                Control cntrl2 = t.Find_Control(this, "DRAGDROP2");
                Control viewControl2 = cntrl2.Controls[0];
                viewControl2.AllowDrop = false;

                object view2 = null;
                if (viewControl2.GetType().ToString() == "DevExpress.XtraGrid.GridControl")
                    view2 = ((DevExpress.XtraGrid.GridControl)viewControl2).MainView;

                ((DevExpress.XtraGrid.Views.Grid.GridView)view2).OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;

                this.behaviorManager1.Attach<DragDropBehavior>(((DevExpress.XtraGrid.Views.Grid.GridView)view2), behavior =>
                {
                    behavior.DragDrop += evg.myBehavior_DragDropToGrid2;
                });

                ((System.ComponentModel.ISupportInitialize)(this.behaviorManager1)).EndInit();
            }
            
        }

    }
}
