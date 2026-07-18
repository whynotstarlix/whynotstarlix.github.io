using System;

namespace nspector.Native.NVAPI2;

[Flags]
public enum NVDRS_GPU_SUPPORT : uint
{
	None = 0u,
	Geforce = 1u,
	Quadro = 2u,
	Nvs = 3u
}
