using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleExpenseTrackingWebApp.Models;

public class Masraf
{
    public int Id { get; set; }

    [Required]
    public int AracId { get; set; }

    [Required(ErrorMessage = "Tarih zorunludur")]
    [DataType(DataType.Date)]
    public DateTime Tarih { get; set; } = DateTime.Today;

    [Required(ErrorMessage = "Masraf tipi zorunludur")]
    public MasrafTipi MasrafTipi { get; set; }

    [Required(ErrorMessage = "Tutar zorunludur")]
    [Range(0.01, 1000000, ErrorMessage = "Geçerli bir tutar giriniz")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Tutar { get; set; }

    [StringLength(200)]
    public string? Aciklama { get; set; }

    // Kilometre bilgisi
    public int? Kilometre { get; set; }

    // Yakıt masrafları için litre bilgisi
    public decimal? Litre { get; set; }

    // Navigation property
    [ForeignKey("AracId")]
    public Arac? Arac { get; set; }
}
