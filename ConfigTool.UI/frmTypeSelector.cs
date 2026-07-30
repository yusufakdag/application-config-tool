using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Luhmann.Kiosk.Tools.ConfigTool
{
    public partial class frmTypeSelector : Form
    {
        internal Type SelectedType { get; set; }

        internal frmTypeSelector(List<Type> TypeList)
        {
            InitializeComponent();

            lstModels.DataSource = TypeList;
            lstModels.DisplayMember = "FullName";
      
        }

        private void lstModels_DrawItem(object sender, DrawItemEventArgs e)
        {
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Hide();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
            this.Hide();
        }

        private void lstModels_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstModels.SelectedIndex != -1)
            {
                this.SelectedType = ((Type)lstModels.SelectedItem);
                btnOk.Enabled = true;
            }
        }

        private void lstModels_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            lstModels.ItemHeight = 25;
        }
    }
}