using Microsoft.Extensions.DependencyInjection;

namespace Olbrasoft.Mediation;


public class MediationBuilder
{
    public IServiceCollection Services { get; init; }

    public MediationBuilder(IServiceCollection services)
    {
        Services = services ?? throw new ArgumentNullException();
    }
}
