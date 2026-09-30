using System;
using System.Runtime.CompilerServices;

public sealed class 瀠罔톼衟湎斁꼹짖僋븲褭祝
{
	private readonly long[] 鰤훀먏蝇杻쥛謎엏胠鎢鳪抣;

	private readonly long 糯夻哒祝鿋觳囁蔤姆脊좴혚馵罬숅;

	public 瀠罔톼衟湎斁꼹짖僋븲褭祝(int count)
	{
		鰤훀먏蝇杻쥛謎엏胠鎢鳪抣 = new long[count];
		Random random = new Random();
		糯夻哒祝鿋觳囁蔤姆脊좴혚馵罬숅 = ((long)random.Next() << 32) | (uint)random.Next();
	}

	[SpecialName]
	public long get_Item(int i)
	{
		return 鰤훀먏蝇杻쥛謎엏胠鎢鳪抣[i] ^ 糯夻哒祝鿋觳囁蔤姆脊좴혚馵罬숅;
	}

	[SpecialName]
	public void set_Item(int i, long value)
	{
		鰤훀먏蝇杻쥛謎엏胠鎢鳪抣[i] = value ^ 糯夻哒祝鿋觳囁蔤姆脊좴혚馵罬숅;
	}

	private static bool 잯鏈픟犒靚웎佤巿얽飚()
	{
		int num = 13;
		do
		{
			num += 2;
		}
		while (num < 743);
		return false;
	}
}
