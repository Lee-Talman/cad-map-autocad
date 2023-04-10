using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;
using Group = Autodesk.AutoCAD.DatabaseServices.Group;

// This line is not mandatory, but improves loading performances
[assembly: CommandClass(typeof(CAD_MAP_AutoCAD_Plugin.CADMAP))]

namespace CAD_MAP_AutoCAD_Plugin
{

    class CADMAP
    {
        public System.Data.DataTable localCSV = new System.Data.DataTable();

        [CommandMethod("CADMAP")]
        public void InitializeGUI()
        {
            var gui = new GUI();
            gui.Show();
        }

        public static string GetPolyLineCoordinates(Polyline polyline)
        {
            var vertexCount = polyline.NumberOfVertices;
            Point2d coord;
            string coords = "";
            int i;
            for (i = 0; i <= vertexCount - 1; i++)
            {
                coord = polyline.GetPoint2dAt(i);
                coords += coord[0].ToString() + "," + coord[1].ToString();
                if ((i < vertexCount - 1))
                    coords += ",";
            }
            return coords;
        }

        public static void ImportAllLines(string connectionString)
        {
            // Get the Document and Editor object
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                TypedValue[] tv = new TypedValue[1];
                tv.SetValue(new TypedValue((int)DxfCode.Start, "LINE"), 0);
                SelectionFilter filter = new SelectionFilter(tv);

                PromptSelectionResult promptSelectionResult = ed.SelectAll(filter);
                // Check if an object is selected
                if (promptSelectionResult.Status == PromptStatus.OK)
                {
                    double startPtX = 0.0, startPtY = 0.0, endPtX = 0.0, endPtY = 0.0;
                    string layer = "", ltype = "", color = "";
                    double len = 0.0;
                    Line line = new Line();
                    SelectionSet selectionSet = promptSelectionResult.Value;

                    String sqlInsertQuery = @"INSERT INTO dbo.Lines (StartPtX, StartPtY, EndPtX, EndPtY, Layer, Color, Linetype, Length, Created)
                                       VALUES(@StartPtX, @StartPtY, @EndPtX, @EndPtY, @Layer, @Color, @Linetype, @Length, @Created)";

                    using (var sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        // Loop through selectionSet and insert into database, one LINE object at a time
                        foreach (SelectedObject selectedObj in selectionSet)
                        {
                            line = trans.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as Line;
                            startPtX = line.StartPoint.X;
                            startPtY = line.StartPoint.Y;
                            endPtX = line.EndPoint.X;
                            endPtY = line.EndPoint.Y;
                            layer = line.Layer;
                            ltype = line.Linetype;
                            color = line.Color.ToString();
                            len = line.Length;



                            SqlCommand cmd = new SqlCommand(sqlInsertQuery, sqlConnection);
                            cmd.Parameters.AddWithValue("@StartPtX", startPtX);
                            cmd.Parameters.AddWithValue("@StartPtY", startPtY);
                            cmd.Parameters.AddWithValue("@EndPtX", endPtX);
                            cmd.Parameters.AddWithValue("@EndPtY", endPtY);
                            cmd.Parameters.AddWithValue("@Layer", layer);
                            cmd.Parameters.AddWithValue("@Color", color);
                            cmd.Parameters.AddWithValue("@Linetype", ltype);
                            cmd.Parameters.AddWithValue("@Length", len);
                            cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                else
                {
                    ed.WriteMessage("No lines found.");
                }
            }
        }

        public static void ImportAllMTexts(string connectionString)
        {
            // Get the Document and Editor object
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                TypedValue[] tv = new TypedValue[1];
                tv.SetValue(new TypedValue((int)DxfCode.Start, "MTEXT"), 0);
                SelectionFilter filter = new SelectionFilter(tv);

                PromptSelectionResult promptSelectionResult = ed.SelectAll(filter);
                if (promptSelectionResult.Status == PromptStatus.OK)
                {
                    double insPtX = 0.0, insPtY = 0.0;
                    string layer = "", textstyle = "", color = "";
                    double height = 0.0, width = 0.0;
                    int attachment;
                    double rotation = 0.0;
                    MText mtx = new MText();
                    string tx = "";
                    SelectionSet selectionSet = promptSelectionResult.Value;

                    String sqlInsertQuery = @"INSERT INTO dbo.MTexts (InsPtX, InsPtY, Layer, Color, TextStyle, Height, Width, Rotation, Text, Attachment, Created)
                                       VALUES(@InsPtX, @InsPtY, @Layer, @Color, @TextStyle, @Height, @Width, @Rotation, @Text, @Attachment, @Created)";

                    using (var sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        foreach (SelectedObject selectedObj in selectionSet)
                        {
                            mtx = trans.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as MText;
                            insPtX = mtx.Location.X;
                            insPtY = mtx.Location.Y;
                            layer = mtx.Layer;
                            color = mtx.Color.ToString();
                            textstyle = mtx.TextStyleName;
                            height = mtx.TextHeight;
                            width = mtx.Width;
                            tx = mtx.Contents;
                            rotation = mtx.Rotation;
                            attachment = Convert.ToInt32(mtx.Attachment);



                            SqlCommand cmd = new SqlCommand(sqlInsertQuery, sqlConnection);
                            cmd.Parameters.AddWithValue("@InsPtX", insPtX);
                            cmd.Parameters.AddWithValue("@InsPtY", insPtY);
                            cmd.Parameters.AddWithValue("@Layer", layer);
                            cmd.Parameters.AddWithValue("@Color", color);
                            cmd.Parameters.AddWithValue("@TextStyle", textstyle);
                            cmd.Parameters.AddWithValue("@Height", height);
                            cmd.Parameters.AddWithValue("@Width", width);
                            cmd.Parameters.AddWithValue("@Rotation", rotation);
                            cmd.Parameters.AddWithValue("@Text", tx);
                            cmd.Parameters.AddWithValue("@Attachment", attachment);
                            cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                else
                {
                    ed.WriteMessage("No mtexts found.");
                }
            }
        }

        public static void ImportAllPolyLines(string connectionString)
        {
            // Get the Document and Editor object
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                TypedValue[] tv = new TypedValue[1];
                tv.SetValue(new TypedValue((int)DxfCode.Start, "LWPOLYLINE"), 0);
                SelectionFilter filter = new SelectionFilter(tv);

                PromptSelectionResult promptSelectionResult = ed.SelectAll(filter);
                // Check if an object is selected
                if (promptSelectionResult.Status == PromptStatus.OK)
                {
                    string name = "";
                    string layer = "", ltype = "";
                    string coords = "";
                    double len = 0.0;
                    Polyline pline = new Polyline();
                    bool isClosed = false;
                    SelectionSet selectionSet = promptSelectionResult.Value;

                    String sqlInsertQuery = @"INSERT INTO dbo.Plines (Name, Layer, Linetype, Length, Coordinates, IsClosed, Created)
                                       VALUES(@Name, @Layer, @Linetype, @Length, @Coordinates, @IsClosed, @Created)";
                    using (var sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        foreach (SelectedObject selectedObj in selectionSet)
                        {
                            pline = trans.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as Polyline;
                            name = pline.BlockName;
                            layer = pline.Layer;
                            ltype = pline.Linetype;
                            len = pline.Length;
                            coords = GetPolyLineCoordinates(pline);
                            isClosed = pline.Closed;


                            SqlCommand cmd = new SqlCommand(sqlInsertQuery, sqlConnection);
                            cmd.Parameters.AddWithValue("@Name", name);
                            cmd.Parameters.AddWithValue("@Layer", layer);
                            cmd.Parameters.AddWithValue("@Linetype", ltype);
                            cmd.Parameters.AddWithValue("@Length", len);
                            cmd.Parameters.AddWithValue("@Coordinates", coords);
                            cmd.Parameters.AddWithValue("@IsClosed", isClosed);
                            cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                            cmd.ExecuteNonQuery();
                        }

                    }
                }
                else
                {
                    ed.WriteMessage("No polylines found.");
                }
            }
        }

        public static void ImportAllBlocks(string connectionString)
        {
            var csvTable = new System.Data.DataTable();

            // Start by getting the validation data CSV
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var csvData = File.ReadAllLines(openFileDialog.FileName);

                // Assume the first row contains the column names
                var columns = csvData[0].Split(',');
                foreach (var column in columns)
                {
                    csvTable.Columns.Add(column);
                }

                // Add the remaining rows as data rows
                for (int i = 1; i < csvData.Length; i++)
                {
                    var row = csvTable.NewRow();
                    row.ItemArray = csvData[i].Split(',');
                    csvTable.Rows.Add(row);
                }
            }
            // Get the Document and Editor object
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                List<MText> mtextIds = new List<MText>();
                BlockTableRecord modelSpace = trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead) as BlockTableRecord;
                foreach (ObjectId objId in modelSpace)
                {
                    if (objId.ObjectClass.DxfName == "MTEXT")
                    {
                        MText mtext = trans.GetObject(objId, OpenMode.ForRead) as MText;
                        foreach (System.Data.DataRow row in csvTable.Rows)
                        {
                            if (row[0].ToString() == mtext.Text)
                            {
                                mtextIds.Add(mtext);
                            }
                        }
                    }
                }

                TypedValue[] tv = new TypedValue[1];
                tv.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                SelectionFilter filter = new SelectionFilter(tv);

                PromptSelectionResult promptSelectionResult = ed.SelectAll(filter);
                // Check if an object is selected
                if (promptSelectionResult.Status == PromptStatus.OK)
                {
                    double insPtX = 0.0, insPtY = 0.0;
                    string blkname = "";
                    string layer = "";
                    double rotation = 0.0, width = 0.0, length = 0.0;
                    BlockReference blk;
                    SelectionSet selectionSet = promptSelectionResult.Value;

                    String sqlInsertQuery = @"INSERT INTO dbo.Blocks (InsPtX, InsPtY, BlockName, ExtX, ExtY, ExtXFromName, ExtYFromName, ExtXFromFile, ExtYFromFile, ExtZFromFile, Layer, Rotation, Label, Created)
                                       VALUES(@InsPtX, @InsPtY, @BlockName, @ExtX, @ExtY, @ExtXFromName, @ExtYFromName, @ExtXFromFile, @ExtYFromFile, @ExtZFromFile, @Layer, @Rotation, @Label, @Created)";

                    foreach (SelectedObject selectedObj in selectionSet)
                    {
                        //List<string> label = new List<string>();
                        string label = "";
                        double widthFromName = 0.0, lengthFromName = 0.0, widthFromFile = 0.0, lengthFromFile = 0.0, heightFromFile = 0.0;
                        // string appended_label = "";
                        blk = trans.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as BlockReference;
                        if (blk.AttributeCollection.Count >= 0 & !blk.Name.Contains("*"))
                        {
                            Extents3d? bounds = blk.Bounds;
                            if (bounds.HasValue)
                            {
                                var ext = bounds.Value;
                                width = ext.MaxPoint.X - ext.MinPoint.X;
                                length = ext.MaxPoint.Y - ext.MinPoint.Y;
                                foreach (MText mtext in mtextIds)
                                {
                                    if (CheckAxisAlignedBlockAndMTextOverlap(blk, mtext))
                                    {
                                        label = mtext.Text;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                width = 0.0;
                                length = 0.0;
                            }

                            insPtX = blk.Position.X;
                            insPtY = blk.Position.Y;
                            blkname = blk.Name;
                            layer = blk.Layer;
                            rotation = blk.Rotation;
                            //appended_label = string.Join(", ", label.ToArray());

                            foreach (System.Data.DataRow row in csvTable.Rows)
                            {
                                if (row[0].ToString() == label)
                                {
                                    widthFromFile = Convert.ToDouble(row[1]);
                                    lengthFromFile = Convert.ToDouble(row[2]);
                                    heightFromFile = Convert.ToDouble(row[3]);
                                }
                            }

                            // @TODO: RegEx for Names requires refactor
                            // @TODO: Underhang Label Strip for Blocks requires refactor

                            using (var sqlConnection = new SqlConnection(connectionString))
                            {
                                sqlConnection.Open();
                                SqlCommand cmd = new SqlCommand(sqlInsertQuery, sqlConnection);
                                cmd.Parameters.AddWithValue("@InsPtX", insPtX);
                                cmd.Parameters.AddWithValue("@InsPtY", insPtY);
                                cmd.Parameters.AddWithValue("@BlockName", blkname);
                                cmd.Parameters.AddWithValue("@ExtX", width);
                                cmd.Parameters.AddWithValue("@ExtY", length);
                                cmd.Parameters.AddWithValue("@ExtXFromName", widthFromName);
                                cmd.Parameters.AddWithValue("@ExtYFromName", lengthFromName);
                                cmd.Parameters.AddWithValue("@ExtXFromFile", widthFromFile);
                                cmd.Parameters.AddWithValue("@ExtYFromFile", lengthFromFile);
                                cmd.Parameters.AddWithValue("@ExtZFromFile", heightFromFile);
                                cmd.Parameters.AddWithValue("@Layer", layer);
                                cmd.Parameters.AddWithValue("@Rotation", rotation);
                                cmd.Parameters.AddWithValue("@Label", label);
                                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                                cmd.ExecuteNonQuery();
                            }

                        }
                    }
                }
                else
                {
                    ed.WriteMessage("No polylines found.");
                }
            }
        }

        public static System.Data.DataTable ImportValidationFile()
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CSV files (*.csv)|*.csv|All files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var csvTable = new System.Data.DataTable();
                var csvData = File.ReadAllLines(openFileDialog.FileName);

                // Assume the first row contains the column names
                var columns = csvData[0].Split(',');
                foreach (var column in columns)
                {
                    csvTable.Columns.Add(column);
                }

                // Add the remaining rows as data rows
                for (int i = 1; i < csvData.Length; i++)
                {
                    var row = csvTable.NewRow();
                    row.ItemArray = csvData[i].Split(',');
                    csvTable.Rows.Add(row);
                }
                return csvTable;

            }
            else
            {
                return null;
            }
        }

