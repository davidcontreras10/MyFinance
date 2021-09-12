using CurrencyUpdateService;
using System;
using System.IO;
using System.Windows.Forms;

namespace DesktopCurrencyUpdateApp
{
    public partial class ReadDataForm : Form
    {

        private readonly CurrencyExchangeLogic _currencyExchangeLogic;
        public ReadDataForm()
        {
            InitializeComponent();
            _currencyExchangeLogic = new CurrencyExchangeLogic();
        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog();
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            SubmitFileData();
        }

        private void OpenFileDialog()
        {
            if(openSourceFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtFileSource.Text = openSourceFileDialog.FileName;
            }
        }

        private void SubmitFileData()
        {
            if (string.IsNullOrEmpty(txtFileSource.Text))
            {
                MessageBox.Show("Emtpy Url", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            _currencyExchangeLogic.UpdateCurrencyExchangeDataByUrl(txtFileSource.Text);
            MessageBox.Show("Finished");
        }


    }
}
