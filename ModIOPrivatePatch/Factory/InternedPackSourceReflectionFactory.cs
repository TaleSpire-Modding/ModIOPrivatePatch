using System;
using System.Reflection;
using TaleSpire.ContentManagement;

internal static class InternedPackSourceReflectionFactory
{
    private static readonly ConstructorInfo _ctor;

    static InternedPackSourceReflectionFactory()
    {
        _ctor = typeof(InternedPackSource).GetConstructor(
            BindingFlags.Instance | BindingFlags.NonPublic,
            binder: null,
            new[] { typeof(PackSourceKind) },
            modifiers: null
        );

        if (_ctor == null)
            throw new MissingMethodException(
                nameof(InternedPackSource),
                "Internal constructor InternedPackSource(PackSourceKind) not found.");
    }

    public static InternedPackSource Create(PackSourceKind kind)
    {
        // Boxing happens here — unavoidable with reflection
        object boxed = _ctor.Invoke(new object[] { kind });
        return (InternedPackSource)boxed;
    }
}
