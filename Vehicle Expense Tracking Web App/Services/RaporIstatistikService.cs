using VehicleExpenseTrackingWebApp.Data;
using VehicleExpenseTrackingWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleExpenseTrackingWebApp.Services;

public class RaporIstatistikService
{
    private readonly AppDbContext _dbContext;

    public RaporIstatistikService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<GenelOzet> GetGenelOzetAsync()
    {
        var araclar = await _dbContext.Araclar.Include(a => a.Masraflar).ToListAsync();
        var tumMasraflar = await _dbContext.Masraflar.ToListAsync();

        return new GenelOzet
        {
            ToplamArac = araclar.Count,
            ToplamMasraf = tumMasraflar.Count,
            ToplamHarcama = tumMasraflar.Sum(m => m.Tutar),
            OrtalamaMasraf = tumMasraflar.Any() ? tumMasraflar.Average(m => m.Tutar) : 0,
            EnYuksekMasraf = tumMasraflar.Any() ? tumMasraflar.Max(m => m.Tutar) : 0,
            EnDusukMasraf = tumMasraflar.Any() ? tumMasraflar.Min(m => m.Tutar) : 0
        };
    }

    public async Task<List<AylikMasrafOzet>> GetAylikMasrafOzetAsync(int? aracId = null, int yil = 0)
    {
        if (yil == 0) yil = DateTime.Now.Year;

        var query = _dbContext.Masraflar.AsQueryable();
        
        if (aracId.HasValue)
            query = query.Where(m => m.AracId == aracId.Value);

        var masraflar = await query
            .Where(m => m.Tarih.Year == yil)
            .ToListAsync();

        var aylikOzet = masraflar
            .GroupBy(m => m.Tarih.Month)
            .Select(g => new AylikMasrafOzet
            {
                Ay = g.Key,
                AyAdi = new DateTime(yil, g.Key, 1).ToString("MMMM"),
                ToplamTutar = g.Sum(m => m.Tutar),
                MasrafSayisi = g.Count()
            })
            .OrderBy(x => x.Ay)
            .ToList();

        // Boş ayları da ekle
        for (int ay = 1; ay <= 12; ay++)
        {
            if (!aylikOzet.Any(a => a.Ay == ay))
            {
                aylikOzet.Add(new AylikMasrafOzet
                {
                    Ay = ay,
                    AyAdi = new DateTime(yil, ay, 1).ToString("MMMM"),
                    ToplamTutar = 0,
                    MasrafSayisi = 0
                });
            }
        }

        return aylikOzet.OrderBy(x => x.Ay).ToList();
    }

    public async Task<List<MasrafTipiDagilim>> GetMasrafTipiDagilimAsync(int? aracId = null)
    {
        var query = _dbContext.Masraflar.AsQueryable();
        
        if (aracId.HasValue)
            query = query.Where(m => m.AracId == aracId.Value);

        var masraflar = await query.ToListAsync();
        var toplam = masraflar.Sum(m => m.Tutar);

        return masraflar
            .GroupBy(m => m.MasrafTipi)
            .Select(g => new MasrafTipiDagilim
            {
                MasrafTipi = g.Key,
                ToplamTutar = g.Sum(m => m.Tutar),
                MasrafSayisi = g.Count(),
                Yuzde = toplam > 0 ? (g.Sum(m => m.Tutar) / toplam) * 100 : 0
            })
            .OrderByDescending(x => x.ToplamTutar)
            .ToList();
    }

    public async Task<List<AracMasrafOzet>> GetAracMasrafOzetAsync()
    {
        var araclar = await _dbContext.Araclar
            .Include(a => a.Masraflar)
            .ToListAsync();

        return araclar.Select(a => new AracMasrafOzet
        {
            AracId = a.Id,
            Plaka = a.Plaka,
            Marka = a.Marka,
            Model = a.Model,
            ToplamMasraf = a.Masraflar.Sum(m => m.Tutar),
            MasrafSayisi = a.Masraflar.Count,
            OrtalamaMasraf = a.Masraflar.Any() ? a.Masraflar.Average(m => m.Tutar) : 0,
            SonMasrafTarihi = a.Masraflar.Any() ? a.Masraflar.Max(m => m.Tarih) : null,
            SonKilometre = GetSonKilometre(a)
        })
        .OrderByDescending(x => x.ToplamMasraf)
        .ToList();
    }


    public async Task<List<YillikKarsilastirma>> GetYillikKarsilastirmaAsync(int? aracId = null)
    {
        var query = _dbContext.Masraflar.AsQueryable();
        
        if (aracId.HasValue)
            query = query.Where(m => m.AracId == aracId.Value);

        var masraflar = await query.ToListAsync();

        return masraflar
            .GroupBy(m => m.Tarih.Year)
            .Select(g => new YillikKarsilastirma
            {
                Yil = g.Key,
                ToplamTutar = g.Sum(m => m.Tutar),
                MasrafSayisi = g.Count(),
                OrtalamaTutar = g.Average(m => m.Tutar)
            })
            .OrderByDescending(x => x.Yil)
            .ToList();
    }


    public async Task<List<AracMasrafOzet>> GetEnMasrafliAraclarAsync(int adet = 5)
    {
        var ozet = await GetAracMasrafOzetAsync();
        return ozet.Take(adet).ToList();
    }


    private int GetSonKilometre(Arac arac)
    {
        var sonMasrafKm = arac.Masraflar
            .Where(m => m.Kilometre.HasValue)
            .OrderByDescending(m => m.Kilometre)
            .Select(m => m.Kilometre!.Value)
            .FirstOrDefault();
        
        return sonMasrafKm > 0 ? sonMasrafKm : arac.Kilometre;
    }
}

#region DTOs

public class GenelOzet
{
    public int ToplamArac { get; set; }
    public int ToplamMasraf { get; set; }
    public decimal ToplamHarcama { get; set; }
    public decimal OrtalamaMasraf { get; set; }
    public decimal EnYuksekMasraf { get; set; }
    public decimal EnDusukMasraf { get; set; }
}

public class AylikMasrafOzet
{
    public int Ay { get; set; }
    public string AyAdi { get; set; } = string.Empty;
    public decimal ToplamTutar { get; set; }
    public int MasrafSayisi { get; set; }
}

public class MasrafTipiDagilim
{
    public MasrafTipi MasrafTipi { get; set; }
    public decimal ToplamTutar { get; set; }
    public int MasrafSayisi { get; set; }
    public decimal Yuzde { get; set; }
}

public class AracMasrafOzet
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public decimal ToplamMasraf { get; set; }
    public int MasrafSayisi { get; set; }
    public decimal OrtalamaMasraf { get; set; }
    public DateTime? SonMasrafTarihi { get; set; }
    public int SonKilometre { get; set; }
}

public class YillikKarsilastirma
{
    public int Yil { get; set; }
    public decimal ToplamTutar { get; set; }
    public int MasrafSayisi { get; set; }
    public decimal OrtalamaTutar { get; set; }
}

#endregion
