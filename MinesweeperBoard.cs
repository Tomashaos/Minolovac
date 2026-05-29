using System;
using System.Drawing;
using System.Windows.Forms;

public class MinesweeperBoard : Form
{
    private const int ROWS = 10;
    private const int COLS = 10;
    private const int CELL_SIZE = 50;
    private const int PADDING = 10;

    public Button[,] Cells { get; private set; }

    public MinesweeperBoard()
    {
        InitializeForm();
        CreateBoard();
    }

    private void InitializeForm()
    {
        this.Text = "Minesweeper";
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.ClientSize = new Size(
            COLS * CELL_SIZE + PADDING * 2,
            ROWS * CELL_SIZE + PADDING * 2
        );
        this.BackColor = Color.FromArgb(255, 255, 255);
    }

    private void CreateBoard()
    {
        Cells = new Button[ROWS, COLS];

        for (int row = 0; row < ROWS; row++)
        {
            for (int col = 0; col < COLS; col++)
            {
                var btn = new Button
                {
                    Size = new Size(CELL_SIZE - 2, CELL_SIZE - 2),
                    Location = new Point(
                        PADDING + col * CELL_SIZE,
                        PADDING + row * CELL_SIZE
                    ),
                    Tag = new Point(row, col),
                    Font = new Font("Arial", 12, FontStyle.Bold),
                    BackColor = Color.FromArgb(192, 192, 192),
                    FlatStyle = FlatStyle.Standard,
                    TabStop = false
                };

                // Čuvamo row i col lokalno za lambda
                int r = row, c = col;

                btn.MouseDown += (sender, e) => OnCellMouseDown(r, c, e);

                Cells[row, col] = btn;
                this.Controls.Add(btn);
            }
        }
    }

    // Ovde ćeš dodati logiku igre
    private void OnCellMouseDown(int row, int col, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            // TODO: Logika levog klika (otkrivanje polja)
            Console.WriteLine("Levi klik: [{row}, {col}]");
        }
        else if (e.Button == MouseButtons.Right)
        {
            // TODO: Logika desnog klika (zastavica)
            Console.WriteLine("Desni klik: [{row}, {col}]");
        }
    }

    [STAThread]
    static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new MinesweeperBoard());
    }
}
