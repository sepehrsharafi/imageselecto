﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using DarkUI.Forms;

namespace ImageSelector
{
    public partial class frmImageSelector : DarkForm
    {

        string[] images;

        int CurrentIndex = 0;

        string NotesPath;

        // pan
        public System.Drawing.Point mouseDownPoint;//Storage mouse focus of global variables
        public bool isSelected = false;

        int CurrentZoom = 0;

        public string SelectedFolderPath
        {
            get
            {
                if (!string.IsNullOrEmpty(txtAddress.Text))
                    return txtAddress.Text + @"\selected";
                return null;
            }
        }

        public frmImageSelector()
        {
            InitializeComponent();
            dgSelectedImages.AutoGenerateColumns = false;
        }

        private void frmImageSelector_Load(object sender, EventArgs e)
        {
            this.DoubleBuffered = true;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    dgSelectedImages.Visible = true;
                    txtAddress.Text = fbd.SelectedPath;
                    Helper.FillSelectedImages(SelectedFolderPath, dgSelectedImages);
                    images = Helper.GetImages(txtAddress.Text);
                    CurrentIndex = 0;
                    RestPictureBoxLocation();
                    ShowImage();
                }
            }
        }

        private void dgSelectedImages_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void dgSelectedImages_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            foreach (DataGridViewRow row in dgSelectedImages.Rows)
            {
                FileInfo file = row.DataBoundItem as FileInfo;
                if (file != null)
                {

                    row.Cells[0].Value = Helper.CreateThumbnailImage(file.FullName);
                }

            }
        }

        private void ShowImage()
        {

            Image img;
            using (var bmpTemp = new Bitmap(images[CurrentIndex]))
            {
                img = new Bitmap(bmpTemp);
            }

            pbCurrentImage.Image = img;
            lblFileName.Text = Path.GetFileName(images[CurrentIndex]);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (CurrentIndex < images.Length - 1)
            {
                CurrentIndex++;
                ShowImage();
            }
        }

        private void btnPrv_Click(object sender, EventArgs e)
        {
            if (CurrentIndex > 0)
            {
                CurrentIndex--;
                ShowImage();
            }
        }

        private void dgSelectedImages_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            this.colImage.DefaultCellStyle.NullValue = null;
        }

        private void btnNext10_Click(object sender, EventArgs e)
        {
            if (CurrentIndex < images.Length - 10)
            {
                CurrentIndex = CurrentIndex + 10;
                ShowImage();
            }
        }

        private void btnPrv10_Click(object sender, EventArgs e)
        {
            if (CurrentIndex > 9)
            {
                CurrentIndex = CurrentIndex - 10;
                ShowImage();
            }
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            if (!Directory.Exists(SelectedFolderPath))
            {
                Directory.CreateDirectory(SelectedFolderPath);
            }

            string currentImagePath = images[CurrentIndex];

            DirectoryInfo source = new DirectoryInfo(txtAddress.Text);
            DirectoryInfo dest = new DirectoryInfo(SelectedFolderPath);

            Helper.CopyFiles(source, dest, true, Path.GetFileNameWithoutExtension(currentImagePath) + ".*");

            Helper.FillSelectedImages(SelectedFolderPath, dgSelectedImages);

            NotesPath = SelectedFolderPath + @"\" + Path.GetFileNameWithoutExtension(currentImagePath) + ".txt";

            File.Create(NotesPath).Close(); // Create file

            using (StreamWriter sw = File.AppendText(NotesPath))
            {
                sw.Write(txtNotes.Text); // Write text to .txt file
                sw.Close();
            }

            txtNotes.Clear();

            //lblFileName.Text + ".txt"

        }

        private void frmImageSelector_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void dgSelectedImages_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.ColumnIndex == 1)
            {
                int rowIndex = dgSelectedImages.CurrentCell.RowIndex;

                string FileName = ((FileInfo[])dgSelectedImages.DataSource)[rowIndex].FullName;

                DirectoryInfo dir = new DirectoryInfo(SelectedFolderPath);

                Helper.DeleteFiles(dir, Path.GetFileNameWithoutExtension(FileName) + ".*");

                Helper.FillSelectedImages(SelectedFolderPath, dgSelectedImages);
            }
            //(dgSelectedImages.Rows.Remove);

        }

        private void frmImageSelector_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgSelectedImages_DoubleClick(object sender, EventArgs e)
        {

        }

        private void dgSelectedImages_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 0)
            {

                int rowIndex = dgSelectedImages.CurrentCell.RowIndex;

                string FileName = ((FileInfo[])dgSelectedImages.DataSource)[rowIndex].FullName;

                frm_PictureBox Picturebox_Form = new frm_PictureBox(rowIndex, FileName, SelectedFolderPath);
                Picturebox_Form.Show();

            }
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {

        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {

        }

        private void dgSelectedImages_DataSourceChanged(object sender, EventArgs e)
        {

        }

        private void btnRotateRight_Click(object sender, EventArgs e)
        {
            pbCurrentImage.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            pbCurrentImage.Refresh();
        }

        private void btnRotateLeft_Click(object sender, EventArgs e)
        {
            pbCurrentImage.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
            pbCurrentImage.Refresh();
        }

        private void frmImageSelector_KeyDown(object sender, KeyEventArgs e)
        {
            //capture left arrow key
            if (e.KeyCode == Keys.Left)
            {
                if (CurrentIndex > 0)
                {
                    CurrentIndex--;
                    ShowImage();
                }
            }

            //capture right arrow key
            if (e.KeyCode == Keys.Right)
            {
                if (CurrentIndex < images.Length)
                {
                    CurrentIndex++;
                    ShowImage();
                }
            }

            //capture up arrow key
            if (e.KeyCode == Keys.Up)
            {
                if (CurrentIndex < images.Length - 10)
                {
                    CurrentIndex = CurrentIndex + 10;
                    ShowImage();
                }
            }

            //capture down arrow key
            if (e.KeyCode == Keys.Down)
            {
                if (CurrentIndex > 9)
                {
                    CurrentIndex = CurrentIndex - 10;
                    ShowImage();
                }
            }

            //Rotate Right
            if (e.KeyCode == Keys.Right && e.Control)
            {
                pbCurrentImage.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                pbCurrentImage.Refresh();
            }

            //Rotate Left
            if (e.KeyCode == Keys.Left && e.Control)
            {
                pbCurrentImage.Image.RotateFlip(RotateFlipType.Rotate90FlipXY);
                pbCurrentImage.Refresh();
            }

            //capture Enter arrow key
            if (e.KeyCode == Keys.Enter && e.Control)
            {
                if (!Directory.Exists(SelectedFolderPath))
                {
                    Directory.CreateDirectory(SelectedFolderPath);
                }

                string currentImagePath = images[CurrentIndex];

                DirectoryInfo source = new DirectoryInfo(txtAddress.Text);
                DirectoryInfo dest = new DirectoryInfo(SelectedFolderPath);

                Helper.CopyFiles(source, dest, true, Path.GetFileNameWithoutExtension(currentImagePath) + ".*");

                Helper.FillSelectedImages(SelectedFolderPath, dgSelectedImages);

                NotesPath = SelectedFolderPath + @"\" + Path.GetFileNameWithoutExtension(currentImagePath) + ".txt";

                File.Create(NotesPath).Close(); // Create file

                using (StreamWriter sw = File.AppendText(NotesPath))
                {
                    sw.Write(txtNotes.Text); // Write text to .txt file
                    sw.Close();
                }

                txtNotes.Clear();

                //byte[] bytes = Convert.FromBase64String(images[CurrentIndex]);

                //var image = GetImage(images[CurrentIndex]);


                //Helper.SelectImage(SelectedFolderPath, CurrentIndex, txtNotes.Text, txtAddress.Text, currentImagePath, image[], dgSelectedImages);
            }
        }

        private void TrackBar_Scroll(object sender, EventArgs e)
        {
            pnlContainer.VerticalScroll.Value = 0;
            if (TrackBar.Value != 0)
            {
                pbCurrentImage.Location = new Point(0, 0);
                if (TrackBar.Value > CurrentZoom)
                {
                    CurrentZoom++;
                    pbCurrentImage.Width += 150;
                    pbCurrentImage.Height += 150;
                }
                else if (TrackBar.Value < CurrentZoom)
                {
                    CurrentZoom--;
                    pbCurrentImage.Width -= 150;
                    pbCurrentImage.Height -= 150;
                }
                pbCurrentImage.Refresh();
            }
            else
            {
                CurrentZoom = 0;
                RestPictureBoxLocation();
            }
        }

        void RestPictureBoxLocation()
        {
            pbCurrentImage.SizeMode = PictureBoxSizeMode.Zoom;
            pbCurrentImage.Location = new Point(0, 0);
            pbCurrentImage.Width = pnlContainer.Width - 10;
            pbCurrentImage.Height = pnlContainer.Height - 10;
            pbCurrentImage.Refresh();
            pnlContainer.Refresh();
        }

        #region Panning

        private void pbCurrentImage_MouseUp(object sender, MouseEventArgs e)
        {
            isSelected = false;
        }

        private void pbCurrentImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (isSelected && IsMouseInPanel())//Determined to have been within the range for the MouseDown event, and mouse in the picturebox
            {
                this.pbCurrentImage.Left = this.pbCurrentImage.Left + (Cursor.Position.X - mouseDownPoint.X);
                this.pbCurrentImage.Top = this.pbCurrentImage.Top + (Cursor.Position.Y - mouseDownPoint.Y);
                mouseDownPoint.X = Cursor.Position.X;
                mouseDownPoint.Y = Cursor.Position.Y;
            }
        }

        private void pbCurrentImage_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseDownPoint.X = Cursor.Position.X;  //Note: The preceding has been defined as a global variable of type Point mouseDownPoint  
                mouseDownPoint.Y = Cursor.Position.Y;
                isSelected = true;
            }
        }

        private void pbCurrentImage_MouseEnter(object sender, EventArgs e)
        {
            pbCurrentImage.Cursor = Cursors.SizeAll;
        }

        private bool IsMouseInPanel()
        {
            if (this.pbCurrentImage.Left < PointToClient(Cursor.Position).X
                    && PointToClient(Cursor.Position).X < this.pbCurrentImage.Left
                    + this.pbCurrentImage.Width && this.pbCurrentImage.Top
                    < PointToClient(Cursor.Position).Y && PointToClient(Cursor.Position).Y
                    < this.pbCurrentImage.Top + this.pbCurrentImage.Height)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}