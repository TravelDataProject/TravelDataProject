using DataAccess;
using Models;
using Models.Extensions;
using System;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;

namespace DesktopApp
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker bulkCopyBackgroundWorkerBulk;
        private DaoMerchant daoMerchant;
        private DaoProduct daoProduct;
        private BackgroundWorker deserialisationBackgroundWorker;
        private cafProductFeed deserializedModel;
        private System.Diagnostics.Stopwatch watch;

        public MainWindow()
        {
            InitializeComponent();

            daoMerchant = DaoMerchant.Instance;
            daoProduct = DaoProduct.Instance;

            deserialisationBackgroundWorker = new BackgroundWorker();
            deserialisationBackgroundWorker.DoWork += DeserialisationBackgroundWorker_DoWork;
            deserialisationBackgroundWorker.RunWorkerCompleted += DeserialisationBackgroundWorker_RunWorkerCompleted;

            bulkCopyBackgroundWorkerBulk = new BackgroundWorker();
            bulkCopyBackgroundWorkerBulk.DoWork += BulkCopyBackgroundWorkerBulk_DoWork;
            bulkCopyBackgroundWorkerBulk.RunWorkerCompleted += BulkCopyBackgroundWorkerBulk_RunWorkerCompleted;
        }

        private static string GetXml(string url)
        {
            using (XmlReader xr = XmlReader.Create(url, new XmlReaderSettings() { IgnoreWhitespace = true, DtdProcessing = DtdProcessing.Parse }))
            {
                using (StringWriter sw = new StringWriter())
                {
                    using (XmlWriter xw = XmlWriter.Create(sw))
                    {
                        xw.WriteNode(xr, false);
                    }
                    return sw.ToString();
                }
            }
        }

        private void DeserialisationBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            string FileName = "voyage.xml";
            string DesktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string FullPath = Path.Combine(DesktopPath, FileName);
            string xmlContent = new StringBuilder(File.ReadAllText(FullPath))
                                        .Replace("-<cafProductFeed", "<cafProductFeed")
                                        .Replace("-<datafeed", "<datafeed")
                                        .Replace("-<prod", "<prod")
                                        .Replace("-<price", "<price")
                                        .Replace("-<text", "<text")
                                        .Replace("-<uri", "<uri")
                                        .Replace("-<vertical", "<vertical")
                                        .Replace("&", "&amp;").ToString();
            XmlSerializer ser = new XmlSerializer(typeof(cafProductFeed));
            using (StringReader sr = new StringReader(xmlContent))
            {
                deserializedModel = (cafProductFeed)ser.Deserialize(sr);
            }
        }

        private void DeserialisationBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            System.Windows.Forms.MessageBox.Show("Desérialisation complète", "Information..", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnDeserialisation.IsEnabled = true;
            btnInsertionMasse.IsEnabled = true;
            btnParcourir.IsEnabled = true;
            tblockState.Text = "Desérialisation terminée.";
            FreeMemory();
        }

        private void BulkCopyBackgroundWorkerBulk_DoWork(object sender, DoWorkEventArgs e)
        {
            BulkInsert(deserializedModel);
        }

        private void BulkCopyBackgroundWorkerBulk_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            watch.Stop();
            System.Windows.Forms.MessageBox.Show("Nombre d'enregistrements : " + deserializedModel.dataFeed.prod.Count +
                                                 "\nTemps passé en millisecondes : " + watch.ElapsedMilliseconds +
                                                 "\nEnregistrement par millisecondes : " + ((watch.ElapsedMilliseconds > deserializedModel.dataFeed.prod.Count) ? (watch.ElapsedMilliseconds / deserializedModel.dataFeed.prod.Count).ToString("N2") : (deserializedModel.dataFeed.prod.Count / watch.ElapsedMilliseconds).ToString("N2")), "Information..", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnDeserialisation.IsEnabled = true;
            btnInsertionMasse.IsEnabled = true;
            btnParcourir.IsEnabled = true;
            tblockState.Text = "Insértion en Masse terminée.";
            FreeMemory();
        }

        private void BulkInsert(cafProductFeed model)
        {
            DataTable dataTable = model.dataFeed.prod.AsDataTable();
            SqlTransaction transaction = null;
            try
            {
                using (transaction = ConnectionAccess.SetTransaction())
                {
                    int merchantKeyId = daoMerchant.Insert(model.dataFeed.merchantElement, transaction);
                    daoProduct.BulkCopy(dataTable, merchantKeyId, transaction);
                    transaction.Commit();
                }
            }
            catch (Exception Ex)
            {
                transaction.Rollback();
                System.Windows.Forms.MessageBox.Show("Erreur : " + Ex.Message, "Erreur..", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FreeMemory()
        {
            GC.Collect();
            GC.SuppressFinalize(this);
            GC.WaitForPendingFinalizers();
        }

        private void btnInsertionMasse_Click(object sender, RoutedEventArgs e)
        {
            FreeMemory();
            watch = System.Diagnostics.Stopwatch.StartNew();
            btnDeserialisation.IsEnabled = false;
            btnInsertionMasse.IsEnabled = false;
            btnParcourir.IsEnabled = false;
            tblockState.Text = "Insértion en Masse en cours..";
            bulkCopyBackgroundWorkerBulk.RunWorkerAsync();
        }

        private void btnDeserialisation_Click(object sender, RoutedEventArgs e)
        {
            FreeMemory();
            tblockState.Text = "Desérialisation en cours..";
            btnDeserialisation.IsEnabled = false;
            btnInsertionMasse.IsEnabled = false;
            btnParcourir.IsEnabled = false;
            deserialisationBackgroundWorker.RunWorkerAsync();
        }

        private void Parcourir_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            DialogResult result = openFileDialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK) // Test result.
            {
                if (Path.GetExtension(openFileDialog.FileName).Equals(".xml"))
                {
                    btnDeserialisation.IsEnabled = true;
                    btnInsertionMasse.IsEnabled = false;
                    tblockState.Text = "Fichier chargé.";
                }
                else
                {
                    btnDeserialisation.IsEnabled = false;
                    btnInsertionMasse.IsEnabled = false;
                    tblockState.Text = "Veuillez sélectionner un fichier au format XML.";
                }
            }
        }
    }
}