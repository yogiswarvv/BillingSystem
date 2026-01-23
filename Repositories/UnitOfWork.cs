using BillingSystem.Data;

namespace BillingSystem.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> Repository<T>() where T : class;
        IPatientRepository Patients { get; }
        IBillingRepository Bills { get; }
        IServiceMasterRepository Services { get; }
        IPaymentRepository Payments { get; }
        Task<int> CompleteAsync();
        // Transaction support if needed explicitly, but wrapping scope usually handled in service
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }

    public class UnitOfWork : IUnitOfWork
    {
        private readonly HospitalDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(HospitalDbContext context)
        {
            _context = context;
        }

        public IRepository<T> Repository<T>() where T : class
        {
            if (_repositories.TryGetValue(typeof(T), out var repo))
            {
                return (IRepository<T>)repo;
            }

            var repository = new Repository<T>(_context);
            _repositories.Add(typeof(T), repository);
            return repository;
        }

        // Specific Repositories
        private IPatientRepository? _patientRepository;
        public IPatientRepository Patients => _patientRepository ??= new PatientRepository(_context);

        private IBillingRepository? _billingRepository;
        public IBillingRepository Bills => _billingRepository ??= new BillingRepository(_context);

        private IServiceMasterRepository? _serviceRepository;
        public IServiceMasterRepository Services => _serviceRepository ??= new ServiceMasterRepository(_context);

        private IPaymentRepository? _paymentRepository;
        public IPaymentRepository Payments => _paymentRepository ??= new PaymentRepository(_context);


        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await _context.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await _context.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await _context.Database.RollbackTransactionAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
