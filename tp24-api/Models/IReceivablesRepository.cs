namespace tp24_api.Models;

public interface IReceivablesRepository
{
    public Task Create(Receivable receivable);
    public Task<Receivable?> Read(long id);
    public Task<ReceivablesReport> Read(DateTime startDate, bool summaryOnly);
    public Task<bool> Update(Receivable receivable);
    public Task Delete(long id);
}