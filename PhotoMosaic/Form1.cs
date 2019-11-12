using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    public partial class Form1 : Form {
        Graphics Canvas;
        int CellsValue;
        string sourceImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            CellsValue = (int)numericUpDownCells.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CellsValue = (int)numericUpDownCells.Value;

            //SourceImage img = new SourceImage(pictureBox1, CellsValue, sourceImage);
            //img.CalculateAVGCellColors();            
            //img.DrawAvgColors();

            //ImageEditor ImgEdit = new ImageEditor(pictureBox1);
            //ImgEdit.ImportImage_CropAndResize_Save(sourceImage);
            //Image nwImage = new Image(pictureBox1, CellsValue, ImgEdit.saveFileName);

            ImageController imgController = new ImageController();
            imgController.ImportSourceImages("dsad");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\Users\nick.muldrew\Downloads\",
                Title = "Browse Pictures",
                CheckFileExists = true,
                CheckPathExists = true,
                RestoreDirectory = true,

                Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" +
                "All files (*.*)|*.*"
        };

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                sourceImage = openFileDialog1.FileName;
            }
        }
    }
}
