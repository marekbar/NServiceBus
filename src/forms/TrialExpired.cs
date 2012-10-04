﻿namespace NServiceBus.Forms
{
    using System;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Forms;

    public partial class TrialExpired : Form
    {
        public TrialExpired()
        {
            InitializeComponent();
        }

        public Func<TrialExpired, string, bool> ValidateLicenseFile { get; set; }

        public DateTime CurrentLicenseExpireDate { get; set; }

        public void DisplayError()
        {
            selectedFileExpirationDateLabel.Visible = false;
            errorMessageLabel.Visible = true;
            errorMessageLabel.Text = "The file you have selected is not valid.";
            Height = 363;
        }

        public void DisplayExpiredLicenseError(DateTime expirationDate)
        {
            errorMessageLabel.Text = "The license file you have selected is expired.";
            selectedFileExpirationDateLabel.Visible = errorMessageLabel.Visible = true;
            selectedFileExpirationDateLabel.Text = String.Format("Expiration Date: {0}", expirationDate.ToLocalTime().ToShortDateString());
            Height = 386;
        }

        private void requestLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("http://www.nservicebus.com/License.aspx");
        }

        private void browseButton_Click(object sender, EventArgs e)
        {
            using (var openDialog = new OpenFileDialog())
            {
                openDialog.InitializeLifetimeService();
                openDialog.Filter = "License files (*.xml)|*.xml|All files (*.*)|*.*";
                openDialog.Title = "Select License file";

                if (ShowDialogInSTA(openDialog) == DialogResult.OK)
                {
                    string licenseFileSelected = openDialog.FileName;

                    if (ValidateLicenseFile(this, licenseFileSelected))
                    {
                        DialogResult = DialogResult.OK;
                    }
                }
            }
        }

        //When calling a OpenFileDialog it needs to be done from an STA thread model otherwise it throws:
        //"Current thread must be set to single thread apartment (STA) mode before OLE calls can be made. Ensure that your Main function has STAThreadAttribute marked on it. This exception is only raised if a debugger is attached to the process."
        private static DialogResult ShowDialogInSTA(FileDialog dialog)
        {
            var result = DialogResult.Cancel;
            
            var thread = new Thread(() =>
                {
                    result = dialog.ShowDialog();
                });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            thread.Join();

            return result;
        }

        private void ignoreLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void notTopMostTimer_Tick(object sender, EventArgs e)
        {
            TopMost = false;
            notTopMostTimer.Enabled = false;
        }
    }
}