        public static bool CheckAxisAlignedBlockAndMTextOverlap(BlockReference blockReference, MText mtext)
        {
            // Extents of Block and MText
            Extents3d? blockBounds = blockReference.Bounds;
            Extents3d? mtextBounds = mtext.Bounds;
            var blockExt = blockBounds.Value;
            var mtextExt = mtextBounds.Value;

            // Rectangular overlap algorithm
            var blockTopLeft = (blockExt.MinPoint.X, blockExt.MaxPoint.Y);
            var blockBottomRight = (blockExt.MaxPoint.X, blockExt.MinPoint.Y);
            var mtextTopLeft = (mtextExt.MinPoint.X, mtextExt.MaxPoint.Y);
            var mtextBottomRight = (mtextExt.MaxPoint.X, mtextExt.MinPoint.Y);

            if (blockTopLeft.X == blockBottomRight.X || blockTopLeft.Y == blockBottomRight.Y || mtextBottomRight.X == mtextTopLeft.X || mtextTopLeft.Y == mtextBottomRight.Y)
            {
                return false;
            }

            if (blockTopLeft.X > mtextBottomRight.X || mtextTopLeft.X > blockBottomRight.X)
            {
                return false;
            }

            if (blockBottomRight.Y > mtextTopLeft.Y || mtextBottomRight.Y > blockTopLeft.Y)
            {
                return false;
            }

            return true;

        }

