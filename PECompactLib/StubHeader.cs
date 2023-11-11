namespace PECompactLib;

public struct StubHeader
{
    public uint PackedStubRVA;
    public uint UnpackedStubSize;
    public uint StubEntryFnOfs;
    public uint APLibDepackFnRVA;
    public uint VirtualAlloc;
    public uint VirtualFree;
    public uint ImageBase;
    public uint LoadLibraryA;
    public uint GetProcAddress;
}