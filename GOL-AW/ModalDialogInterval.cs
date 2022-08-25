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
    public partial class ModalDialogInterval : Form
    {
        public ModalDialogInterval()
        {
            InitializeComponent();
        }
        public int Number
        {
            get
            {
                return (int)numericUpDown1.Value;
            }
            set
            {
                numericUpDown1.Value = value;
            }
        }
    }
}
