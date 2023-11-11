using System.Collections.Generic;
using System.IO;

namespace PECompactLib;

public class APLib
{
    protected uint Tag;
    protected int BitCount = 0;
    protected int BytePos = 0;

    public byte[] Depack(ReadOnlySpan<byte> source)
    {
        List<byte> destination = new();
        uint offs, len, R0, LWM;
        int done;
        int i;

        R0 = 0;
        LWM = 0;
        done = 0;

        /* first byte verbatim */
        destination.Add(GetByte(source));

        /* main decompression loop */
        while (done == 0)
        {
            if (GetBit(source) > 0)
            {
                if (GetBit(source) > 0)
                {
                    if (GetBit(source) > 0)
                    {
                        offs = 0;

                        for (i = 4; i > 0; i--)
                        {
                            offs = (offs << 1) + GetBit(source);
                        }

                        if (offs > 0)
                        {
                            destination.Add(destination[destination.Count - (int)offs]);
                        }
                        else
                        {
                            destination.Add(0x00);
                        }

                        LWM = 0;

                    }
                    else
                    {

                        offs = GetByte(source);

                        len = 2 + (offs & 0x0001);

                        offs >>= 1;

                        if (offs > 0)
                        {
                            for (; len > 0; len--)
                            {
                                destination.Add(destination[destination.Count - (int)offs]);
                            }
                        }
                        else
                        {
                            done = 1;
                        }

                        R0 = offs;
                        LWM = 1;
                    }

                }
                else
                {

                    offs = GetGamma(source);

                    if (LWM == 0 && offs == 2)
                    {
                        offs = R0;

                        len = GetGamma(source);

                        for (; len > 0; len--)
                        {
                            destination.Add(destination[destination.Count - (int)offs]);
                        }
                    }
                    else
                    {

                        if (LWM == 0)
                        {
                            offs -= 3;
                        }
                        else
                        {
                            offs -= 2;
                        }

                        offs <<= 8;
                        offs += GetByte(source);

                        len = GetGamma(source);

                        if (offs >= 32000)
                        {
                            len++;
                        }

                        if (offs >= 1280)
                        {
                            len++;
                        }

                        if (offs < 128)
                        {
                            len += 2;
                        }

                        for (; len > 0; len--)
                        {
                            destination.Add(destination[destination.Count - (int)offs]);
                        }

                        R0 = offs;
                    }

                    LWM = 1;
                }

            }
            else
            {
                destination.Add(GetByte(source));
                LWM = 0;
            }
        }

        return destination.ToArray();

        byte GetByte(ReadOnlySpan<byte> source) => source[BytePos++];

        uint GetBit(ReadOnlySpan<byte> source)
        {
            uint bit;

            /* check if tag is empty */
            if (--BitCount < 0)
            {
                /* load next tag */
                Tag = GetByte(source);
                BitCount = 7;
            }

            /* shift bit out of tag */
            bit = Tag >> 7 & 0x01;
            Tag <<= 1;

            return bit;
        }

        uint GetGamma(ReadOnlySpan<byte> source)
        {
            uint result = 1;

            /* input gamma2-encoded bits */
            do
            {
                result = (result << 1) + GetBit(source);
            } while (GetBit(source) > 0);

            return result;
        }
    }
}