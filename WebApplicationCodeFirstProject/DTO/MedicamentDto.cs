namespace WebApplicationCodeFirstProject.DTO;

public class MedicamentDto
{
    public int IdMedicament { get; set; }
    public int? Dose { get; set; }
    public string Details { get; set; } = null!;
    
    public string Type { get; set; }
}