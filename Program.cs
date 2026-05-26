// See https://aka.ms/new-console-template for more information
using System;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

// bar untuk sorting dan knapsack
const int ItemCount = 15;

// delay untuk visualisasi perbandingan
const int DelayMs = 60;

CancellationTokenSource? _cancellationTokenSource = null;
int _unoptimizedOps = 0;
int _optimizedOps = 0;
double _maxWeight = 0; 
double _unoptimizedProfit = 0;
double _optimizedProfit = 0;

KnapsackItem[] UnoptimizedItems = new KnapsackItem[ItemCount];
KnapsackItem[] OptimizedItems = new KnapsackItem[ItemCount];

// inisialisasi Gui
Visual.InitializeGUI(ItemCount);
Visual.MainWindow.Load += (s, e) => StartSimulation();
Visual.RestartButton.Click += (s, e) => StartSimulation();
Application.Run(Visual.MainWindow);


async void StartSimulation()
{
    _cancellationTokenSource?.Cancel();
    _cancellationTokenSource = new CancellationTokenSource();
    var token = _cancellationTokenSource.Token;

    GenerateRandomItems();
    
    // Reset Counter Text dan Status
    _unoptimizedOps = 0;
    _optimizedOps = 0;
    _unoptimizedProfit = 0;
    _optimizedProfit = 0;
    
    UpdateHUD(0, 0);

    try
    {
        var task1 = RunUnoptimizedGreedy(token);
        var task2 = RunOptimizedGreedy(token);

        await Task.WhenAll(task1, task2);
    }
    catch (OperationCanceledException) { }
}

void GenerateRandomItems()
{
    var random = new Random();
    double totalW = 0;
    KnapsackItem[] tempItems = new KnapsackItem[ItemCount];

    for (int i = 0; i < ItemCount; i++)
    {
        double weight = random.Next(10, 45); 
        double profit = random.Next(40, 250);
        totalW += weight;
        Color itemColor = Color.FromArgb(random.Next(100, 255), random.Next(100, 255), random.Next(100, 255));
        tempItems[i] = new KnapsackItem(i, profit, weight, itemColor);
    }
    
    _maxWeight = totalW * 0.45; 
    
    // Perhitungan Pixel
    double pixelPerWeight = 360.0 / _maxWeight;
    
    Visual.UnoptKnapsackFill.Height = 0;
    Visual.OptKnapsackFill.Height = 0;
    
    Visual.UnoptimizedCanvas.Controls.Clear();
    Visual.OptimizedCanvas.Controls.Clear();
    Visual.UnoptimizedCanvas.Controls.Add(Visual.UnoptKnapsackBg);
    Visual.OptimizedCanvas.Controls.Add(Visual.OptKnapsackBg);
    Visual.UnoptimizedCanvas.Controls.Add(Visual.UnoptCapText);
    Visual.OptimizedCanvas.Controls.Add(Visual.OptCapText);

    for (int i = 0; i < ItemCount; i++)
    {
        UnoptimizedItems[i] = tempItems[i];
        OptimizedItems[i] = tempItems[i];
        
        int scaledHeightPix = (int)Math.Round(tempItems[i].Weight * pixelPerWeight);
        scaledHeightPix = Math.Max(15, scaledHeightPix); 

        Visual.UnoptimizedItems[i] = CreateItemVisual(tempItems[i], i, scaledHeightPix);
        Visual.OptimizedItems[i] = CreateItemVisual(tempItems[i], i, scaledHeightPix);
        
        Visual.UnoptimizedCanvas.Controls.Add(Visual.UnoptimizedItems[i]);
        Visual.OptimizedCanvas.Controls.Add(Visual.OptimizedItems[i]);
    }
}

Label CreateItemVisual(KnapsackItem item, int index, int h)
{
    int col = index % 4;
    int row = index / 4;
    int xLoc = 110 + (col * 70); 
    int yLoc = 30 + (row * 90); 

    return new Label
    {
        Size = new Size(55, h), 
        Location = new Point(xLoc, yLoc),
        BackColor = item.Color,
        ForeColor = Color.Black,
        Text = $"{Math.Round(item.Ratio, 1)}",
        TextAlign = ContentAlignment.MiddleCenter,
        Font = new Font("Consolas", 8.5f, FontStyle.Bold)
    };
}

