using VehicleExpenseTrackingWebApp.Data;
using VehicleExpenseTrackingWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleExpenseTrackingWebApp.Services;

public class KarsilastirmaService
{
    private readonly AppDbContext _dbContext;

    public KarsilastirmaService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

        public async Task<KarsilastirmaSonuc?> KarsilastirAsync(int arac1Id, int arac2Id)
    {
        var arac1 = await _dbContext.Araclar
            .Include(a => a.Masraflar)
            .FirstOrDefaultAsync(a => a.Id == arac1Id);

        var arac2 = await _dbContext.Araclar
            .Include(a => a.Masraflar)
            .FirstOrDefaultAsync(a => a.Id == arac2Id);

        if (arac1 == null || arac2 == null)
            return null;

        return new KarsilastirmaSonuc
        {
            Arac1 = GetAracDetay(arac1),
            Arac2 = GetAracDetay(arac2)
        };
    }

    public async Task<List<AracKarsilastirmaDetay>> GetTumAraclarAsync()
    {
        var araclar = await _dbContext.Araclar
            .Include(a => a.Masraflar)
            .ToListAsync();

        return araclar.Select(GetAracDetay).ToList();
    }

    private AracKarsilastirmaDetay GetAracDetay(Arac arac)
    {
        var yakitMasraflari = arac.Masraflar.Where(m => m.MasrafTipi == MasrafTipi.Yakit).ToList();
        var bakimMasraflari = arac.Masraflar.Where(m => m.MasrafTipi == MasrafTipi.Bakim).ToList();
        
        var sonKm = GetSonKilometre(arac);
        var gidilenKm = sonKm - arac.Kilometre;
        var toplamMasraf = arac.Masraflar.Sum(m => m.Tutar);

        decimal? ortalamaYakitTuketimi = null;
        if (yakitMasraflari.Count >= 2)
        {
            var tuketimler = new List<decimal>();
            var siraliMasraflar = yakitMasraflari
                .Where(m => m.Kilometre.HasValue && m.Litre.HasValue)
                .OrderBy(m => m.Tarih)
                .ToList();

            for (int i = 1; i < siraliMasraflar.Count; i++)
            {
                var onceki = siraliMasraflar[i - 1];
                var simdiki = siraliMasraflar[i];
                var kmFark = simdiki.Kilometre!.Value - onceki.Kilometre!.Value;
                
                if (kmFark > 0 && simdiki.Litre > 0)
                {
                    var tuketim = (simdiki.Litre!.Value / kmFark) * 100;
                    tuketimler.Add(tuketim);
                }
            }

            if (tuketimler.Any())
                ortalamaYakitTuketimi = tuketimler.Average();
        }

        return new AracKarsilastirmaDetay
        {
            AracId = arac.Id,
            Plaka = arac.Plaka,
            Marka = arac.Marka,
            Model = arac.Model,
            Yil = arac.Yil,
            YakitTipi = arac.YakitTipi,
            BaslangicKm = arac.Kilometre,
            SonKm = sonKm,
            GidilenKm = gidilenKm > 0 ? gidilenKm : 0,
            ToplamMasraf = toplamMasraf,
            MasrafSayisi = arac.Masraflar.Count,
            YakitMasrafi = yakitMasraflari.Sum(m => m.Tutar),
            BakimMasrafi = bakimMasraflari.Sum(m => m.Tutar),
            SigortaMasrafi = arac.Masraflar.Where(m => m.MasrafTipi == MasrafTipi.Sigorta).Sum(m => m.Tutar),
            DigerMasraf = arac.Masraflar.Where(m => m.MasrafTipi == MasrafTipi.Diger || m.MasrafTipi == MasrafTipi.Muayene || m.MasrafTipi == MasrafTipi.Yikama).Sum(m => m.Tutar),
            KmBasinaMaliyet = gidilenKm > 0 ? toplamMasraf / gidilenKm : 0,
            OrtalamaYakitTuketimi = ortalamaYakitTuketimi,
            ToplamLitre = yakitMasraflari.Where(m => m.Litre.HasValue).Sum(m => m.Litre!.Value),
            SonMasrafTarihi = arac.Masraflar.Any() ? arac.Masraflar.Max(m => m.Tarih) : null
        };
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

public class KarsilastirmaSonuc
{
    public AracKarsilastirmaDetay Arac1 { get; set; } = new();
    public AracKarsilastirmaDetay Arac2 { get; set; } = new();
}

public class AracKarsilastirmaDetay
{
    public int AracId { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Yil { get; set; }
    public YakitTipi YakitTipi { get; set; }
    public int BaslangicKm { get; set; }
    public int SonKm { get; set; }
    public int GidilenKm { get; set; }
    public decimal ToplamMasraf { get; set; }
    public int MasrafSayisi { get; set; }
    public decimal YakitMasrafi { get; set; }
    public decimal BakimMasrafi { get; set; }
    public decimal SigortaMasrafi { get; set; }
    public decimal DigerMasraf { get; set; }
    public decimal KmBasinaMaliyet { get; set; }
    public decimal? OrtalamaYakitTuketimi { get; set; }
    public decimal ToplamLitre { get; set; }
    public DateTime? SonMasrafTarihi { get; set; }
}

#endregion
