using System.Windows.Forms;

namespace Konamiman.NestorMSX.Misc
{
    public partial class KeyTestForm : Form
    {
        public KeyTestForm()
        {
            InitializeComponent();
            textBox.PreviewKeyDown += textBox_PreviewKeyDown;
        }

        void textBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            textBox.AppendText(e.KeyCode + "\r\n");
        }

        private void btnClear_Click(object sender, System.EventArgs e)
        {
            textBox.Clear();
            textBox.Focus();
        }

        private void btnExit_Click(object sender, System.EventArgs e)
        {
            Application.Exit();
        }
    }
}