// unoptimized greedy (Iterasi Linear berulang mencari kandidat terbaik O(n^2))
async Task RunUnoptimizedGreedy(CancellationToken token)
{
    double currentWeight = 0;
    bool[] usedItems = new bool[ItemCount];

    while (currentWeight < _maxWeight)
    {
        int bestIdx = -1;
        double bestRatio = -1;

        // Iterasi pencarian linear
        for (int i = 0; i < ItemCount; i++)
        {
            if (usedItems[i]) continue;
            token.ThrowIfCancellationRequested();
            
            // increment dan update gui
            _unoptimizedOps++;
            
            Visual.MainWindow.Invoke(() => { 
                Visual.UnoptimizedItems[i].BackColor = Color.White;
                UpdateHUD(currentWeight, -1); 
            });
            await Task.Delay(DelayMs / 2, token); 
            Visual.MainWindow.Invoke(() => { 
                Visual.UnoptimizedItems[i].BackColor = UnoptimizedItems[i].Color; 
            });

            if (UnoptimizedItems[i].Ratio > bestRatio)
            {
                bestRatio = UnoptimizedItems[i].Ratio;
                bestIdx = i;
            }
        }

        if (bestIdx == -1) break; 
        usedItems[bestIdx] = true;
        
        double remain = _maxWeight - currentWeight;
        double fraction = Math.Min(UnoptimizedItems[bestIdx].Weight, remain) / UnoptimizedItems[bestIdx].Weight;
        
        currentWeight += UnoptimizedItems[bestIdx].Weight * fraction;
        _unoptimizedProfit += UnoptimizedItems[bestIdx].Profit * fraction;
        
        Visual.MainWindow.Invoke(() => 
        {
            Visual.UnoptimizedItems[bestIdx].Visible = false;
            
            int fillPixel = (int)((currentWeight / _maxWeight) * 360);
            Visual.UnoptKnapsackFill.Height = fillPixel;
            Visual.UnoptKnapsackFill.Top = 360 - fillPixel;
            
            UpdateHUD(currentWeight, -1);
        });
        await Task.Delay(DelayMs * 3, token);
    }
}

// optimized Greedy (pre sorting dengan QuickSort O(n log n))
async Task RunOptimizedGreedy(CancellationToken token)
{
    await QuickSortRatio(0, ItemCount - 1, token);

    double currentWeight = 0;
    
    // Pengambilan kandidat langsung O(n) tanpa searching
    for(int i = 0; i < ItemCount; i++)
    {
        if (currentWeight >= _maxWeight) break;
        token.ThrowIfCancellationRequested();
        
        // Increment operasi dan update gui
        _optimizedOps++;
        
        double remain = _maxWeight - currentWeight;
        double fraction = Math.Min(OptimizedItems[i].Weight, remain) / OptimizedItems[i].Weight;

        currentWeight += OptimizedItems[i].Weight * fraction;
        _optimizedProfit += OptimizedItems[i].Profit * fraction;
        
        Visual.MainWindow.Invoke(() => 
        {
            Visual.OptimizedItems[i].Visible = false; 
            
            int fillPixel = (int)((currentWeight / _maxWeight) * 360);
            Visual.OptKnapsackFill.Height = fillPixel; 
            Visual.OptKnapsackFill.Top = 360 - fillPixel;
            
            UpdateHUD(-1, currentWeight);
        });
        
        await Task.Delay(DelayMs * 3, token);
    }
}

async Task QuickSortRatio(int left, int right, CancellationToken token)
{
    if (left < right)
    {
        int p = await Partition(left, right, token);
        await QuickSortRatio(left, p - 1, token);
        await QuickSortRatio(p + 1, right, token);
    }
}

async Task<int> Partition(int left, int right, CancellationToken token)
{
    double pivot = OptimizedItems[right].Ratio;
    int i = left - 1;

    for (int j = left; j < right; j++)
    {
        token.ThrowIfCancellationRequested();
        
        // Increment operasi, update gui
        _optimizedOps++;
        UpdateHUD(-1, -1);
        
        Visual.MainWindow.Invoke(() => { Visual.OptimizedItems[j].BackColor = Color.White; });
        await Task.Delay(DelayMs, token);
        Visual.MainWindow.Invoke(() => { Visual.OptimizedItems[j].BackColor = OptimizedItems[j].Color; });

        if (OptimizedItems[j].Ratio > pivot)
        {
            i++;
            SwapOptimized(i, j);
        }
    }
    SwapOptimized(i + 1, right);
    return i + 1;
}

void SwapOptimized(int a, int b)
{
    var temp = OptimizedItems[a];
    OptimizedItems[a] = OptimizedItems[b];
    OptimizedItems[b] = temp;
    
    Visual.MainWindow.Invoke(() => {
        Point pA = Visual.OptimizedItems[a].Location;
        Point pB = Visual.OptimizedItems[b].Location;
        Visual.OptimizedItems[a].Location = pB;
        Visual.OptimizedItems[b].Location = pA;
        
        var tempV = Visual.OptimizedItems[a];
        Visual.OptimizedItems[a] = Visual.OptimizedItems[b];
        Visual.OptimizedItems[b] = tempV;
    });
}

void UpdateHUD(double c1 = -1, double c2 = -1)
{
    Visual.MainWindow.Invoke(() => {
        Visual.UnoptimizedCounterText.Text = $"Operasi: {_unoptimizedOps} | Profit: ${Math.Round(_unoptimizedProfit, 1)}";
        Visual.OptimizedCounterText.Text = $"Operasi: {_optimizedOps} | Profit: ${Math.Round(_optimizedProfit, 1)}";
        
        if(c1 != -1) Visual.UnoptCapText.Text = $"{Math.Round(c1, 1)} / {Math.Round(_maxWeight, 1)}";
        if(c2 != -1) Visual.OptCapText.Text = $"{Math.Round(c2, 1)} / {Math.Round(_maxWeight, 1)}";
    });
}

public struct KnapsackItem
{
    public int Id;
    public double Profit;
    public double Weight;
    public double Ratio;
    public Color Color;

    public KnapsackItem(int id, double profit, double weight, Color color)
    {
        Id = id;
        Profit = profit;
        Weight = weight;
        Ratio = profit / weight;
        Color = color;
    }
}