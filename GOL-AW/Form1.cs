using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace GOL_AW
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[5, 5];
        bool[,] scratchPad = new bool[5, 5];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.BlueViolet;

        // The Timer class
        Timer timer = new Timer();

        //Modes
        bool finiteFlag = true;
        bool toroidalFlag = false;

        // Generation count
        int generations = 0;

        public Form1()
        {
            cellColor = Properties.Settings.Default.LiveCellColor;
            universe = new bool[Properties.Settings.Default.UniverseWidth, Properties.Settings.Default.UniverseHeight];
            scratchPad = new bool[Properties.Settings.Default.UniverseWidth, Properties.Settings.Default.UniverseHeight];
            InitializeComponent();

            // Setup the timer
            timer.Interval = Properties.Settings.Default.Interval; // milliseconds
            timer.Tick += new EventHandler(Timer_Tick);
            timer.Enabled = false; // start timer running
        }


        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for(int yNeighbor = y - 1; yNeighbor <= y + 1 ; yNeighbor++)
            {
                for(int xNeighbor = x - 1; xNeighbor <= x + 1  ; xNeighbor++)
                {
                    if (yNeighbor < 0 || yNeighbor >= yLen) break;
                    if (xNeighbor < 0 || xNeighbor >= xLen) continue;
                    if (xNeighbor == x && yNeighbor == y) continue;
                    if (universe[xNeighbor, yNeighbor] == true) count++;                    
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            int yCheck;
            int xCheck;
            for (int yNeighbor = y - 1; yNeighbor <= y + 1; yNeighbor++)
            {
                yCheck = yNeighbor;
                for(int xNeighbor = x - 1; xNeighbor <= x + 1; xNeighbor++)
                {
                    xCheck = xNeighbor;
                    if (yCheck == y && xCheck == x) { continue; }
                    if (yNeighbor < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    else if (yNeighbor >= yLen)
                    {
                        yCheck = 0;
                    }
                    else
                    {
                        yCheck = yNeighbor;
                    }
                    if (xNeighbor < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    else if(xNeighbor >= xLen)
                    {
                        xCheck = 0;
                    }
                    else
                    {
                        xCheck = xNeighbor;
                    }
                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for(int yCurrent = 0; yCurrent < universe.GetLength(1); yCurrent++)
            {
                for(int xCurrent = 0; xCurrent < universe.GetLength(0); xCurrent++)
                {
                    int neighborCount = (finiteFlag) ? CountNeighborsFinite(xCurrent, yCurrent)
                        : CountNeighborsToroidal(xCurrent, yCurrent);

                    switch (universe[xCurrent, yCurrent])
                    {
                        case true:
                            if (neighborCount == 2 || neighborCount == 3)
                            {
                                scratchPad[xCurrent, yCurrent] = true;
                            }
                            else
                            {
                                scratchPad[xCurrent, yCurrent] = false;
                            }
                            break;
                        default:
                            if (neighborCount == 3)
                            {
                                scratchPad[xCurrent, yCurrent] = true;
                            }
                            else
                            {
                                scratchPad[xCurrent, yCurrent] = false;
                            }
                            break;
                    }
                }
            }
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;


            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void ClearScratchPad()
        {
            for (int y = 0; y < scratchPad.GetLength(1); y++)
            {
                for (int x = 0; x < scratchPad.GetLength(0); x++)
                {
                    scratchPad[x, y] = false;
                }
            }
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // A Brush for writing the neighbor count
            Brush countBrush = new SolidBrush(Color.Red);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    int neighborCount = (finiteFlag) ? CountNeighborsFinite(x, y)
                        : CountNeighborsToroidal(x, y);
                    string neighbors = neighborCount.ToString();
                    PointF center = new PointF(cellRect.X + (cellWidth / 2),
                        cellRect.Y + (cellHeight / 2));


                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    //Write the neighbor count.
                    if (neighborCount > 0)
                    {
                        e.Graphics.DrawString(neighbors, DefaultFont, countBrush, center);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            countBrush.Dispose();
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void graphicsPanel1_Click(object sender, EventArgs e)
        {
            
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            NextGeneration();
        }

        //Clears the universe
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearScratchPad();
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
            timer.Start();
        }
        
        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            timer.Stop();
            timer.Enabled = false;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Properties.Settings.Default.LiveCellColor = cellColor;
            Properties.Settings.Default.BackgroundColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.UniverseWidth = universe.GetLength(0);
            Properties.Settings.Default.UniverseHeight = universe.GetLength(1);

            Properties.Settings.Default.Save();
        }

        //resets the settings
        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cellColor = Properties.Settings.Default.LiveCellColor;
            graphicsPanel1.BackColor = Properties.Settings.Default.BackgroundColor;
            universe = new bool[Properties.Settings.Default.UniverseWidth, Properties.Settings.Default.UniverseHeight];
            Properties.Settings.Default.Reset();
        }

        //reloads the settings
        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cellColor = Properties.Settings.Default.LiveCellColor;
            graphicsPanel1.BackColor = Properties.Settings.Default.BackgroundColor;
            universe = new bool[Properties.Settings.Default.UniverseWidth, Properties.Settings.Default.UniverseHeight];
            Properties.Settings.Default.Reload();
        }

        //opens a dialog box to change the color of the living cells
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = cellColor;


            if (DialogResult.OK == dialog.ShowDialog())
            {
                cellColor = dialog.Color;
                graphicsPanel1.Invalidate();
            }
        }

        //opens a dialog box to change the color of the background
        private void backgroundColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog dialog = new ColorDialog();
            dialog.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == dialog.ShowDialog())
            {
                graphicsPanel1.BackColor = dialog.Color;
                graphicsPanel1.Invalidate();
            }
        }

        //Opens the filesystem to save the current state of cells in the universe
        private void saveToolStripButton_Click(object sender, EventArgs e)
        {
            SaveFileDialog dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StreamWriter streamWriter = new StreamWriter(dialog.FileName);
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    string currentRow = string.Empty;
                    for( int x = 0; x < universe.GetLength(0); x++)
                    {
                        if(universe[x,y] == true)
                        {
                            currentRow += 'O';
                        }
                        else
                        {
                            currentRow += '.';
                        }
                    }
                    streamWriter.WriteLine(currentRow);
                }
                streamWriter.Close();
            }
        }

        //Universe will treat boundaries as wrapping around
        private void torodialToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            finiteToolStripMenuItem.Checked = !finiteToolStripMenuItem.Checked;
            toroidalFlag = !toroidalFlag;
            finiteFlag = !finiteFlag;
            graphicsPanel1.Invalidate();
        }

        //Universe will end at the boundaries
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            toroidalToolStripMenuItem.Checked = !toroidalToolStripMenuItem.Checked;
            finiteFlag = !finiteFlag;
            toroidalFlag = !toroidalFlag;
            graphicsPanel1.Invalidate();
        }

        //Will change state of cells to match a saved universe
        private void openToolStripButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                StreamReader reader = new StreamReader(dialog.FileName);
                int maxHeight = 0;
                int maxWidth = 0;
                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();
                    if (!row.StartsWith("!"))
                    {
                        maxHeight += 1;
                        maxWidth = row.Length;
                    }
                }
                universe = new bool[maxWidth, maxHeight];
                scratchPad = new bool[maxWidth, maxHeight];

                reader.BaseStream.Seek(0, SeekOrigin.Begin);
                while (!reader.EndOfStream)
                {
                    string row = reader.ReadLine();
                    if (!row.StartsWith("!"))
                    {
                        int y = 0;
                        for (int x = 0; x < row.Length; x++)
                        {
                            if (row[x] == 'O')
                            {
                                scratchPad[x, y] = true;
                            }
                            else
                            {
                                scratchPad[x, y] = false;
                            }
                            y++;
                        }
                    }
                }
                reader.Close();
                universe = scratchPad;
                graphicsPanel1.Invalidate();
            }
        }

        //opens a dialog box to change the size of the universe
        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModalDialog dialog = new ModalDialog();
            dialog.NumberWidth = universe.GetLength(0);
            dialog.NumberHeight = universe.GetLength(1);

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                universe = new bool[dialog.NumberWidth, dialog.NumberHeight];
                scratchPad = new bool[dialog.NumberWidth, dialog.NumberHeight];
                graphicsPanel1.Invalidate();
            }
        }

        //opens a dialog box to change the length of interval between generations
        private void intervalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModalDialogInterval dialog = new ModalDialogInterval();
            dialog.Number = timer.Interval;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                timer.Interval = dialog.Number;
                graphicsPanel1.Invalidate();
            }
        }

        private void randomizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Random rng = new Random();
            int number;
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    number = rng.Next();
                    if (number % 2 == 0)
                    {
                        scratchPad[x, y] = true;
                        universe[x, y] = true;
                    }
                    else
                    {
                        scratchPad[x, y] = false;
                        universe[x, y] = false;
                    }
                }
            }
            graphicsPanel1.Invalidate();
        }
    }
}
