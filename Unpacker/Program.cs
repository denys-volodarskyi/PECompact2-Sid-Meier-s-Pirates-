using PECompactLib;

internal class Program
{
    private static void Main()
    {
        var pec = new PEC(@"input\Pirates!.exe");
        pec.LoadStub(0x04F3EA0);
    }
}