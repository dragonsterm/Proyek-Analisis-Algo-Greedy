using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

public static class Visual
{
    public static Form MainWindow = null!;
    public static Panel UnoptimizedCanvas = null!;
    public static Panel OptimizedCanvas = null!;
    public static Label UnoptimizedCounterText = null!;
    public static Label OptimizedCounterText = null!;
    public static Button RestartButton = null!;

    public static Label[] UnoptimizedItems = null!;
    public static Label[] OptimizedItems = null!;
    
    public static Panel UnoptKnapsackBg = null!;
    public static Panel OptKnapsackBg = null!;
    public static Panel UnoptKnapsackFill = null!;
    public static Panel OptKnapsackFill = null!;
    
    public static Label UnoptCapText = null!;
    public static Label OptCapText = null!;

    public static void EnableDoubleBuffered(Control control)
    {
        typeof(Control).InvokeMember("DoubleBuffered",
            BindingFlags.SetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
            null, control, new object[] { true });
    }

    public static void InitializeGUI(int itemCount)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        MainWindow = new Form
        {
            Text = "Greedy Algorithm Visualization",
            Size = new Size(1000, 930),
            BackColor = Color.FromArgb(13, 13, 17),
            StartPosition = FormStartPosition.CenterScreen,
            FormBorderStyle = FormBorderStyle.FixedDialog
        };
        EnableDoubleBuffered(MainWindow);

        Label title = new Label { Text = "Knapsack Optimization (Greedy)", ForeColor = Color.White, Font = new Font("Segoe UI", 24, FontStyle.Bold), AutoSize = false, Size = new Size(1000, 50), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(0, 15) };
        MainWindow.Controls.Add(title);

