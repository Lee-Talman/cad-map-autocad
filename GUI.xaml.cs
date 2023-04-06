using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;

namespace CAD_MAP_AutoCAD_Plugin
{
    public partial class GUI : Window
    {
        public SqlConnection sqlConnection = new SqlConnection(null);

        public GUI()
        {
            InitializeComponent();
        }

        public void btnConnectDB_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Get the connection string from the TextBox control
                string connectionString = inputConnectionString.Text;

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

        public void btnConfigureDB_Click(object sender, RoutedEventArgs e)
        {
                System.Windows.MessageBox.Show("To Do: Set Up Automatic SQL DB configuration");
        }

        public void btnClearDB_Click(object sender, RoutedEventArgs e)
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

        private void btnUploadFile_Click(object sender, RoutedEventArgs e)
        {
            DataTable csvTable = CADMAP.ImportValidationFile();
            // Display the CSV data in a DataGrid control
            if (csvTable == null)
            {
                System.Windows.MessageBox.Show("There was an error importing the file.");
            }
            else
            {
                ImportDataGrid.ItemsSource = csvTable.DefaultView;
                System.Windows.MessageBox.Show("Import Successful!");
            }
        }

        private void inputConnectionString_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void btnStartCADMAP_Click(object sender, RoutedEventArgs e)
        {
            string connectionString = inputConnectionString.Text;
            if (chkLines.IsChecked == true)
            {
                CADMAP.ImportAllLines(connectionString);
            }
            if (chkPolylines.IsChecked == true)
            {
                CADMAP.ImportAllPolyLines(connectionString);
            }
            if (chkMTexts.IsChecked == true)
            {
                CADMAP.ImportAllMTexts(connectionString);
            }
            if (chkBlocks.IsChecked == true)
            {
                CADMAP.ImportAllBlocks(connectionString);
            }
            if (chkGroups.IsChecked == true)
            {
                CADMAP.ImportAllGroups(connectionString);
            }
        }

        private void btnOverlapTest_Click(object sender, RoutedEventArgs e)
        {
            CADMAP.MTextExistsWithinBlock();
        }

        private void chkGroups_Checked(object sender, RoutedEventArgs e)
        {
            string connectionString = inputConnectionString.Text;
            CADMAP.ImportAllGroups(connectionString);
        }
    }
}
