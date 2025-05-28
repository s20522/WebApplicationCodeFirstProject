using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationCodeFirstProject.Models;

[Table("Prescription_Medicament")]
[PrimaryKey("IdMedicament", "IdPrescription")]
public class PrescriptionMedicament
{
    [Column("IdMedicament")]
    public int IdMedicament {get; set;}
    
    [Column("IdPrescription")]
    public int IdPrescription {get; set;}
    
    public int? Dose { get; set; }

    [MaxLength(100)] 
    public string Details { get; set; } = null!;
    
    // tutaj koniec podstawowych
    
    [ForeignKey("IdMedicament")]
    public virtual Medicament Medicament { get; set; } = null!;
    
    [ForeignKey("IdPrescription")]
    public virtual Prescription Prescription { get; set; } = null!;
}