using System.ComponentModel.DataAnnotations;

namespace WebApplicationCodeFirstProject.DTO;

public class PrescriptionCreateDto
{
    public DateTime Date { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public int IdPatient {get; set;}
    
    public int IdDoctor {get; set;}
    
    
    public List<MedicamentDto> Medicaments { get; set; } = new();
}