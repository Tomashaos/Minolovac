using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;


namespace WindowsFormsApplication3
{
    public class GraphicDesign : Form
    {
        private int ROWS;
        private int COLS;
        private int CELL_SIZE;
        public Button[,] cells;
        Label lblFlags = new Label();
        Button btnReset;

        Timer timer = new Timer();
        Form1 form1;

        public GraphicDesign(int rOWS, int cOLS, int cELL_SIZE)
        {
            ROWS = rOWS;
            COLS = cOLS;
            CELL_SIZE = cELL_SIZE;
            cells = new Button[ROWS, COLS];
        }

        public void BuildBoard(Form1 form)
        {
            form1 = form;
            form.Text = "Minesweeper";
            form.ClientSize = new Size(COLS * CELL_SIZE, ROWS * CELL_SIZE + 60);
            form.FormBorderStyle = FormBorderStyle.FixedSingle;
            form.MaximizeBox = false;

            // ── Status traka na vrhu ──────────────────────────────
            Panel topPanel = new Panel();
            topPanel.Size = new Size(COLS * CELL_SIZE, 50);
            topPanel.Location = new Point(0, 0);
            topPanel.BackColor = Color.Silver;
            form.Controls.Add(topPanel);

            lblFlags.Text = "🚩 " + LogicManager.bombs;
            lblFlags.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblFlags.Location = new Point(10, 10);
            lblFlags.AutoSize = true;
            topPanel.Controls.Add(lblFlags);

            btnReset = new Button();
            btnReset.Text = "🙂";
            btnReset.Font = new Font("Segoe UI", 14);
            btnReset.Size = new Size(45, 35);
            btnReset.Location = new Point((COLS * CELL_SIZE) / 2 - 22, 7);
            btnReset.FlatStyle = FlatStyle.Flat;
            topPanel.Controls.Add(btnReset);
            btnReset.MouseDown += ClickResetButton;

            Label lblTimer = new Label();
            lblTimer.Text = "⏱ 0";
            lblTimer.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTimer.Location = new Point(COLS * CELL_SIZE - 80, 10);
            lblTimer.AutoSize = true;
            topPanel.Controls.Add(lblTimer);
            int seconds = 0;
            timer = new System.Windows.Forms.Timer();
            timer.Interval = 1000;
            timer.Tick += (s, e) =>
            {
                seconds++;
                lblTimer.Text = "⏱ " + seconds.ToString();
            };
            timer.Start();

            // ── Tabla sa ćelijama ─────────────────────────────────
            Panel boardPanel = new Panel();
            boardPanel.Location = new Point(0, 50);
            boardPanel.Size = new Size(COLS * CELL_SIZE, ROWS * CELL_SIZE);
            form.Controls.Add(boardPanel);

            for (int row = 0; row < ROWS; row++)
            {
                for (int col = 0; col < COLS; col++)
                {
                    Button btn = new Button();
                    btn.Size = new Size(CELL_SIZE, CELL_SIZE);
                    btn.Location = new Point(col * CELL_SIZE, row * CELL_SIZE);
                    btn.Font = new Font("Segoe UI", 11, FontStyle.Bold);
                    btn.FlatStyle = FlatStyle.Standard;
                    btn.BackColor = Color.LightGray;
                    btn.Tag = new Point(row, col); // čuva poziciju

                    // Placeholder event handleri
                    btn.MouseDown += Cell_MouseDown;

                    cells[row, col] = btn;
                    btn.TabStop = false;
                    boardPanel.Controls.Add(btn);
                }
            }
        }

