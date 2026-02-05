using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StockManagementSystem.Shared.Models;

public class StockItem
{
    [Key]
    [Column("S.No")]
    public int SNo { get; set; }
    
    [Required(ErrorMessage = "Item name is required")]
    [StringLength(200)]
    public string ItemName { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Quantity is required")]
    [Range(0, int.MaxValue, ErrorMessage = "Quantity must be positive")]
    public int Quantity { get; set; }
    
    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;
    
    [StringLength(100)]
    public string? UpdatedBy { get; set; }
    
    public DateTime CreatedDate { get; set; }
    
    public DateTime? UpdatedDate { get; set; }
}
