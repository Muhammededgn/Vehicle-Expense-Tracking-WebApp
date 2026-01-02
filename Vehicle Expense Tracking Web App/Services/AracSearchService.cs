using VehicleExpenseTrackingWebApp.Data;
using VehicleExpenseTrackingWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleExpenseTrackingWebApp.Services;

public class AracSearchService
{
    private readonly AppDbContext _dbContext;

    public AracSearchService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<AracSearchResult>> AraAsync(
        string? searchTerm = null,
        YakitTipi? yakitTipi = null,
        int? minYil = null,
        int? maxYil = null,
        decimal? minMasraf = null,
        decimal? maxMasraf = null)
    {
        var query = _dbContext.Araclar
            .Include(a => a.Masraflar)
            .AsNoTracking()
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.ToLower().Trim();
            query = query.Where(a => 
                a.Plaka.ToLower().Contains(term) ||
                a.Marka.ToLower().Contains(term) ||
                a.Model.ToLower().Contains(term));
        }

        if (yakitTipi.HasValue)
        {
            query = query.Where(a => a.YakitTipi == yakitTipi.Value);
        }

        if (minYil.HasValue)
        {
            query = query.Where(a => a.Yil >= minYil.Value);
        }
        if (maxYil.HasValue)
        {
            query = query.Where(a => a.Yil <= maxYil.Value);
        }

        var araclar = await query.ToListAsync();

        var results = araclar.Select(a => new AracSearchResult
        {
            Id = a.Id,
            Plaka = a.Plaka,
            Marka = a.Marka,
            Model = a.Model,
            Yil = a.Yil,
            YakitTipi = a.YakitTipi,
            SonKilometre = GetSonKilometre(a),
            ToplamMasraf = a.Masraflar.Sum(m => m.Tutar)
        }).ToList();

        if (minMasraf.HasValue)
        {
            results = results.Where(r => r.ToplamMasraf >= minMasraf.Value).ToList();
        }
        if (maxMasraf.HasValue)
        {
            results = results.Where(r => r.ToplamMasraf <= maxMasraf.Value).ToList();
        }

        return results;
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

public class AracSearchResult
{
    public int Id { get; set; }
    public string Plaka { get; set; } = string.Empty;
    public string Marka { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Yil { get; set; }
    public YakitTipi YakitTipi { get; set; }
    public int SonKilometre { get; set; }
    public decimal ToplamMasraf { get; set; }
}
