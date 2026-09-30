using dnlib.DotNet;

namespace Kitsune_VM;

public class ExternalCallEntry
{
	public int Id;

	public string TypeName;

	public string MethodName;

	public bool HasReturnValue;

	public string[] ParamTypeNames;

	public string[] MethodGenericArgs;

	public MethodDef SourceDef;
}
