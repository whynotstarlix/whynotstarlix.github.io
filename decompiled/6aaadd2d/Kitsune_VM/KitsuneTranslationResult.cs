using System.Collections.Generic;

namespace Kitsune_VM;

public class KitsuneTranslationResult
{
	public byte[] Bytecode;

	public List<KitsuneEHEntryRef> EHEntries;

	public KitsuneTranslationResult(byte[] bytecode, List<KitsuneEHEntryRef> ehEntries)
	{
		Bytecode = bytecode;
		EHEntries = ehEntries ?? new List<KitsuneEHEntryRef>();
	}
}
