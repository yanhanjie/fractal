using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FractalAssignment
{
    public partial class Form2 : Form
    {
        public int Value
        {
            get
            {
                return int.Parse(textBox1.Text);
            }
        }

        public Form2()
        {
            InitializeComponent();
            this.Text = "Custom";
            this.AcceptButton = button1;
            Form1 f1 = new Form1();
            textBox1.Text = f1.J + "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            float floatVal;
            int intVal;
            bool check1 = float.TryParse(textBox1.Text, out floatVal);
            bool check2 = int.TryParse(textBox1.Text, out intVal);
            if (check1 == false)
            {
                MessageBox.Show("You must enter a number","Error");
            }
            else if (check2 == false)
            {
                MessageBox.Show("You must enter an integer", "Error");
            }
            else if (intVal < 0 || intVal > 200)
            {
                MessageBox.Show("You must enter a number between 0 and 200", "Error");
            }
            else
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
