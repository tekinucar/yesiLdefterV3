using System.Windows.Forms;

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

            this.KeyPreview = true;

        }
    }
}
