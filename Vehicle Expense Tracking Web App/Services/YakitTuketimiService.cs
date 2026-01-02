using VehicleExpenseTrackingWebApp.Data;
using VehicleExpenseTrackingWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleExpenseTrackingWebApp.Services;

public class YakitTuketimiService
{
    private readonly AppDbContext _dbContext;

    public YakitTuketimiService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Dictionary<int, decimal?>> GetTumYakitTuketimleriAsync(int aracId)
    {
        var yakitMasraflari = await _dbContext.Masraflar
            .Where(m => m.AracId == aracId && m.MasrafTipi == MasrafTipi.Yakit)
            .OrderBy(m => m.Tarih)
            .ToListAsync();

        var tuketimler = new Dictionary<int, decimal?>();

        for (int i = 0; i < yakitMasraflari.Count; i++)
        {
            var masraf = yakitMasraflari[i];
            
            if (i == 0 || !masraf.Kilometre.HasValue || !masraf.Litre.HasValue)
            {
                tuketimler[masraf.Id] = null;
                continue;
            }

            var oncekiMasraf = yakitMasraflari[i - 1];
            
            if (!oncekiMasraf.Kilometre.HasValue)
            {
                tuketimler[masraf.Id] = null;
                continue;
            }

            var kmFark = masraf.Kilometre.Value - oncekiMasraf.Kilometre.Value;
            
            if (kmFark <= 0 || masraf.Litre <= 0)
            {
                tuketimler[masraf.Id] = null;
                continue;
            }

            tuketimler[masraf.Id] = (masraf.Litre.Value / kmFark) * 100;
        }

        return tuketimler;
    }

    public async Task<int?> GetSonKilometreAsync(int aracId)
    {
        var sonMasraf = await _dbContext.Masraflar
            .Where(m => m.AracId == aracId && m.Kilometre.HasValue)
            .OrderByDescending(m => m.Tarih)
            .ThenByDescending(m => m.Kilometre)
            .FirstOrDefaultAsync();

        return sonMasraf?.Kilometre;
    }
}
