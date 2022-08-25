using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOL_AW
{
    public partial class ModalDialog : Form
    {
        public ModalDialog()
        {
            InitializeComponent();
        }


        public int NumberWidth
        {
            get
            {
                return (int)numericUpDownWidth.Value;
            }
            set
            {
                numericUpDownWidth.Value = value;
            }
        }

        public int NumberHeight
        {
            get
            {
                return (int)numericUpDownHeight.Value;
            }
            set
            {
                numericUpDownHeight.Value = value;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }
    }
}
