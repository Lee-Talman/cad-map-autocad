using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Group = Autodesk.AutoCAD.DatabaseServices.Group;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using DataTable = System.Data.DataTable;
using Exception = System.Exception;

namespace CAD_MAP_AutoCAD_Plugin
{
    public partial class GUI : Window
    {
        public SqlConnection sqlConnection = new SqlConnection(null);
        public string connectionString = "";

        public GUI()
        {
            InitializeComponent();
        }

        public void BtnConnectDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the connection string from the TextBox control
                connectionString = inputConnectionString.Text;

                // Create a SqlConnection object with the connection string
                sqlConnection = new SqlConnection(connectionString);

                // Open the connection
                sqlConnection.Open();

                // Display a message indicating that the connection was successful
                System.Windows.MessageBox.Show("Connection successful!");

                // Close the connection
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                // Display an error message if the connection failed
                System.Windows.MessageBox.Show("Connection failed: " + ex.Message);
            }
        }

        public void BtnConfigureDB_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.MessageBox.Show("To Do: Set Up Automatic SQL DB configuration");
        }

        public void BtnClearDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the connection string from the TextBox control
                string connectionString = inputConnectionString.Text;

                // Create a SqlConnection object with the connection string
                using (SqlConnection sqlConnection = new SqlConnection(connectionString))
                {
                    // Open the connection
                    sqlConnection.Open();

                    // Get the names of all tables in the database
                    DataTable tables = sqlConnection.GetSchema("Tables");
                    foreach (DataRow table in tables.Rows)
                    {
                        // Get the name of the current table
                        string tableName = (string)table[2];

                        // Truncate the table (i.e. delete all data)
                        using (SqlCommand command = new SqlCommand("TRUNCATE TABLE " + tableName, sqlConnection))
                        {
                            command.ExecuteNonQuery();
                        }
                    }

                    // Display a message indicating that all tables were emptied successfully
                    System.Windows.MessageBox.Show("All tables were emptied successfully!");

                    // Close the connection
                    sqlConnection.Close();
                }
            }
            catch (Exception ex)
            {
                // Display an error message if the operation failed
                System.Windows.MessageBox.Show("Operation failed: " + ex.Message);
            }
        }

        private void BtnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            DataTable csvTable = CADMAP.ImportValidationFile();
            // Display the CSV data in a DataGrid control
            if (csvTable == null)
            {
                System.Windows.MessageBox.Show("There was an error importing the file.");
            }
            else
            {
                System.Windows.MessageBox.Show("Import Successful!");
            }
        }

        private void BtnStartCADMAP_Click(object sender, RoutedEventArgs e)
        {
        Document doc = Application.DocumentManager.MdiActiveDocument;
        Editor ed = doc.Editor;
        Database db = doc.Database;

        bool selectAll = true;
            if (RadioBtnImportAll.IsChecked == true)
            {
                selectAll = true;
            }
            else if (RadioBtnImportSelected.IsChecked == true)
            {
                selectAll = false;
            }

            string connectionString = inputConnectionString.Text;
            if (ChkBoxLines.IsChecked == true)
            {
                CADMAP.ImportAllLines(connectionString, selectAll, doc, ed, db);
            }
            if (ChkBoxPolyLines.IsChecked == true)
            {
                CADMAP.ImportAllPolyLines(connectionString, selectAll, doc, ed, db);
            }
            if (ChkBoxTexts.IsChecked == true)
            {
                //CADMAP.ImportAllTexts(connectionString, selectAll, doc, ed, db);
            }
            if (ChkBoxMTexts.IsChecked == true)
            {
                CADMAP.ImportAllMTexts(connectionString, selectAll, doc, ed, db);
            }
            if (ChkBoxBlocks.IsChecked == true)
            {
                CADMAP.ImportAllBlocks(connectionString, selectAll, doc, ed, db);
            }
            if (ChkBoxGroups.IsChecked == true)
            {
                CADMAP.ImportAllGroups(connectionString, selectAll, doc, ed, db);
            }
        }

        private void btnOverlapTest_Click(object sender, RoutedEventArgs e)
        {
            CADMAP.MTextExistsWithinBlock();
        }
    }
}
