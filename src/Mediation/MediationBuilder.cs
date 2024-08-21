using Microsoft.Extensions.DependencyInjection;

namespace Olbrasoft.Mediation;


public class MediationBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services ?? throw new ArgumentNullException();
}