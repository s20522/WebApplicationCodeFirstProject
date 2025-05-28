using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace WebApplicationCodeFirstProject.Models;

[Table("Prescription")]
public class Prescription
{
    [Key]
    public int IdPrescription {get; set;}
    
    public DateTime Date { get; set; }
    
    public DateTime DueDate { get; set; }
    
    [Column("IdPatient")]
    public int IdPatient {get; set;}
    
    [Column("IdDoctor")]
    public int IdDoctor {get; set;}
    
    // tutaj koniec podstawowych
    [ForeignKey("IdDoctor")]
    public virtual Doctor Doctor { get; set; } = null!;
    
    [ForeignKey("IdPatient")]
    public virtual Patient Patient { get; set; } = null!;
        
    //Właściwość nawigująca
    public virtual ICollection<PrescriptionMedicament> PrescriptionMedicaments { get; set; } = null!;
    
}