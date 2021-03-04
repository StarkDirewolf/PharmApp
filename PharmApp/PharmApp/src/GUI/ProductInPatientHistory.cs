using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using PharmApp.src.Product_Info;

namespace PharmApp.src.GUI
{
    partial class ProductInPatientHistory : Form
    {
        private Product product;

        public ProductInPatientHistory(Product product)
        {
            InitializeComponent();
            this.product = product;
            SetProduct(product);
        }

        public void SetProduct(Product product)
        {
            patientHistoryView.DataSource = SQLQueryer.GetProductPatientHistory(product);
            //this.Width = patientHistoryView.Width;
            this.Width = Enumerable.patientH.Aggregate(0, (acc, e) => patientHistoryView.Columns.
        }
    }
}
