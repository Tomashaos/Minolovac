using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace WindowsFormsApplication3
{
    public partial class Form1 : Form
    {
        private const int ROWS = 10;
        private const int COLS = 10;
        private const int CELL_SIZE = 40;

        private Button[,] cells = new Button[ROWS, COLS];

        public Form1()
        {
            InitializeComponent();
            BuildBoard();
        }

        private void BuildBoard()
        {
            this.Text = "Minesweeper";
            this.ClientSize = new Size(COLS * CELL_SIZE, ROWS * CELL_SIZE + 60);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            // ── Status traka na vrhu ──────────────────────────────
            Panel topPanel = new Panel();
            topPanel.Size = new Size(COLS * CELL_SIZE, 50);
            topPanel.Location = new Point(0, 0);
            topPanel.BackColor = Color.Silver;
            this.Controls.Add(topPanel);

            Label lblMines = new Label();
            lblMines.Text = "💣 10";
            lblMines.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblMines.Location = new Point(10, 10);
            lblMines.AutoSize = true;
            topPanel.Controls.Add(lblMines);

            Button btnReset = new Button();
            btnReset.Text = "🙂";
            btnReset.Font = new Font("Segoe UI", 14);
            btnReset.Size = new Size(45, 35);
            btnReset.Location = new Point((COLS * CELL_SIZE) / 2 - 22, 7);
            btnReset.FlatStyle = FlatStyle.Flat;
            topPanel.Controls.Add(btnReset);

            Label lblTimer = new Label();
            lblTimer.Text = "⏱ 0";
            lblTimer.Font = new Font("Segoe UI", 14, FontStyle.Bold);
            lblTimer.Location = new Point(COLS * CELL_SIZE - 80, 10);
            lblTimer.AutoSize = true;
            topPanel.Controls.Add(lblTimer);

            // ── Tabla sa ćelijama ─────────────────────────────────
            Panel boardPanel = new Panel();
            boardPanel.Location = new Point(0, 50);
            boardPanel.Size = new Size(COLS * CELL_SIZE, ROWS * CELL_SIZE);
            this.Controls.Add(boardPanel);

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

                    // Placeholder event handleri (logika dolazi kasnije)
                    btn.MouseDown += Cell_MouseDown;

                    cells[row, col] = btn;
                    boardPanel.Controls.Add(btn);
                }
            }
        }

        // Levi klik = otkriva ćeliju | Desni klik = zastava
        private void Cell_MouseDown(object sender, MouseEventArgs e)
        {
            Button btn = (Button)sender;
            Point pos = (Point)btn.Tag;

            if (e.Button == MouseButtons.Left)
            {
                // TODO: logika otkrivanja
                btn.Text = "";
                btn.BackColor = Color.White;
                btn.Enabled = false;
            }
            else if (e.Button == MouseButtons.Right)
            {
                // TODO: logika zastave
                if (btn.Text == "🚩")
                    btn.Text = "";
                else
                    btn.Text = "🚩";
            }
        }
    }
}