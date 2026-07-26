using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace ExecViewHrk.WebUI.Helpers
{
    /// <summary>
    /// Minimal multi-page PDF builder (JPEG pages). Used when scanner/imported images must be saved as PDF.
    /// Web browsers cannot drive TWAIN/WIA directly; pages are acquired via upload then assembled here.
    /// </summary>
    public static class SimplePdfBuilder
    {
        public static byte[] BuildPdfFromImageStreams(IEnumerable<Stream> imageStreams)
        {
            var jpegPages = new List<byte[]>();
            var sizes = new List<Size>();

            foreach (var stream in imageStreams)
            {
                using (var img = Image.FromStream(stream))
                using (var ms = new MemoryStream())
                {
                    using (var clone = new Bitmap(img.Width, img.Height))
                    using (var g = Graphics.FromImage(clone))
                    {
                        g.DrawImage(img, 0, 0, img.Width, img.Height);
                        clone.Save(ms, ImageFormat.Jpeg);
                    }
                    jpegPages.Add(ms.ToArray());
                    sizes.Add(new Size(img.Width, img.Height));
                }
            }

            if (jpegPages.Count == 0)
                throw new InvalidOperationException("No pages to save as PDF.");

            return BuildPdfFromJpegPages(jpegPages, sizes);
        }

        public static byte[] BuildPdfFromJpegPages(IList<byte[]> jpegPages, IList<Size> sizes)
        {
            using (var ms = new MemoryStream())
            using (var writer = new StreamWriter(ms, new UTF8Encoding(false), 1024, true))
            {
                writer.NewLine = "\n";
                var offsets = new List<long>();

                void WriteObjStart()
                {
                    writer.Flush();
                    offsets.Add(ms.Position);
                }

                writer.WriteLine("%PDF-1.4");

                // 1: Catalog
                WriteObjStart();
                writer.WriteLine("1 0 obj");
                writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
                writer.WriteLine("endobj");

                // 2: Pages (kids filled later — write placeholder then we build kids string first)
                var pageObjNumbers = new List<int>();
                var contentObjNumbers = new List<int>();
                var imageObjNumbers = new List<int>();

                // Object layout:
                // 1 Catalog
                // 2 Pages
                // For each page i: pageObj, contentObj, imageObj
                int nextObj = 3;
                for (int i = 0; i < jpegPages.Count; i++)
                {
                    pageObjNumbers.Add(nextObj++);
                    contentObjNumbers.Add(nextObj++);
                    imageObjNumbers.Add(nextObj++);
                }

                var kids = new StringBuilder("[");
                for (int i = 0; i < pageObjNumbers.Count; i++)
                {
                    if (i > 0) kids.Append(" ");
                    kids.Append(pageObjNumbers[i]).Append(" 0 R");
                }
                kids.Append("]");

                WriteObjStart();
                writer.WriteLine("2 0 obj");
                writer.WriteLine("<< /Type /Pages /Kids " + kids + " /Count " + jpegPages.Count + " >>");
                writer.WriteLine("endobj");

                for (int i = 0; i < jpegPages.Count; i++)
                {
                    int w = sizes[i].Width;
                    int h = sizes[i].Height;
                    int pageObj = pageObjNumbers[i];
                    int contentObj = contentObjNumbers[i];
                    int imageObj = imageObjNumbers[i];

                    var content = string.Format("q\n{0} 0 0 {1} 0 0 cm\n/Im{2} Do\nQ\n", w, h, i + 1);
                    var contentBytes = Encoding.ASCII.GetBytes(content);

                    WriteObjStart();
                    writer.WriteLine(pageObj + " 0 obj");
                    writer.WriteLine(string.Format(
                        "<< /Type /Page /Parent 2 0 R /MediaBox [0 0 {0} {1}] /Contents {2} 0 R /Resources << /XObject << /Im{3} {4} 0 R >> >> >>",
                        w, h, contentObj, i + 1, imageObj));
                    writer.WriteLine("endobj");

                    WriteObjStart();
                    writer.WriteLine(contentObj + " 0 obj");
                    writer.WriteLine("<< /Length " + contentBytes.Length + " >>");
                    writer.WriteLine("stream");
                    writer.Flush();
                    ms.Write(contentBytes, 0, contentBytes.Length);
                    writer.WriteLine();
                    writer.WriteLine("endstream");
                    writer.WriteLine("endobj");

                    var jpeg = jpegPages[i];
                    WriteObjStart();
                    writer.WriteLine(imageObj + " 0 obj");
                    writer.WriteLine(string.Format(
                        "<< /Type /XObject /Subtype /Image /Width {0} /Height {1} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {2} >>",
                        w, h, jpeg.Length));
                    writer.WriteLine("stream");
                    writer.Flush();
                    ms.Write(jpeg, 0, jpeg.Length);
                    writer.WriteLine();
                    writer.WriteLine("endstream");
                    writer.WriteLine("endobj");
                }

                writer.Flush();
                long xrefPos = ms.Position;
                int totalObjects = nextObj - 1;
                writer.WriteLine("xref");
                writer.WriteLine("0 " + (totalObjects + 1));
                writer.WriteLine("0000000000 65535 f ");
                for (int i = 0; i < offsets.Count; i++)
                    writer.WriteLine(offsets[i].ToString("D10") + " 00000 n ");

                writer.WriteLine("trailer");
                writer.WriteLine("<< /Size " + (totalObjects + 1) + " /Root 1 0 R >>");
                writer.WriteLine("startxref");
                writer.WriteLine(xrefPos);
                writer.WriteLine("%%EOF");
                writer.Flush();

                return ms.ToArray();
            }
        }
    }
}
