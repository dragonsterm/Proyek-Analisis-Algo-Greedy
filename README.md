# Project Analisis Algoritma IF-D

---

### Anggota
1. Jauza Ilham Maharhdika Putra (123240174)
2. Mufid Dhamarjati Kusuma (123240171)
3. Dimas Hafid Fathoni (123240159)

---

### Algoritma Greedy
**Algoritma Greedy** adalah pendekatan pemecahan masalah yang membuat pilihan terbaik secara lokal (paling menguntungkan saat itu juga) pada setiap langkahnya, dengan harapan bahwa pilihan-pilihan tersebut akan mengarah pada solusi terbaik secara keseluruhan (global optimal). Algoritma ini mengambil keputusan yang tidak bisa ditarik kembali (*no backtracking*).

---

### Fractional Knapsack Problem
**Knapsack Problem** adalah masalah optimasi di mana kita memiliki tas (knapsack) dengan kapasitas berat maksimal, dan sejumlah barang yang memiliki berat dan nilai (profit). Tujuannya adalah memasukkan barang ke dalam tas untuk mendapatkan total profit sebesar mungkin. 

Pada variasi **Fractional**, jika sebuah barang tidak muat sepenuhnya ke dalam tas, kita diperbolehkan memotong atau mengambil sebagian dari barang tersebut (pecahan/fraksi) agar ruang tas yang tersisa terisi penuh. Strategi paling efisien untuk masalah ini adalah dengan mengutamakan barang yang memiliki **rasio profit per berat tertinggi**.

---

### Implementasi Kode

Berikut adalah logika dasar dari algoritma yang digunakan dalam proyek ini, dibersihkan dari kode GUI dan animasi visual:

#### Struktur Data Item

```csharp
public struct KnapsackItem
{
    public int Id;
    public double Profit;
    public double Weight;
    public double Ratio; // Profit dibagi Weight

    public KnapsackItem(int id, double profit, double weight)
    {
        Id = id;
        Profit = profit;
        Weight = weight;
        Ratio = profit / weight;
    }
}
```

####  Unoptimized (Kompleksitas $O(n^2)$)

Algoritma mencari barang dengan rasio terbaik secara berulang melalui pencarian linear pada setiap iterasi, sehingga kurang efisien jika jumlah datanya besar.

```csharp
public void RunUnoptimizedGreedy(KnapsackItem[] items, double maxWeight)
{
    double currentWeight = 0;
    double totalProfit = 0;
    bool[] usedItems = new bool[items.Length];

    while (currentWeight < maxWeight)
    {
        int bestIdx = -1;
        double bestRatio = -1;

        // Iterasi pencarian linear untuk mencari rasio terbaik yang belum diambil
        for (int i = 0; i < items.Length; i++)
        {
            if (!usedItems[i] && items[i].Ratio > bestRatio)
            {
                bestRatio = items[i].Ratio;
                bestIdx = i;
            }
        }

        // Jika tidak ada barang lagi yang bisa diambil, hentikan loop
        if (bestIdx == -1) break; 
        
        usedItems[bestIdx] = true;
        
        // Hitung apakah barang bisa masuk semua atau harus dipotong
        double remain = maxWeight - currentWeight;
        double fraction = Math.Min(items[bestIdx].Weight, remain) / items[bestIdx].Weight;
        
        currentWeight += items[bestIdx].Weight * fraction;
        totalProfit += items[bestIdx].Profit * fraction;
    }
}
```

#### Optimized (Kompleksitas $O(n \log n)$)

Algoritma mengurutkan (sorting) seluruh barang terlebih dahulu berdasarkan rasio profit/beratnya (misal menggunakan QuickSort atau algoritma bawaan). Setelah diurutkan, pengambilan barang dilakukan dalam satu kali putaran (looping).

```csharp
public void RunOptimizedGreedy(KnapsackItem[] items, double maxWeight)
{
    // Pre-sorting: Urutkan item berdasarkan Rasio secara Descending (O(n log n))
    Array.Sort(items, (a, b) => b.Ratio.CompareTo(a.Ratio));

    double currentWeight = 0;
    double totalProfit = 0;
    
    // Pengambilan kandidat langsung dari array yang sudah terurut (O(n))
    for(int i = 0; i < items.Length; i++)
    {
        // Jika tas sudah penuh, hentikan iterasi
        if (currentWeight >= maxWeight) break;
        
        // Hitung apakah barang bisa masuk semua atau harus dipotong
        double remain = maxWeight - currentWeight;
        double fraction = Math.Min(items[i].Weight, remain) / items[i].Weight;

        currentWeight += items[i].Weight * fraction;
        totalProfit += items[i].Profit * fraction;
    }
}
```