        private void Cell_MouseDown(object sender, MouseEventArgs e)
        {
            // pronalazi koordinate button-a
            Button btn = (Button)sender;
            

            Point pos = (Point)btn.Tag;
            int row = 0, col = 0;
            for(int i = 0; i < ROWS; i++)
            {
                for(int j = 0; j < COLS; j++)
                {
                    if(btn == cells[i, j])
                    {
                        row = i;
                        col = j;
                        break;
                    }
                }
            }

            if (e.Button == MouseButtons.Left)
            {
                // logika otkrivanja
                int value = LogicManager.layout[row, col];
                if (btn.Text == "🚩") return;
                if(value == -1)
                {
                    // Igra je zavrsena
                    btn.Text = "💣";
                    btnReset.Text = "😢";
                    LogicManager.GameOver();
                    timer.Stop();
                    for(int i = 0; i < ROWS; i++)
                    {
                        for(int j = 0; j < COLS; j++)
                        {   
                            if(LogicManager.layout[i, j] == -1 && cells[i, j].Text == "🚩")
                            {

                            }
                            else if (LogicManager.layout[i, j] == -1)
                            {
                                cells[i, j].Text = "💣";
                                cells[i, j].BackColor = Color.White;
                            }
                            else if (cells[i, j].Text == "🚩")
                            {
                                cells[i, j].BackColor = Color.LightPink;
                            }
                            cells[i, j].Enabled = false;
                        }
                    }
                    btn.BackColor = Color.LightPink;
                }
                else if (value == 0)
                {
                    btn.Text = "";
                    List<int> susedneNule = LogicManager.SusedneNule(row, col);
                    // otvara sva poslednja polja
                    int n = susedneNule.Count;
                    for(int i = 0; i < n; i += 2)
                    {
                        if (cells[susedneNule[i], susedneNule[i + 1]].Text == "🚩") continue;
                        if (LogicManager.layout[susedneNule[i], susedneNule[i + 1]] == 0)
                            cells[susedneNule[i], susedneNule[i + 1]].Text = "";
                        else
                            cells[susedneNule[i], susedneNule[i + 1]].Text = LogicManager.layout[susedneNule[i], susedneNule[i + 1]].ToString();
                        cells[susedneNule[i], susedneNule[i + 1]].BackColor = Color.White;
                        cells[susedneNule[i], susedneNule[i + 1]].Enabled = false;
                    }
                    btn.BackColor = Color.White;
                }
                else
                {
                    btn.Text = value.ToString();
                    btn.BackColor = Color.White;
                }
                btn.Enabled = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                // logika zastave
                if (btn.Text == "🚩")
                {
                    btn.Text = "";
                    LogicManager.bombs++;
                    lblFlags.Text = "🚩 " + LogicManager.bombs;
                }
                else
                {
                    btn.Text = "🚩"; 
                    LogicManager.bombs--;
                    lblFlags.Text = "🚩 " + LogicManager.bombs;
                    // provera pobede
                    for (int i = 0; i < ROWS; i++)
                    {
                        for (int j = 0; j < COLS; j++)
                        {
                            if (LogicManager.layout[i, j] == -1 && cells[i, j].Text != "🚩")
                            {
                                return;
                            }
                            if (LogicManager.layout[i, j] != -1 && cells[i, j].Enabled == true)
                            {
                                return;
                            }
                        }
                    }
                    LogicManager.GameWon();
                    btnReset.Text = "😎";
                    for(int i = 0; i < ROWS; i++)
                    {
                        for(int j = 0; j < COLS; j++)
                        {
                            cells[i, j].Enabled = false;
                        }
                    }
                }
            }
        }

