using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplicationCodeFirstProject.Data;
using WebApplicationCodeFirstProject.DTO;
using WebApplicationCodeFirstProject.Models;

namespace WebApplicationCodeFirstProject.Services;

public interface IPrescriptionService
{
    Task<IActionResult> CreatePrescriptionAsync(PrescriptionCreateDto prescriptionData);
    Task<IActionResult> GetPatientDetailsAsync(int idPatient);
}

public class PrescriptionService(AppDbContext data) : IPrescriptionService
{
    public async Task<IActionResult> CreatePrescriptionAsync(PrescriptionCreateDto PrescriptionData)
    {
        //1. Jeśli pacjent przekazany w żądaniu nie istnieje, wstawiamy nowego pacjenta do tabeli Pacjent
        var patient = await data.Patients.FindAsync(PrescriptionData.IdPatient);
        if (patient == null)
        {
            patient = new Patient
            {
                IdPatient = PrescriptionData.IdPatient,
                FirstName = "NowyPacjent",
                LastName = "NowyPacjent",
                Birthdate = DateTime.Now.AddYears(-30)
            };
            await data.Patients.AddAsync(patient);
            await data.SaveChangesAsync();
        }
        
        //2 .Jeśli lek podany na recepcie nie istnieje, zwracamy błąd.
        var medicamentIds = PrescriptionData.Medicaments.Select(m => m.IdMedicament).ToList();
        var existingMedicaments = await data.Medicaments
            .Where(m => medicamentIds.Contains(m.IdMedicament))
            .Select(m => m.IdMedicament)
            .ToListAsync();

        var missingMedicaments = medicamentIds.Except(existingMedicaments).ToList();
        if (missingMedicaments.Any())
        {
            return new BadRequestObjectResult($"Leki o Id: {string.Join(", ", missingMedicaments)} nie istnieją.");
        }

        //3.  Recepta może obejmować maksymalnie 10 leków. W inny wypadku zwracamy błąd
        if (PrescriptionData.Medicaments.Count > 10)
        {
            return new BadRequestObjectResult("Recepta może obejmować maksymalnie 10 leków.");
        }

        //4. Musimy sprawdzić czy DueData>=Date
        if (PrescriptionData.DueDate < PrescriptionData.Date)
        {
            return new BadRequestObjectResult("DueDate musi być większa lub równa Date.");
        }

        //tu juz po prostu dodaje recepte
        var prescription = new Prescription
        {
            Date = PrescriptionData.Date,
            DueDate = PrescriptionData.DueDate,
            IdPatient = patient.IdPatient,
            IdDoctor = PrescriptionData.IdDoctor
        };
        await data.Prescriptions.AddAsync(prescription);
        await data.SaveChangesAsync();

        // i dodanie do Medicaments
        var addedMedicaments = new List<object>();
        foreach (var medicamentDto in PrescriptionData.Medicaments)
        {
            var prescriptionMedicament = new PrescriptionMedicament
            {
                IdPrescription = prescription.IdPrescription,
                IdMedicament = medicamentDto.IdMedicament,
                Dose = medicamentDto.Dose,
                Details = medicamentDto.Details
            };
            await data.PrescriptionMedicaments.AddAsync(prescriptionMedicament);

            var medicament = await data.Medicaments.FindAsync(medicamentDto.IdMedicament);
            addedMedicaments.Add(new
            {
                medicament.IdMedicament,
                medicament.Name,
                medicament.Description,
                medicament.Type,
                medicamentDto.Dose,
                medicamentDto.Details
            });
        }

        await data.SaveChangesAsync();

        var response = new
        {
            patient = new
            {
                patient.IdPatient,
                patient.FirstName,
                patient.LastName,
                patient.Birthdate
            },
            medicaments = addedMedicaments,
            Date = prescription.Date,
            DueDate = prescription.DueDate
        };

        return new CreatedResult(nameof(CreatePrescriptionAsync), response);
    }

    public async Task<IActionResult> GetPatientDetailsAsync(int idPatient)
    {
        var patient = await data.Patients
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.PrescriptionMedicaments)
            .ThenInclude(pm => pm.Medicament)
            .Include(p => p.Prescriptions)
            .ThenInclude(pr => pr.Doctor)
            .FirstOrDefaultAsync(p => p.IdPatient == idPatient);

        if (patient == null)
        {
            return new NotFoundObjectResult($"Nie znaleziono pacjenta o Id {idPatient}");
        }

        var response = new
        {
            patient.IdPatient,
            patient.FirstName,
            patient.LastName,
            patient.Birthdate,
            Prescriptions = patient.Prescriptions
                .OrderBy(pr => pr.DueDate)
                .Select(pr => new
                {
                    pr.IdPrescription,
                    pr.Date,
                    pr.DueDate,
                    Medicaments = pr.PrescriptionMedicaments.Select(pm => new
                    {
                        pm.Medicament.IdMedicament,
                        pm.Medicament.Name,
                        pm.Medicament.Description,
                        pm.Dose
                    }),
                    Doctor = new
                    {
                        pr.Doctor.IdDoctor,
                        pr.Doctor.FirstName,
                        pr.Doctor.LastName,
                        pr.Doctor.Email
                    }
                })
        };

        return new OkObjectResult(response);
    }
}
