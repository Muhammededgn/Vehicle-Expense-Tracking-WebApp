using System.ComponentModel.DataAnnotations;

namespace VehicleExpenseTrackingWebApp.Models;

public class Arac
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Plaka zorunludur")]
    [StringLength(10)]
    public string Plaka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Marka zorunludur")]
    [StringLength(50)]
    public string Marka { get; set; } = string.Empty;

    [Required(ErrorMessage = "Model zorunludur")]
    [StringLength(50)]
    public string Model { get; set; } = string.Empty;

    [Range(1900, 2100, ErrorMessage = "Geçerli bir yıl giriniz")]
    public int Yil { get; set; }

    public YakitTipi YakitTipi { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Kilometre 0'dan küçük olamaz")]
    public int Kilometre { get; set; } = 0;

    [StringLength(200)]
    public string? Aciklama { get; set; }

    // Navigation property
    public ICollection<Masraf> Masraflar { get; set; } = new List<Masraf>();
}