        public static bool CheckIrregularBlockAndMTextOverlap(BlockReference blockReference, MText mtext)
        {

            return true;

        }

        public static bool MTextExistsWithinBlock()
        {
            // Get the Document and Editor object
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            using (Transaction trans = doc.TransactionManager.StartTransaction())
            {
                TypedValue[] tv = new TypedValue[1];
                tv.SetValue(new TypedValue((int)DxfCode.Start, "INSERT"), 0);
                SelectionFilter filter = new SelectionFilter(tv);

                PromptSelectionResult promptSelectionResult = ed.GetSelection(filter);
                // Check if an object is selected
                if (promptSelectionResult.Status == PromptStatus.OK)
                {
                    BlockReference blockReference;
                    SelectionSet selectionSet = promptSelectionResult.Value;

                    foreach (SelectedObject selectedObj in selectionSet)
                    {
                        blockReference = trans.GetObject(selectedObj.ObjectId, OpenMode.ForRead) as BlockReference;
                        Extents3d? blockBounds = blockReference.Bounds;
                        if (blockBounds.HasValue)
                        {
                            var blockExt = blockBounds.Value;
                            // Get all MTEXT objects in the document
                            ObjectIdCollection mtextIds = new ObjectIdCollection();
                            BlockTableRecord modelSpace = trans.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead) as BlockTableRecord;
                            foreach (ObjectId objId in modelSpace)
                            {
                                if (objId.ObjectClass.DxfName == "MTEXT")
                                {
                                    MText mtext;
                                    // Check if the MTEXT object is within the extents of the block object
                                    mtext = trans.GetObject(objId, OpenMode.ForRead) as MText;
                                    Extents3d? mtextBounds = mtext.Bounds;
                                    if (mtextBounds.HasValue)
                                    {
                                        var mtextExt = mtextBounds.Value;
                                        var blockTopLeft = (blockExt.MinPoint.X, blockExt.MaxPoint.Y);
                                        var blockBottomRight = (blockExt.MaxPoint.X, blockExt.MinPoint.Y);
                                        var mtextTopLeft = (mtextExt.MinPoint.X, mtextExt.MaxPoint.Y);
                                        var mtextBottomRight = (mtextExt.MaxPoint.X, mtextExt.MinPoint.Y);

                                        if (blockTopLeft.X > mtextBottomRight.X || mtextTopLeft.X > mtextBottomRight.X)
                                        {
                                            return false;
                                        }

                                        if (blockBottomRight.Y > mtextTopLeft.Y || mtextBottomRight.Y > blockTopLeft.Y)
                                        {
                                            return false;
                                        }

                                        System.Windows.MessageBox.Show(mtext.Text);
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }
        public static void ImportAllGroups(string connectionString)
        {
            // Get the current document and database
            Document doc = Application.DocumentManager.MdiActiveDocument;
            Editor ed = doc.Editor;
            Database db = doc.Database;

            String sqlInsertQuery = @"INSERT INTO dbo.Groups (GroupName, Alpha, InsPtX, InsPtY, ExtX, ExtY, Rotation, Other, Created)
                                       VALUES(@GroupName, @Alpha, @InsPtX, @InsPtY, @ExtX, @ExtY, @Rotation, @Other, @Created)";

            //Regex to find alpha/EqNum
            Regex rxAlpha = new Regex(@"[A-Z]{3}", RegexOptions.Compiled);
            Regex rxEqNum = new Regex(@"(?<!\s)\S{6,}", RegexOptions.Compiled);

            // Start a transaction
            using (Transaction trans = db.TransactionManager.StartTransaction())
            {
                DBDictionary groups = trans.GetObject(db.GroupDictionaryId, OpenMode.ForRead) as DBDictionary;

                foreach (DBDictionaryEntry entry in groups)
                {
                    Group group = (Group)trans.GetObject(entry.Value, OpenMode.ForRead);

                    // Create a new Extents3d to hold the aggregate extents of all non-text and non-mtext objects in the group
                    var groupExtents = new Extents3d();
                    List<string> other = new List<string>();
                    string alpha = "";
                    //string eqNum = "";
                    double rotation = 0.0;

                    ObjectId[] ids = group.GetAllEntityIds();
                    foreach (ObjectId id in ids)
                    {

                        if (id.ObjectClass.DxfName == "TEXT")
                        {
                            DBText text = trans.GetObject(id, OpenMode.ForRead) as DBText;
                            ed.WriteMessage(group.Name + " contains the following potential labels: " + text.TextString + "\n");

                            if (text.TextString.Length == 3 && rxAlpha.IsMatch(text.TextString))
                            {
                                alpha = text.TextString;
                            }
                            //else if (rxEqNum.IsMatch(text.TextString))
                            //{
                            //    eqNum = text.TextString;
                            //}
                            //else
                            {
                                other.Add(text.TextString);
                            }
                        }
                        else if (id.ObjectClass.DxfName == "MTEXT")
                        {
                            var mtext = trans.GetObject(id, OpenMode.ForRead) as MText;
                            ed.WriteMessage(group.Name + " contains the following potential labels: " + mtext.Contents + "\n");

                            if (mtext.Contents.Length == 3 && rxAlpha.IsMatch(mtext.Contents))
                            {
                                alpha = mtext.Contents;
                            }
                            //else if (rxEqNum.IsMatch(mtext.Contents))
                            //{
                            //    eqNum = mtext.Contents;
                            //}
                            //else
                            {
                                other.Add(mtext.Contents);
                            }
                        }
                        // @TODO: find out how to identify Insert base point before turning this back on
                        //else if (id.ObjectClass.DxfName == "INSERT")
                        //{
                        //    BlockReference block = id.GetObject(OpenMode.ForRead) as BlockReference;
                        //    groupExtents = block.GeometricExtents;
                        //    rotation = block.Rotation * 57.2958;
                        //}
                        else
                        {
                            var ent = (Entity)id.GetObject(OpenMode.ForRead);
                            groupExtents.AddExtents(ent.GeometricExtents);
                        }
                    }
                    using (var sqlConnection = new SqlConnection(connectionString))
                    {
                        sqlConnection.Open();
                        string joinedOther = "";
                        joinedOther = String.Join(", ", other);
                        SqlCommand cmd = new SqlCommand(sqlInsertQuery, sqlConnection);
                        cmd.Parameters.AddWithValue("@GroupName", group.Name);
                        cmd.Parameters.AddWithValue("@Alpha", alpha.Trim()); // Plant Sim doesn't support spaces in a name field
                        //cmd.Parameters.AddWithValue("@EQNum", eqNum);
                        cmd.Parameters.AddWithValue("@InsPtX", groupExtents.MinPoint.X + ((groupExtents.MaxPoint.X - groupExtents.MinPoint.X) / 2)); // AutoCAD gives bottom-left coords, Plant Sim uses center-point
                        cmd.Parameters.AddWithValue("@InsPtY", groupExtents.MinPoint.Y + ((groupExtents.MaxPoint.Y - groupExtents.MinPoint.Y) / 2));
                        cmd.Parameters.AddWithValue("@ExtX", groupExtents.MaxPoint.X - groupExtents.MinPoint.X);
                        cmd.Parameters.AddWithValue("@ExtY", groupExtents.MaxPoint.Y - groupExtents.MinPoint.Y);
                        cmd.Parameters.AddWithValue("@Rotation", rotation);
                        cmd.Parameters.AddWithValue("@Other", joinedOther);
                        cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                        cmd.ExecuteNonQuery();

                        ed.WriteMessage(group.Name + " bounded by X = " + groupExtents.MinPoint.X + ", " + groupExtents.MaxPoint.X + ", Y = " + groupExtents.MinPoint.Y + ", " + groupExtents.MaxPoint.Y + "\n");
                    }
                }
            }
        }
    }
}