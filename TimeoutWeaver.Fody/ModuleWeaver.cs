using Fody;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace TimeoutWeaver.Fody;

public class ModuleWeaver : BaseModuleWeaver
{
    public override void Execute()
    {
        var timeoutAttribute = ModuleDefinition
            .Types
            .SelectMany(t => t.Methods)
            .Where(m => m.HasCustomAttributes)
            .SelectMany(m => m.CustomAttributes)
            .FirstOrDefault(a => a.AttributeType.Name == "Timeout");

        if (timeoutAttribute == null)
            return;

        var timeoutValue = (int)timeoutAttribute.ConstructorArguments[0].Value;

        foreach (var method in ModuleDefinition.Types
                     .SelectMany(t => t.Methods)
                     .Where(m => m.CustomAttributes.Any(a => a.AttributeType.Name == "Timeout")))
        {
            InjectTimeoutLogic(method, timeoutValue);
        }
    }


    #region GetAssembliesForScanning

    public override IEnumerable<string> GetAssembliesForScanning()
    {
        yield return "netstandard";
        yield return "mscorlib";
    }

    #endregion

    private void InjectTimeoutLogic(MethodDefinition method, int timeoutInSeconds)
    {
        var timeoutCtor = typeof(CancellationTokenSource).GetConstructor(new[] { typeof(int) });
        var timeoutCtorRef = ModuleDefinition.ImportReference(timeoutCtor);

        var il = method.Body.GetILProcessor();
        il.Body.SimplifyMacros();

        var startInstruction = method.Body.Instructions.First();
        il.InsertBefore(startInstruction, il.Create(OpCodes.Ldc_I4, timeoutInSeconds * 1000));
        il.InsertAfter(startInstruction, il.Create(OpCodes.Newobj, timeoutCtorRef));

        var timeoutVar = new VariableDefinition(ModuleDefinition.ImportReference(typeof(CancellationTokenSource)));
        method.Body.Variables.Add(timeoutVar);

        il.InsertAfter(startInstruction, il.Create(OpCodes.Stloc, timeoutVar));
        il.InsertAfter(startInstruction, il.Create(OpCodes.Ldloc, timeoutVar));
        il.InsertAfter(startInstruction,
            il.Create(OpCodes.Callvirt,
                ModuleDefinition.ImportReference(
                    typeof(CancellationTokenSource).GetProperty("Token").GetGetMethod())));

        il.Body.OptimizeMacros();
    }
}