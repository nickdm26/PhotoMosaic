using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoMosaic {
    public partial class Form1 : Form {
        Graphics Canvas;
        int CellsValue;
        string InputImage;
        string SourceImagesFolder;
        ImageController imgController;
        int CellOldValue;

        public Form1()
        {
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {   
            imgController = new ImageController(pictureBox1);
            numericUpDownCells.Maximum = 256;
            numericUpDownCells.Minimum = 16;

            CellOldValue = (int) numericUpDownCells.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //CellsValue = (int)trackBar1.Value;
            Console.WriteLine(CellsValue);

            //SourceImage img = new SourceImage(pictureBox1, CellsValue, sourceImage);        //calculates the average cell colours then draws it.
            //img.CalculateAVGCellColors();            
            //img.DrawAvgColors();

            //ImageEditor ImgEdit = new ImageEditor(pictureBox1);
            //ImgEdit.ImportImage_CropAndResize_Save(sourceImage);
            //Image nwImage = new Image(pictureBox1, CellsValue, ImgEdit.saveFileName);

            //ImageController imgController = new ImageController(pictureBox1);
            imgController.GenerateMosaic(InputImage, SourceImagesFolder, (int)numericUpDownCells.Value, (int) numericUpDownSize.Value);
        }

        /*
         * Browse InputImage Button fills a global variable InputImage with the path of the image.
         */
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog1 = new OpenFileDialog
            {
                InitialDirectory = @"C:\Users\nick.muldrew\Downloads\",
                Title = "Browse Pictures",
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                RestoreDirectory = true,
                FileName = "Folder Selection.",

                Filter = "Images (*.BMP;*.JPG;*.GIF,*.PNG,*.TIFF)|*.BMP;*.JPG;*.GIF;*.PNG;*.TIFF|" +
                "All files (*.*)|*.*"
        };

            if(openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                InputImage = openFileDialog1.FileName;
            }
        }

        /*
         * Browse Source Images Folder fills a global variable SourceImagesFolder with the path of the folder selected.
         */
        private void Button3_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog2 = new OpenFileDialog
            {
                Title = "Select Folder",
                ValidateNames = false,
                CheckFileExists = false,
                CheckPathExists = true,
                FileName = "anyFile",
                Filter = "folders|*.neverseenthisfile"
        };

            if (openFileDialog2.ShowDialog() == DialogResult.OK)
            {
                string dir = openFileDialog2.FileName;
                SourceImagesFolder = dir.Substring(0, dir.LastIndexOf('\\'));

                Console.WriteLine(openFileDialog2.FileName);
                Console.WriteLine(dir.Substring(0, dir.LastIndexOf('\\')));
            }
        }

        private void ButtonSlow_Click(object sender, EventArgs e)
        {
            //ImageController imgController = new ImageController(pictureBox1);
            imgController.GenerateMosaic(@"C:\Users\User\Documents\PhotoMosaic\Images\test_dog.png", @"C:\Users\User\Documents\PhotoMosaic\Images", (int)numericUpDownCells.Value, (int)numericUpDownSize.Value);
        }

        private void ButtonFast_Click(object sender, EventArgs e)
        {
            //ImageController imgController = new ImageController(pictureBox1);
            imgController.GenerateMosaic(@"C:\Users\User\Documents\PhotoMosaic\Images\test_dog.png", @"C:\Users\User\Documents\PhotoMosaic\Images\n02085620-Chihuahua\", (int)numericUpDownCells.Value, (int)numericUpDownSize.Value);
        }

        private void ButtonSuperFast_Click(object sender, EventArgs e)
        {
            //Clear();
            //ImageController imgController = new ImageController(pictureBox1);
            imgController.GenerateMosaic(@"C:\Users\User\Documents\PhotoMosaic\Images\test_dog.png", @"C:\Users\User\Documents\PhotoMosaic\Images\Test\", (int)numericUpDownCells.Value, (int)numericUpDownSize.Value);
            //Image te = new Image(@"C:\Users\User\Documents\PhotoMosaic\Images\TestPic.png", 64);
            //Image test = new Image(@"C:\Users\User\Documents\PhotoMosaic\Images\TestPic.png", 64);
            //Image te2 = new Image(@"C:\Users\User\Documents\PhotoMosaic\Images\TestPic2.png", 64);
            //Image test2 = new Image(@"C:\Users\User\Documents\PhotoMosaic\Images\TestPic2.png", 64);

            //Console.WriteLine("Normal: " + te.CalculateSection(0, 0, 800, 800));
            //Console.WriteLine("Lockbits: " + test.CalculateSectionUsingLockBits(0, 0, 800,800));

            //Console.WriteLine("Normal 2: " + te2.CalculateSection(0, 0, 800, 800));
            //Console.WriteLine("Lockbits 2: " + test2.CalculateSectionUsingLockBits(0, 0, 800, 800));
        }

        public void Clear()
        {
            var b = new Bitmap(1, 1);
            b.SetPixel(0, 0, Color.Black);
            pictureBox1.Image = new Bitmap(b, 800, 800);
        }

        private void ButtonAllDogs_Click(object sender, EventArgs e)
        {
            Clear();
            ImageController imgController = new ImageController(pictureBox1);
            imgController.GenerateMosaic(@"C:\Users\User\Documents\PhotoMosaic\Images\test_dog.png", @"C:\Users\User\Documents\PhotoMosaic\Dogs\", (int) numericUpDownCells.Value, (int)numericUpDownSize.Value);
        }

        private void ButtonSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefiledialog = new SaveFileDialog
            {
                Filter = "Bitmap Image (.bmp)|*.bmp|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png"
            };

            //savefiledialog.ShowDialog();
            if(savefiledialog.ShowDialog() == DialogResult.OK)
            {
                imgController.Save(savefiledialog.FileName);
                Console.WriteLine(Path.GetFullPath(savefiledialog.FileName));
            }
        }

        private void NumericUpDownCells_ValueChanged(object sender, EventArgs e)
        {
            int newValue = (int) numericUpDownCells.Value;

            if (CellOldValue > newValue)
            {
                newValue = CellOldValue / 2;
                numericUpDownCells.Value = newValue;
            }else if(CellOldValue < newValue)
            {
                newValue = CellOldValue * 2;
                numericUpDownCells.Value = newValue;
            }

            CellOldValue = newValue;
        }

        private void NumericUpDownSize_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
