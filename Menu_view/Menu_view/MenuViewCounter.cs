
namespace MenuViews;

public sealed class MenuViewCounter
{
    private readonly object _lock = new();


public int Views;

    public void RegisterView()
    {
        lock (_lock)
        {
            Views++;
        }
    }


}





