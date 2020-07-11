using System;
using System.Windows.Forms;

namespace OrganisePhotos.App
{
    public partial class DateInputForm : Form
    {
        public DateTime SelectedValue { get; private set; }

        public DateInputForm()
        {
            InitializeComponent();
            dateTimePicker.Value = SelectedValue = DateTime.Now.Date;
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            SelectedValue = dateTimePicker.Value;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