        Label unoptTitle = new Label { Text = "Unoptimized (O(n²))", ForeColor = Color.FromArgb(255, 76, 76), AutoSize = false, Size = new Size(400, 30), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16), Location = new Point(50, 80) };
        Label optTitle = new Label { Text = "Optimized (O(n log n))", ForeColor = Color.FromArgb(76, 255, 76), AutoSize = false, Size = new Size(400, 30), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 16), Location = new Point(550, 80) };
        MainWindow.Controls.Add(unoptTitle);
        MainWindow.Controls.Add(optTitle);

        UnoptimizedCanvas = new Panel { Size = new Size(400, 400), Location = new Point(50, 120), BackColor = Color.FromArgb(20, 20, 20) };
        OptimizedCanvas = new Panel { Size = new Size(400, 400), Location = new Point(550, 120), BackColor = Color.FromArgb(20, 20, 20) };
        EnableDoubleBuffered(UnoptimizedCanvas);
        EnableDoubleBuffered(OptimizedCanvas);
        MainWindow.Controls.Add(UnoptimizedCanvas);
        MainWindow.Controls.Add(OptimizedCanvas);

        UnoptCapText = new Label { Text = "0 / 100", ForeColor = Color.White, AutoSize = false, Size = new Size(100, 20), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(5, 5) };
        OptCapText = new Label { Text = "0 / 100", ForeColor = Color.White, AutoSize = false, Size = new Size(100, 20), TextAlign = ContentAlignment.MiddleCenter, Location = new Point(5, 5) };
        UnoptimizedCanvas.Controls.Add(UnoptCapText);
        OptimizedCanvas.Controls.Add(OptCapText);

        UnoptKnapsackBg = new Panel { Size = new Size(70, 360), Location = new Point(20, 30), BackColor = Color.FromArgb(30, 30, 40), BorderStyle = BorderStyle.FixedSingle };
        OptKnapsackBg = new Panel { Size = new Size(70, 360), Location = new Point(20, 30), BackColor = Color.FromArgb(30, 30, 40), BorderStyle = BorderStyle.FixedSingle };
        UnoptimizedCanvas.Controls.Add(UnoptKnapsackBg);
        OptimizedCanvas.Controls.Add(OptKnapsackBg);

        UnoptKnapsackFill = new Panel { Size = new Size(70, 0), Location = new Point(0, 360), BackColor = Color.FromArgb(255, 76, 76) };
        OptKnapsackFill = new Panel { Size = new Size(70, 0), Location = new Point(0, 360), BackColor = Color.FromArgb(76, 255, 76) };
        UnoptKnapsackBg.Controls.Add(UnoptKnapsackFill);
        OptKnapsackBg.Controls.Add(OptKnapsackFill);

        UnoptimizedCounterText = new Label { Text = "Operasi: 0 | Profit: 0", ForeColor = Color.FromArgb(255, 76, 76), AutoSize = false, Size = new Size(400, 30), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(50, 530) };
        OptimizedCounterText = new Label { Text = "Operasi: 0 | Profit: 0", ForeColor = Color.FromArgb(76, 255, 76), AutoSize = false, Size = new Size(400, 30), TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 14, FontStyle.Bold), Location = new Point(550, 530) };
        MainWindow.Controls.Add(UnoptimizedCounterText);
        MainWindow.Controls.Add(OptimizedCounterText);

        RestartButton = new Button { Text = "Restart Simulation", BackColor = Color.FromArgb(122, 162, 247), ForeColor = Color.FromArgb(22, 22, 30), Font = new Font("Segoe UI", 14, FontStyle.Bold), Size = new Size(200, 45), Location = new Point(400, 575), FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand };
        RestartButton.FlatAppearance.BorderSize = 0;
        MainWindow.Controls.Add(RestartButton);

        UnoptimizedItems = new Label[itemCount];
        OptimizedItems = new Label[itemCount];

        RichTextBox codeBoxUnopt = AddCodeBoxToForm(new Point(50, 640));
        AppendText(codeBoxUnopt, "function ", Color.FromArgb(247, 118, 142));
        AppendText(codeBoxUnopt, "greedyStandard", Color.FromArgb(122, 162, 247));
        AppendText(codeBoxUnopt, "(C, MaxW)\n", Color.FromArgb(169, 177, 214));
        AppendText(codeBoxUnopt, "// O(n²) - Mencari nilai tertinggi tiap loop\n", Color.FromArgb(86, 95, 137));
        AppendText(codeBoxUnopt, "while w_total < MaxW and C != {} do\n", Color.FromArgb(187, 154, 247));
        AppendText(codeBoxUnopt, "    x <- GetBestRatioItem", Color.FromArgb(125, 207, 255));
        AppendText(codeBoxUnopt, "(C)\n", Color.FromArgb(169, 177, 214));
        AppendText(codeBoxUnopt, "    if W[x] <= sisa_kapasitas then\n", Color.FromArgb(187, 154, 247));
        AppendText(codeBoxUnopt, "        MasukanBarang(x)\n", Color.FromArgb(169, 177, 214));
        AppendText(codeBoxUnopt, "    else\n", Color.FromArgb(187, 154, 247));
        AppendText(codeBoxUnopt, "        MasukanPecahanBarang(x)\n", Color.FromArgb(169, 177, 214));
        AppendText(codeBoxUnopt, "    C <- C - {x}\n", Color.FromArgb(169, 177, 214));

        RichTextBox codeBoxOpt = AddCodeBoxToForm(new Point(550, 640));
        AppendText(codeBoxOpt, "function ", Color.FromArgb(247, 118, 142));
        AppendText(codeBoxOpt, "greedyOptimized", Color.FromArgb(122, 162, 247));
        AppendText(codeBoxOpt, "(C, MaxW)\n", Color.FromArgb(169, 177, 214));
        AppendText(codeBoxOpt, "// O(n log n) - Di urutkan 1x di awal\n", Color.FromArgb(86, 95, 137));
        AppendText(codeBoxOpt, "QuickSortRatioDesc", Color.FromArgb(125, 207, 255));
        AppendText(codeBoxOpt, "(C)\n", Color.FromArgb(169, 177, 214));
        AppendText(codeBoxOpt, "foreach x in C do\n", Color.FromArgb(187, 154, 247));
        AppendText(codeBoxOpt, "    if w_total == MaxW then break\n", Color.FromArgb(187, 154, 247));
        AppendText(codeBoxOpt, "    if W[x] <= sisa_kapasitas then\n", Color.FromArgb(187, 154, 247));
        AppendText(codeBoxOpt, "        MasukanBarang(x)\n", Color.FromArgb(169, 177, 214));
        AppendText(codeBoxOpt, "    else\n", Color.FromArgb(187, 154, 247));
        AppendText(codeBoxOpt, "        MasukanPecahanBarang(x)\n", Color.FromArgb(169, 177, 214));
    }

    private static RichTextBox AddCodeBoxToForm(Point location)
    {
        Panel container = new Panel { Size = new Size(400, 230), Location = location, BackColor = Color.FromArgb(22, 22, 30), Padding = new Padding(15) };
        RichTextBox rtb = new RichTextBox
        {
            BackColor = Color.FromArgb(22, 22, 30), ForeColor = Color.White, ReadOnly = true, BorderStyle = BorderStyle.None, Font = new Font("Consolas", 10.5f), Dock = DockStyle.Fill, ScrollBars = RichTextBoxScrollBars.None
        };
        container.Controls.Add(rtb);
        MainWindow.Controls.Add(container);
        return rtb;
    }

    private static void AppendText(RichTextBox box, string text, Color color)
    {
        box.SelectionStart = box.TextLength;
        box.SelectionLength = 0;
        box.SelectionColor = color;
        box.AppendText(text);
    }
}