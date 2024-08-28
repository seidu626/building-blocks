using Microsoft.AspNetCore.Http;

namespace BuildingBlocks.Infrastructure;

/// <summary>
/// Holds some state for the current HttpContext or thread
/// </summary>
/// <typeparam name="T">The type of data to store</typeparam>
public class ContextState<T> where T : class
{
    private readonly string _name;
    private readonly Func<T> _defaultValue;
    private readonly IHttpContextAccessor _contextAccessor;

    public ContextState(string name, IHttpContextAccessor contextAccessor)
    {
        _name = name;
        _contextAccessor = contextAccessor;
    }

    public ContextState(string name, Func<T> defaultValue)
    {
        _name = name;
        _defaultValue = defaultValue;
    }

    public T GetState()
    {
        var key = BuildKey();

        if (_contextAccessor.HttpContext == null)
        {
            var data = CallContext<object>.GetData(key);

            if (data == null)
            {
                if (_defaultValue != null)
                {
                    CallContext<object>.SetData(key, data = _defaultValue());
                    return data as T;
                }
            }

            return data as T;
        }

        if (_contextAccessor.HttpContext.Items[key] == null)
        {
            _contextAccessor.HttpContext.Items[key] = _defaultValue?.Invoke();
        }

        return _contextAccessor.HttpContext.Items[key] as T;
    }

    public void SetState(T state)
    {
        if (_contextAccessor.HttpContext == null)
        {
            CallContext<object>.SetData(BuildKey(), state);
        }
        else
        {
            _contextAccessor.HttpContext.Items[BuildKey()] = state;
        }
    }

    public void RemoveState()
    {
        var key = BuildKey();

        if (_contextAccessor.HttpContext == null)
        {
            CallContext<object>.FreeNamedDataSlot(key);
        }
        else
        {
            if (_contextAccessor.HttpContext.Items.Any(x => (string)x.Key == key))
            {
                _contextAccessor.HttpContext.Items.Remove(key);
            }
        }
    }

    private string BuildKey()
    {
        return "__ContextState." + _name;
    }
}