        private void ClickResetButton(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LogicManager.ResetGame();
            }
        }
    }


    public class LogicManager
    {
        private static int ROWS;
        private static int COLS;
        private static int CELL_SIZE;
        public static int[,] layout;
        public static int bombs = 0;
        private static bool gameOver = false;
        private static bool gameWon = false;

        static Form1 form1;
        static GraphicDesign tabla;

        public LogicManager(int rOWS, int cOLS, int cELL_SIZE, Form1 form, GraphicDesign tabla1)
        {
            ROWS = rOWS;
            COLS = cOLS;
            CELL_SIZE = cELL_SIZE;
            layout = new int[rOWS, cOLS]; // -1 za bombe, 0 za prazna polja, 1-8 za broj susednih
            Rasporedi(layout);
            form1 = form;
            tabla = tabla1;
        }
        public LogicManager()
        {

        }

        private static int PrebrojSusedne(int x, int y)
        {
            int br = 0;
            for (int i = x - 1; i <= x + 1; i++)
            {
                for (int j = y - 1; j <= y + 1; j++)
                {
                    if (i == x && j == y) continue;
                    if (i < 0 || j < 0) continue;
                    if (i > ROWS - 1 || j > COLS - 1) continue;
                    if (layout[i, j] == -1) br++;
                }
            }
            return br;
        }
        public static void Rasporedi(int[,] layout)
        {
            Random rng = new Random();
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    int result = rng.Next(0, 4) == 0 ? 1 : 0;
                    if (result == 1)
                    {
                        layout[i, j] = -1;
                        bombs++;
                    }
                }
            }
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLS; j++)
                {
                    if (layout[i, j] != -1)
                    {
                        layout[i, j] = PrebrojSusedne(i, j);
                    }
                }
            }
        }


        private static List<int> Susedni(int row, int col)
        {
            List<int> susedni = new List<int>();
            for(int i = row - 1; i <= row + 1; i++)
            {
                for(int j = col - 1; j <= col + 1; j++)
                {
                    if (i == row && j == col) continue;
                    if (i < 0 || j < 0) continue;
                    if (i > ROWS - 1 || j > COLS - 1) continue;
                    susedni.Add(i); susedni.Add(j);
                }
            }
            return susedni;
        }
        public static List<int> SusedneNule(int row, int col)
        {
            List<int> susedneNule = new List<int>();
            // polje koje je nula dodajemo u red
            Queue<int> redRow = new Queue<int>();
            Queue<int> redCol = new Queue<int>();
            redRow.Enqueue(row);
            redCol.Enqueue(col);
            // svi su na poctku nule sem pocetnog
            int[,] poseceni = new int[ROWS, COLS];
            for(int i = 0; i < ROWS; i++)
            {
                for(int j = 0; j < COLS; j++)
                {
                    poseceni[i, j] = 0;
                }
            }
            poseceni[row, col] = 1;

            while(redRow.Count > 0 && redCol.Count > 0)
            {
                // i, i+1 su koordinate polja
                List<int> susedni = Susedni(redRow.Dequeue(), redCol.Dequeue());
                int n = susedni.Count;
                for(int i = 0; i < n; i += 2) // prolazi kroz susedne
                {
                    if (poseceni[susedni[i], susedni[i + 1]] == 0)
                    {
                        poseceni[susedni[i], susedni[i + 1]] = 1;
                        susedneNule.Add(susedni[i]);
                        susedneNule.Add(susedni[i + 1]);
                        if (layout[susedni[i], susedni[i + 1]] == 0)
                        {
                            redRow.Enqueue(susedni[i]);
                            redCol.Enqueue(susedni[i + 1]);
                        }
                    }
                }
            }
            return susedneNule;
        }

        public static void GameOver()
        {
            gameOver = true;
        }

        public static void GameWon()
        {
            gameWon = true;
        }

        public static void ResetGame()
        {
            form1.Controls.Clear();

            bombs = 0;
            gameOver = false;
            gameWon = false;
            layout = new int[ROWS, COLS];
            
            Rasporedi(layout);
            tabla.BuildBoard(form1);
        }

    }




    public partial class Form1 : Form
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int CELL_SIZE = 40;

        GraphicDesign tabla = new GraphicDesign(ROWS, COLS, CELL_SIZE);
        
        public Form1()
        {
            InitializeComponent();
            LogicManager logic = new LogicManager(ROWS, COLS, CELL_SIZE, this, tabla);
            tabla.BuildBoard(this);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}