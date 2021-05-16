using PharmApp.src.Requests;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PharmApp.src.GUI
{
    public partial class LoadingForm : Form
    {
        public LoadingForm()
        {
            InitializeComponent();

            backgroundWorker1.DoWork += (object sender, DoWorkEventArgs e) => {
                if (Properties.Settings.Default.CLEAN_RMS)
                {
                    SQLQueryer.CleanRMS1();
                    backgroundWorker1.ReportProgress(1);
                    SQLQueryer.CleanRMS2();
                    backgroundWorker1.ReportProgress(1);
                    SQLQueryer.CleanRMS3();
                    backgroundWorker1.ReportProgress(1);
                }
                SQLQueryer.PopulateRequests();
            };

            backgroundWorker1.ProgressChanged += new ProgressChangedEventHandler(ProgressChangedHandler);
            backgroundWorker1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(WorkerCompleteHandler);
            backgroundWorker1.WorkerReportsProgress = true;
            this.Show();

            backgroundWorker1.RunWorkerAsync();
        }

       private void ProgressChangedHandler(object sender, ProgressChangedEventArgs e)
       {
            progressBar1.PerformStep();
            if (progressBar1.Value == progressBar1.Maximum)
            {
                label1.Text = "Populating requests";
            }
       }

        private void WorkerCompleteHandler(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Hide();

            RequestManager requester = RequestManager.Get();
            requester.GenerateRequestEmail(true);
        }
    }
}
