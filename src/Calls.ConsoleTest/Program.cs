
using MediatR;
using Microsoft.Extensions.DependencyInjection;

ServiceCollection serviceCollection = new();
serviceCollection.AddMediatR(options =>
{
    options.RegisterServicesFromAssembly(typeof(BaseType).Assembly);
});

var services = serviceCollection.BuildServiceProvider();

await services.GetService<IMediator>().Send(new RequestB());

Test(typeof(long), typeof(int));

Console.ReadLine();

void Test(Type a, Type b)
{

    Console.WriteLine(a.IsAssignableFrom(b));
    Console.WriteLine(b.IsAssignableFrom(a));
}

public class BaseType { }

public class DeivedType : BaseType { }

public class RequestHandler : IRequestHandler<RequestA>, IRequestHandler<RequestB>
{
    public Task Handle(RequestA request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public Task Handle(RequestB request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

public class RequestA : IRequest
{

}

public class RequestB : RequestA
{

}

public class RequestC : RequestB
{

}