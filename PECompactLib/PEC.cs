using DV.Formats.PortableExecutable;
using DV.IO;

namespace PECompactLib;
public class PEC
{
    private readonly Image Img;
    private readonly ImageStream ImgStream;

    public PEC(string path)
    {
        using var fs = File.OpenRead(path);
        var bs = new DV.IO.ByteStream(fs);
        Img = ImageReader.Load(bs);
        ImgStream = new ImageStream(Img);
    }

    public void LoadStub(uint rva)
    {
        if (!ImgStream.TrySeekRVA(rva))
            throw new NotImplementedException();

        var rdr = new ByteReader(ImgStream);

        var hdr = new StubHeader
        {
            PackedStubRVA = rdr.U32(),
            UnpackedStubSize = rdr.U32(),
            StubEntryFnOfs = rdr.U32(),

            APLibDepackFnRVA = rdr.U32(),

            VirtualAlloc = rdr.U32(),
            VirtualFree = rdr.U32(),

            ImageBase = rdr.U32(),

            LoadLibraryA = rdr.U32(),
            GetProcAddress = rdr.U32()
        };

        if (!ImgStream.TrySeekRVA(hdr.PackedStubRVA))
            throw new NotImplementedException();

        var ap = new APLib();
        var buf_unp = ap.Depack(ImgStream.GetSpan());

        if (buf_unp.Length != hdr.UnpackedStubSize)
            throw new NotImplementedException();
    }
}
