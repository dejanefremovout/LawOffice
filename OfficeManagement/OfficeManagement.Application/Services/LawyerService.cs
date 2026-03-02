using OfficeManagement.Domain.Entities;
using OfficeManagement.Domain.Interfaces;
using OfficeManagement.Domain.ViewModels;

namespace OfficeManagement.Application.Services;

public class LawyerService(ILawyerRepository lawyerRepository,
    IOfficeRepository officeRepository) : ILawyerService
{
    private readonly ILawyerRepository _lawyerRepository = lawyerRepository;
    private readonly IOfficeRepository _officeRepository = officeRepository;

    public async Task<LawyerModel?> Get(string lawyerId, string officeId)
    {
        Lawyer? lawyer = await _lawyerRepository.Get(lawyerId, officeId);

        return lawyer is null ? null : new LawyerModel(lawyer);
    }

    public async Task<IEnumerable<LawyerModel>> GetAll(string officeId)
    {
        IEnumerable<Lawyer> lawyers = await _lawyerRepository.GetAll(officeId);

        return lawyers.Select(x => new LawyerModel(x));
    }

    public async Task<LawyerModel> Create(LawyerCreateModel lawyerModel)
    {
        Office? office = await _officeRepository.Get(lawyerModel.OfficeId) ?? throw new ArgumentException("Office not found");

        Lawyer lawyer = Lawyer.New(office, lawyerModel.FirstName, lawyerModel.LastName, lawyerModel.Email);

        lawyer.GenerateNewInvitationCode();

        lawyer = await _lawyerRepository.Add(lawyer);

        // Send an email to the lawyer with the invitation code (not implemented here)

        return new LawyerModel(lawyer);
    }

    public async Task<LawyerModel> Update(LawyerModel lawyerModel)
    {
        Lawyer? lawyer = await _lawyerRepository.Get(lawyerModel.Id, lawyerModel.OfficeId) ?? throw new ArgumentException("Lawyer not found");

        lawyer.SetName(lawyerModel.FirstName, lawyerModel.LastName);
        lawyer.SetEmail(lawyerModel.Email);
        lawyer.SetActive(lawyerModel.Active);

        lawyer = await _lawyerRepository.Update(lawyer);

        return new LawyerModel(lawyer);
    }

    public async Task<bool> ValidateInvitationCode(string lawyerEmail, string invitationCode)
    {
        Lawyer? lawyer = await _lawyerRepository.GetByEmail(lawyerEmail);
        return lawyer is not null && lawyer.ValidateInvitationCode(invitationCode);
    }
}
